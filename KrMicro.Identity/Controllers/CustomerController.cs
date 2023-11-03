using KrMicro.Core.Services;
using KrMicro.Identity.CQS.Commands;
using KrMicro.Identity.CQS.Queries;
using KrMicro.Identity.Models;
using KrMicro.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KrMicro.Identity.Controllers;

[Controller]
[Route("Customers")]
[Consumes("application/json")]
public class CustomerController : Controller
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<Customer>> GetCurrentProfile()
    {
        var userName = JwtUtils.GetUserNameByToken(HttpContext.Request.Headers.Authorization);
        if (userName == string.Empty) return Unauthorized("Access token not valid");
        var customer = await _customerService.GetDetailAsync(c => c.UserInformation.UserName == userName);

        return Ok(customer);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetAllCustomerQueryResult>> GetAllCustomer()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(new GetAllCustomerQueryResult(customers.ToList()));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<Customer>> GetCustomerById(short id)
    {
        var customer = await _customerService.GetDetailAsync(c => c.Id == id);
        return customer is not null ? Ok(customer) : BadRequest("Not found");
    }

    [HttpPatch]
    [Route("{userId}")]
    [Authorize]
    public async Task<ActionResult> UpdateCustomerProfile([FromRoute] string userId,
        [FromBody] UpdateCustomerCommandRequest request)
    {
        if (JwtUtils.GetUserRoleByToken(HttpContext.Request.Headers.Authorization) == "Customer")
        {
            var userName = JwtUtils.GetUserNameByToken(HttpContext.Request.Headers.Authorization);
            var validate = await _customerService.GetDetailAsync(c => c.UserInformation.UserName == userName);
            if (validate is null) return BadRequest();
            if (userId != validate.UserId) return Forbid("You are not allowed");
        }

        var result = await _customerService.UpdateCustomerAsync(userId, request);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }
}