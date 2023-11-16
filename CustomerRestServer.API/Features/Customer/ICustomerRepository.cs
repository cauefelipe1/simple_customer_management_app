using CustomerRestServer.Models;

namespace CustomerRestServer.API.Features.Customer;

public interface ICustomerRepository
{
    CustomerModel[] GetCustomers();
    void AddCustomer(CustomerModel model);
}