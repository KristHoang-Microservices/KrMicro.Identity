using KrMicro.Identity.Models;

namespace KrMicro.Identity.CQS.Queries;

public class GetCustomerDetailQueryResult
{
    public GetCustomerDetailQueryResult(Customer customer)
    {
        Id = customer.Id;
        UserId = customer.UserId;
        Point = customer.Point;
        FullAddress = customer.FullAddress;
        DOB = customer.DOB;
        Name = customer.UserInformation.FullName;
        Phone = customer.UserInformation.PhoneNumber;
    }

    public GetCustomerDetailQueryResult()
    {
    }

    public short? Id { get; set; }

    public string UserId { get; set; }

    public int Point { get; set; }

    public string FullAddress { get; set; } = string.Empty;

    public DateTime DOB { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }
}