namespace KrMicro.Core.CQS.Command.Abstraction;

public abstract class UpdateCommandResult<TEntity>
{
    protected UpdateCommandResult(TEntity? data, string? message, bool isSuccess = true)
    {
        Data = data;
        IsSuccess = isSuccess;
        Message = message;
    }

    public TEntity? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}