using Inspekta.Persistance.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspekta.Persistance.Entities;

public class Company : DbEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required, MaxLength(200)]
    public required string Name { get; set; }

    [StringLength(10, MinimumLength = 10)]
    [RegularExpression(@"^\d{10}$")]
    [Column(TypeName = "char(10)")]
    public string? NIP { get; set; }

    [MaxLength(200)]
    public string? Street { get; set; }

    [StringLength(6, MinimumLength = 6)]
    [RegularExpression(@"^\d{2}-\d{3}$")]
    [Column(TypeName = "char(6)")]
    public string? ZipCode { get; set; }

    [MaxLength(128)]
    public string? Town { get; set; }

    [MaxLength(254)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(20)]
    [RegularExpression(@"^\+?\d{6,15}$")]
    public string? Phone { get; set; }

    public ICollection<User>? Users { get; set; }
}
