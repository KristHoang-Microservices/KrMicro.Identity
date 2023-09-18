using System.ComponentModel.DataAnnotations;

namespace KrMicro.Identity.CQS.Commands;

public sealed record LoginCommandRequest([Required] string Username, [Required] string Password);