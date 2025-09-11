using Inspekta.Persistance.Abstractions;
using Inspekta.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspekta.Persistance.Entities;

public class User : DbEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [MaxLength(255)]
	[Column(TypeName = "varchar(255)")]
	[Required]
	public required string Login { get; set; }

	[MaxLength(64)]
	[Column(TypeName = "varchar(64)")]
	[Required]
	public required string PassHash { get; set; }

	[MaxLength(16)]
	[Column(TypeName = "varchar(16)")]
	[Required]
	public required string Salt { get; set; }

	[Column(TypeName = "integer")]
	[Required]
    public required EUserRole Role { get; set; }
}
