using System.Text.Json;

namespace CustomerRestServer.Consumer;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Inform the amount of customers to be created.");
        
        string userInput = Console.ReadLine();
        int customersAmount = int.Parse(userInput);
        
        var exec = new CustomerManagementRestClient();
            
        var customers = await exec.Execute(customersAmount);
        
        Console.WriteLine("All customers created:");
        
        string serialized = JsonSerializer.Serialize(
            customers,
            new JsonSerializerOptions { WriteIndented = true });
        
        Console.WriteLine(serialized);
    }
}