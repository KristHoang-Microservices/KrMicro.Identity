using System.Linq.Expressions;
using KrMicro.Identity.CQS.Commands;
using KrMicro.Identity.CQS.Queries;
using KrMicro.Identity.Infrastructure;
using KrMicro.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KrMicro.Patterns.Template;

public interface ICustomerService : Core.Services.IBaseService<Customer>
{
    public Task<IdentityResult> UpdateCustomerAsync(string userId, UpdateCustomerCommandRequest request);
    public new Task<GetCustomerDetailQueryResult?> GetDetailAsync(Expression<Func<Customer, bool>> predicate);
}

public class CustomerService : Core.Services.BaseRepositoryService<Customer, KrIdentityDbContext>, ICustomerService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomerService(KrIdentityDbContext dataContext, UserManager<ApplicationUser> userManager) :
        base(dataContext)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> UpdateCustomerAsync(string userId, UpdateCustomerCommandRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        user.FullName = request.FullName ?? user.FullName;
        user.Email = request.Email ?? user.Email;
        user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);

        var customer = user.Customer;
        customer!.DOB = request.DOB ?? customer.DOB;
        customer.FullAddress = request.FullAddress ?? customer.FullAddress;

        await UpdateAsync(customer);

        return result;
    }

    public new async Task<GetCustomerDetailQueryResult?> GetDetailAsync(Expression<Func<Customer, bool>> predicate)
    {
        var result = await DataContext.Set<Customer>()
            .AsNoTracking()
            .Include(c => c.UserInformation)
            .Where(predicate)
            .Select(c => new
            {
                c.Id, c.FullAddress, c.DOB, c.Point, Name = c.UserInformation.FullName,
                Phone = c.UserInformation.PhoneNumber, c.UserId
            })
            .FirstOrDefaultAsync();

        return result is not null
            ? new GetCustomerDetailQueryResult
            {
                Id = result!.Id ?? 0,
                UserId = result.UserId,
                Name = result.Name,
                Phone = result.Phone,
                FullAddress = result.FullAddress,
                DOB = result.DOB,
                Point = result.Point
            }
            : null;
    }
}