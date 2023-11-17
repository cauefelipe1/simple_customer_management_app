using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CustomerRestServer.Models;

namespace CustomerRestServer.Consumer;

public class CustomerManagementRestClient
{
    private readonly string[] _firstNames = { "Leia", "Sadie", "Jose", "Sara", "Frank",
                                              "Dewey", "Tomas",  "Joel",  "Lukas",  "Carlos" };

    private readonly string[] _lastNames = { "Liberty", "Ray", "Harrison", "Ronan", "Drew",
                                             "Powell", "Larsen", "Chan", "Anderson", "Lane" };
    
    private readonly string _baseUrl = "http://localhost:5186";

    private readonly HttpClient _httpClient;
    
    public CustomerManagementRestClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl)
        };

        _httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<CustomerModel>> Execute(int customersAmount)
    {
        var customers = await BuildCustomers(customersAmount);
        var tasks = new List<Task>();
        
        int i = 0;
        var random = new Random();

        while (i < customersAmount)
        {
            int customersToSend = random.Next(2, 10); //Just an arbitrary limit.

            var postBody = customers.Skip(i).Take(customersToSend);

            var postTask = CreateCustomers(postBody);
            var getTask = GetCustomers();
            
            tasks.Add(postTask);
            tasks.Add(getTask);

            i += customersToSend;
        }

        Console.WriteLine("Waiting requests.");
        await Task.WhenAll(tasks);

        var result = new List<CustomerModel>();
        foreach (var t in tasks)
        {
            if (t is Task<CustomerModel[]> task)
                result.AddRange(task.Result);
        }

        return result;
    }

    private async Task<List<CustomerModel>> BuildCustomers(int amount)
    {
        var result = new List<CustomerModel>();
        uint currentId = await GetMaxCustomersId();
        
        var random = new Random();
        
        for (int i = 0; i < amount; i++)
        {
            int firstNameIdx = random.Next(0, _firstNames.Length - 1);
            int lastNameIdx = random.Next(0, _lastNames.Length - 1);
            
            result.Add(new()
            {
                Id = ++currentId,
                LastName = _lastNames[lastNameIdx],
                FirstName = _firstNames[firstNameIdx],
                Age = (byte)random.Next(10, 90)
            });
        }

        return result;
    }
    
    private Task CreateCustomers(IEnumerable<CustomerModel> customers)
    {
        Console.WriteLine($"Sending request to create {customers.Count()} customers.");
        
        var body = new StringContent(JsonSerializer.Serialize(customers), Encoding.UTF8, "application/json");
        
        return _httpClient.PostAsync("customer", body);
    }
    
    private async Task<CustomerModel[]> GetCustomers()
    {
        Console.WriteLine("Retrieving customers");

        var resp = await _httpClient.GetAsync($"customer");

        if (!resp.IsSuccessStatusCode)
            return new CustomerModel[] {};
        
        string customers = await resp.Content.ReadAsStringAsync();

        var jsonOpt = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        return JsonSerializer.Deserialize<CustomerModel[]>(customers, jsonOpt) ?? new CustomerModel[] {};
    }
    
    private async Task<uint> GetMaxCustomersId()
    {
        Console.WriteLine("Retrieving customer ID seed.");

        var resp = await _httpClient.GetAsync($"customer/max-id");

        if (!resp.IsSuccessStatusCode)
            return 0;
        
        string id = await resp.Content.ReadAsStringAsync();

        var jsonOpt = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        return JsonSerializer.Deserialize<uint>(id, jsonOpt);
    }
}