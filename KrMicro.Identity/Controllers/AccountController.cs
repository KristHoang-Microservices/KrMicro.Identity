using KrMicro.Core.CQS.Query.Abstraction;
using KrMicro.Core.Services;
using KrMicro.Identity.CQS.Commands;
using KrMicro.Identity.Models;
using KrMicro.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KrMicro.Identity.Controllers;

[Route("Identity/[action]")]
[Authorize]
[Consumes("application/json")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> SignUp([FromBody] SignUpCommandRequest request)
    {
        var result = await _accountService.SignUpForCustomerAsync(request);
        if (result.Succeeded)
        {
            var roleAssign = await _accountService.AssignRoleToUserAsync(request.UserName, "Customer");
            if (roleAssign.Succeeded)
                return new ObjectResult(new
                {
                    accessToken =
                        await _accountService.LoginAndGetAccessTokenAsync(
                            new LoginCommandRequest(request.UserName, request.Password))
                });

            return StatusCode(500, roleAssign);
        }

        return BadRequest(result);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> SignUpAdmin([FromBody] SignUpAdminCommandRequest request)
    {
        var result = await _accountService.SignUpForAdminAsync(request);
        if (result.Succeeded)
        {
            var roleAssign = await _accountService.AssignRoleToUserAsync(request.UserName, "Admin");
            if (roleAssign.Succeeded)
                return new ObjectResult(new
                {
                    accessToken =
                        await _accountService.LoginAndGetAccessTokenAsync(
                            new LoginCommandRequest(request.UserName, request.Password))
                });

            return StatusCode(500, roleAssign);
        }

        return BadRequest(result);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] LoginCommandRequest request)
    {
        var result = await _accountService.LoginAndGetAccessTokenAsync(request);

        if (string.IsNullOrEmpty(result)) return BadRequest("Login failed");

        return new ObjectResult(new { accessToken = result });
    }

    [HttpPatch]
    [Route("{id}")]
    public async Task<ActionResult> UpdateUserProfile([FromRoute] string id,
        [FromBody] UpdateUserCommandRequest request)
    {
        var result = await _accountService.UpdateUserAsync(id, request);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApplicationUser>> GetCurrentUser()
    {
        var userName = JwtUtils.GetUserNameByToken(HttpContext.Request.Headers.Authorization);
        if (userName == string.Empty) return Unauthorized("Access token not valid");
        var user = await _accountService.GetDetailAsync(c => c.UserName == userName);
        return Ok(user);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetAllQueryResult<ApplicationUser>>> GetAllUsers()
    {
        var users = (await _accountService.GetAllAsync()).ToList();
        return Ok(new GetAllQueryResult<ApplicationUser>(users));
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApplicationUser>> GetUserById(string id)
    {
        var user = await _accountService.GetDetailAsync(u => u.Id == id);
        return user is not null ? Ok(user) : BadRequest();
    }

    [HttpPost]
    [Route("{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<string>> ResetPassword(string userId, [FromBody] ResetPasswordRequest request)
    {
        var result = await _accountService.ResetPasswordAsync(userId, request);

        if (result.Succeeded) return Ok(result.Succeeded);

        return BadRequest(result);
    }

    [HttpPost]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgetPasswordRequest request)
    {
        var user = await _accountService.GetDetailAsync(c => c.NormalizedEmail == request.Email.ToUpper());
        if (user is null) return BadRequest("User not found, please check your email!");
        var result = await _accountService.ForgetPasswordAsync(user);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<string> Test()
    {
        return await Task.FromResult("ok");
    }
}