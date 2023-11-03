using KrMicro.Core.CQS.Command.Jwt;
using KrMicro.Core.Services;
using KrMicro.Identity.Constants;
using KrMicro.Identity.CQS.Commands;
using KrMicro.Identity.Infrastructure;
using KrMicro.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace KrMicro.Identity.Services;

public interface IAccountService : IBaseService<ApplicationUser>
{
    public Task<IdentityResult> SignUpForCustomerAsync(SignUpCommandRequest request);
    public Task<IdentityResult> SignUpForAdminAsync(SignUpAdminCommandRequest request);
    public Task<IdentityResult> SignUpForEmployeeAsync(SignUpAdminCommandRequest request);
    public Task<string> LoginAndGetAccessTokenAsync(LoginCommandRequest request);
    public Task<ApplicationUser?> GetUserByTokenAsync(string accessToken);
    public Task<IdentityResult> AssignRoleToUserAsync(ApplicationUser user, string role);
    public Task<IdentityResult> AssignRoleToUserAsync(string userName, string role);
    public Task<IdentityResult> DeactivateUserAsync(ApplicationUser user);
    public Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserCommandRequest request);
    public Task<IdentityResult> ResetPasswordAsync(string userId, ResetPasswordRequest request);
    public Task<IdentityResult> ForgetPasswordAsync(ApplicationUser user);
}

public class AccountService : BaseRepositoryService<ApplicationUser, KrIdentityDbContext>, IAccountService
{
    private readonly IConfiguration _configuration;
    private readonly ICustomerService _customerService;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration, RoleManager<IdentityRole> roleManager, ICustomerService customerService,
        KrIdentityDbContext context) : base(context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _roleManager = roleManager;
        _customerService = customerService;
    }

    public async Task<IdentityResult> SignUpForCustomerAsync(SignUpCommandRequest request)
    {
        var newUser = new ApplicationUser
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber
        };

        if (request.DOB.AddYears(AccountConstants.MIN_AGE).CompareTo(DateTime.UtcNow) > 0)
            return IdentityResult.Failed(new IdentityError
            {
                Description = "Age must be greater than " + AccountConstants.MIN_AGE,
                Code = "MinAgeRequired"
            });

        var result = await _userManager.CreateAsync(newUser, request.Password);
        var user = await _userManager.FindByNameAsync(request.UserName);

        if (!result.Succeeded) return result;

        var newCustomer = new Customer
        {
            DOB = request.DOB,
            FullAddress = request.FullAddress,
            UserInformation = user,
            UserId = user.Id
        };

        await _customerService.InsertAsync(newCustomer);

        return result;
    }

    public async Task<IdentityResult> SignUpForAdminAsync(SignUpAdminCommandRequest request)
    {
        var newUser = new ApplicationUser
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.UserName
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded) return result;

        return result;
    }

    public async Task<IdentityResult> SignUpForEmployeeAsync(SignUpAdminCommandRequest request)
    {
        var newUser = new ApplicationUser
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.UserName
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded) return result;

        return result;
    }

    public async Task<string> LoginAndGetAccessTokenAsync(LoginCommandRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, false);

        if (!result.Succeeded) return string.Empty;

        var user = await _signInManager.UserManager.FindByNameAsync(request.Username);
        var role = await _signInManager.UserManager.GetRolesAsync(user);

        return JwtUtils.GenerateToken(
            new GenerateJwtCommandRequest(user.Id, request.Username, role.FirstOrDefault() ?? ""),
            _configuration["Jwt:Key"], _configuration["Jwt:ValidIssuer"], _configuration["Jwt:ValidAudience"]);
    }

    public async Task<ApplicationUser?> GetUserByTokenAsync(string accessToken)
    {
        var userName = JwtUtils.GetUserNameByToken(accessToken);
        if (userName == null) return null;
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<IdentityResult> AssignRoleToUserAsync(ApplicationUser user, string role)
    {
        var result = await _userManager.AddToRoleAsync(user, role);
        return result;
    }

    public async Task<IdentityResult> AssignRoleToUserAsync(string userName, string role)
    {
        var user = await _userManager.FindByNameAsync(userName);
        var result = await _userManager.AddToRoleAsync(user, role);
        return result;
    }

    public async Task<IdentityResult> DeactivateUserAsync(ApplicationUser user)
    {
        var result = await _userManager.SetLockoutEnabledAsync(user, true);
        if (!result.Succeeded) return result;

        result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddYears(100));
        return result;
    }

    public async Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserCommandRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        user.FullName = request.FullName ?? user.FullName;
        user.Email = request.Email ?? user.Email;
        user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);

        return result;
    }

    public async Task<IdentityResult> ResetPasswordAsync(string userId, ResetPasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
        return result;
    }

    public async Task<IdentityResult> ForgetPasswordAsync(ApplicationUser user)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        // Mail this to user
        return IdentityResult.Success;
    }
}