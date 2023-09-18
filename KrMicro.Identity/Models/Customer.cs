using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KrMicro.Core.Models.Abstraction;

namespace KrMicro.Identity.Models;

[Table("Customers")]
public class Customer : BaseModel
{
    public string UserId { get; set; } = null!;
    public ApplicationUser UserInformation { get; set; } = null!;

    public int Point { get; set; } = 0;

    [Required] public string FullAddress { get; set; } = string.Empty;

    [Required] public DateTime DOB { get; set; }
}