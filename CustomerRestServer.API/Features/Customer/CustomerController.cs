using CustomerRestServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRestServer.API.Features.Customer;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

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
    public ActionResult AddCustomer([FromBody] CustomerModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        
        _repo.AddCustomer(model);

        return Ok();
    }
}