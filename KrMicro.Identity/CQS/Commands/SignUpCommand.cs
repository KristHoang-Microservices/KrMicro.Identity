using System.ComponentModel.DataAnnotations;
using KrMicro.Core.CQS.Command.Abstraction;

namespace KrMicro.Identity.CQS.Commands;

public record SignUpCommandRequest([Required] string UserName, [Required] string FullName,
    [Required] [EmailAddress] string Email, [Required] string Password, [Required] DateTime DOB, [Required] string FullAddress, [Required][Phone] string PhoneNumber);
    