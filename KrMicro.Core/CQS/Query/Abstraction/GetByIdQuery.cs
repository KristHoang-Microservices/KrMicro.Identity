namespace KrMicro.Core.CQS.Query.Abstraction;

public class GetByIdQueryResult<TEntity>
{
    protected GetByIdQueryResult(TEntity? data, bool isSuccess)
    {
        Data = data;
        IsSuccess = isSuccess;
    }

    public TEntity? Data { get; set; }
    public bool IsSuccess { get; set; }
}