namespace KrMicro.Identity.CQS.Commands;

public record UpdateUserCommandRequest(
    string? FullName, string? Email, string? PhoneNumber
);