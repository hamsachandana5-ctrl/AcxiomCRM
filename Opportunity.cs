using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Opportunity
{
    public int OpportunityId { get; set; }

    [Required]
    public string OpportunityName { get; set; } = "";

    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }

    [Range(0.01, 1000000000)]
    public decimal Amount { get; set; }

    public string Stage { get; set; } = "Qualification";

    [Range(0, 100)]
    public int Probability { get; set; }

    public DateTime ExpectedCloseDate { get; set; }

    public string Status { get; set; } = "Open";

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? AssignedTo { get; set; }

    public decimal WeightedValue => Amount * Probability / 100;
}