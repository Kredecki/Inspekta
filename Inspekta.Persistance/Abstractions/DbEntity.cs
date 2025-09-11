using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspekta.Persistance.Abstractions;

public abstract class DbEntity
{
	[Required]
	public required Guid CreatedBy { get; set; }

	[Required]
	public required DateTime CreatedAt { get; set; }

	[Required]
	public required Guid ModifiedBy { get; set; }

	[Required]
	public required DateTime ModifiedAt { get; set; }
}
