namespace KrMicro.Core.CQS.Command.Abstraction;

public abstract class UpdateStatusCommandResult
{
    public UpdateStatusCommandResult(string message, bool isSuccess)
    {
        Message = message;
        IsSuccess = isSuccess;
    }

    public string Message { get; set; }
    public bool IsSuccess { get; set; }
}