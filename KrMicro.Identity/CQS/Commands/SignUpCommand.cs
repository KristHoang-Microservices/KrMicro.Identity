using System.ComponentModel.DataAnnotations;

namespace KrMicro.Identity.CQS.Commands;

public record SignUpCommandRequest([Required] string UserName, [Required] string FullName,
    [Required] [EmailAddress] string Email, [Required] string Password, [Required] DateTime DOB, string? FullAddress,
    [Required] [Phone] string PhoneNumber);