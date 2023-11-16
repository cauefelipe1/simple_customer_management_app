using CustomerRestServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRestServer.API.Features.Customer;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _repo;

    public CustomerController(ICustomerRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public CustomerModel[] GetCustomers()
    {
        var customers = _repo.GetCustomers();

        return customers;
    }

    [HttpPost]
    public ActionResult AddCustomer([FromBody] CustomerModel[] models)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        foreach (var model in models)
        {
            try
            {
                _repo.AddCustomer(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        return NoContent();
    }
}