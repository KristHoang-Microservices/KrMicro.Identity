namespace KrMicro.Identity.CQS.Commands;

public record UpdateCustomerCommandRequest(
    string? FullName, string? Email, string? PhoneNumber, DateTime? DOB, string? FullAddress
) : UpdateUserCommandRequest(FullName, Email, PhoneNumber);