namespace KrMicro.Identity.CQS.Queries;

public class GetCustomerDetailQueryResult
{
    public short Id { get; set; }

    public string UserId { get; set; }

    public int Point { get; set; } = 0;

    public string FullAddress { get; set; } = string.Empty;

    public DateTime DOB { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }
}