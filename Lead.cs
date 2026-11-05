using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Lead
{
    public int LeadId { get; set; }

    public string LeadCode { get; set; } = "";

    [Required]
    public string LeadName { get; set; } = "";

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? Phone { get; set; }

    public string? CompanyName { get; set; }
    public string? Source { get; set; }

    public string Status { get; set; } = "New";

    [Range(0, 1000000000)]
    public decimal ExpectedValue { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? AssignedTo { get; set; }
}