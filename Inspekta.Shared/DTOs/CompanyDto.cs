namespace Inspekta.Shared.DTOs;

public sealed class CompanyDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? NIP { get; set; }
    public string? Street { get; set; }
    public string? ZipCode { get; set; }
    public string? Town { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
