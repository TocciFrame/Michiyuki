using System.ComponentModel.DataAnnotations;

namespace Michiyuki.Models;

/// <summary>Future-date rule: the departure must be at least N days from today.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class LeadTimeAttribute : ValidationAttribute
{
    private readonly int _days;
    public LeadTimeAttribute(int days) => _days = days;

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is not DateTime date) return ValidationResult.Success; // [Required] handles null
        return date.Date >= DateTime.Today.AddDays(_days)
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage ?? $"Pick a date at least {_days} days from today.");
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class MustBeTrueAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value is true;
}

/// <summary>Bound to the EditForm on the Plan page.</summary>
public class InquiryForm
{
    [Required(ErrorMessage = "Tell us who is leading the trip.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Name must be 2 to 60 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "We need an email to send the tickets to.")]
    [EmailAddress(ErrorMessage = "That email address doesn't look right.")]
    public string Email { get; set; } = string.Empty;

    [Range(1, TripPlanner.MaxTravelers, ErrorMessage = "Travelers must be between 1 and 12.")]
    public int Travelers { get; set; } = 2;

    [Required(ErrorMessage = "Choose a departure date.")]
    [LeadTime(14, ErrorMessage = "Departures need at least 14 days of lead time.")]
    public DateTime? Departure { get; set; }

    public Pace Pace { get; set; } = Pace.Steady;

    [StringLength(400, ErrorMessage = "Keep notes under 400 characters.")]
    public string Notes { get; set; } = string.Empty;

    [MustBeTrue(ErrorMessage = "Please accept the cancellation terms to continue.")]
    public bool AcceptTerms { get; set; }
}
