namespace KrMicro.Core.CQS.Command.Abstraction;

public abstract class CreateCommandResult<TEntity>
{
    protected CreateCommandResult(TEntity? data, string? message, bool isSuccess = true)
    {
        Data = data;
        IsSuccess = isSuccess;
        Message = message;
    }

    public TEntity? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}