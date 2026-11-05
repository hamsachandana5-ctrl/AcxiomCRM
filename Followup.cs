using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class FollowUp
{
    public int FollowUpId { get; set; }

    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }

    [Required]
    public DateTime FollowUpDate { get; set; }

    public string FollowUpType { get; set; } = "Call";

    public string? Remarks { get; set; }

    public string Status { get; set; } = "Planned";

    public string? AssignedTo { get; set; }
}