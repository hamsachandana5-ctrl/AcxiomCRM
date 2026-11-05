using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Activity
{
    public int ActivityId { get; set; }

    [Required]
    public string ActivityType { get; set; } = "Task";

    [Required]
    public string Subject { get; set; } = "";

    public string? Description { get; set; }

    public DateTime ActivityDate { get; set; }

    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }

    public string? AssignedTo { get; set; }

    public string Status { get; set; } = "Planned";
}