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

    [Required, MaxLength(255)]
	[Column(TypeName = "varchar(255)")]
	public required string Login { get; set; }

	[Required, MaxLength(64)]
	[Column(TypeName = "varchar(64)")]
	public required string PassHash { get; set; }

	[Required, MaxLength(16)]
	[Column(TypeName = "varchar(16)")]
	public required string Salt { get; set; }

    [Required]
    [Column(TypeName = "integer")]
    public required EUserRole Role { get; set; }

    [Required]
    public Guid CompanyId { get; set; }

    public Company Company { get; set; } = null!;
}
