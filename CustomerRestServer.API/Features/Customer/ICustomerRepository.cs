using CustomerRestServer.Models;

namespace CustomerRestServer.API.Features.Customer;

public interface ICustomerRepository
{
    uint GetCustomersMaxId();
    CustomerModel[] GetCustomers();
    void AddCustomer(CustomerModel model);
}