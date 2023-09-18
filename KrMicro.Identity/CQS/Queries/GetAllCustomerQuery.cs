using KrMicro.Core.CQS.Query.Abstraction;
using KrMicro.Identity.Models;

namespace KrMicro.Identity.CQS.Queries;

public class GetAllCustomerQueryResult : GetAllQueryResult<Customer>
{
    public GetAllCustomerQueryResult(List<Customer> list) : base(list)
    {
    }
}