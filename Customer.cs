using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Customer
{
    public int CustomerId { get; set; }

    public string CustomerCode { get; set; } = "";

    [Required, StringLength(100)]
    public string CustomerName { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, Phone]
    public string Phone { get; set; } = "";

    public string? CompanyName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
}