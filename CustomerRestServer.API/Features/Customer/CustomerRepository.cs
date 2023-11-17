using System.Text.Json;
using CustomerRestServer.Models;

namespace CustomerRestServer.API.Features.Customer;

public class CustomerRepository : ICustomerRepository
{
    private const string STORAGE_FILE_NAME = "customer_storage.json";
    private readonly JsonSerializerOptions JSON_OPTIONS = new() 
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private CustomerModel[]? _storage;
    private readonly object _lock = new();

    public CustomerRepository()
    {
        LoadStorage();
    }

    private void LoadStorage()
    {
        if (_storage is null)
        {
            lock (_lock)
            {
                if (File.Exists(STORAGE_FILE_NAME))
                {
                    string fileContent = File.ReadAllText(STORAGE_FILE_NAME);

                    if (!string.IsNullOrEmpty(fileContent))
                        _storage = JsonSerializer.Deserialize<CustomerModel[]>(fileContent) ?? new CustomerModel[] {};
                    else
                        _storage = new CustomerModel[] { };
                }
                else
                {
                    _storage = new CustomerModel[] { };
                }
            }    
        }
        
    }
    
    private void SaveStorage()
    {
        if (_storage is not null)
        {
            //It doesn't lock because it is used inside the AddCustomer
            //which is already locking
            string content = JsonSerializer.Serialize(_storage, JSON_OPTIONS);
            File.WriteAllText(STORAGE_FILE_NAME, content);
        }
    }

    public uint GetCustomersMaxId()
    {
        uint id = 0;
        
        if (_storage is not null && _storage.Length > 0)
            id = _storage?.Max(c => c.Id) ?? 0;

        return id;
    }

    public CustomerModel[] GetCustomers() => _storage ?? new CustomerModel[] {};

    public void AddCustomer(CustomerModel model)
    {
        lock (_lock)
        {
            var customer = _storage!.FirstOrDefault(c => c.Id == model.Id);

            if (customer is not null)
                throw new Exception($"Id {model.Id} has been already used.");

            int idx = Array.BinarySearch(_storage!.ToArray(), model);
            idx = ~idx;
            
            InsertIntoArray(model, idx);
            
            SaveStorage();
        }
    }

    private void InsertIntoArray(CustomerModel model, int idx)
    {
        CustomerModel[] newArr = new CustomerModel[_storage!.Length + 1];

        Array.Copy(_storage, newArr, idx);
		
        newArr[idx] = model;
        
        Array.Copy(_storage, idx, newArr, idx+1, _storage.Length - idx);
            
        _storage = newArr;
    }
}