using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Michiyuki.Data.Rows;

/// <summary>
/// One row of public.experiences. GuidedTour, RailJourney, HomeStay and CraftWorkshop share this
/// table; "kind" says which one, and the kind-specific columns are null for the others.
/// </summary>
[Table("experiences")]
public class ExperienceRow : BaseModel
{
    [PrimaryKey("id", false)] public long Id { get; set; }
    [Column("destination_id")] public long DestinationId { get; set; }
    [Column("kind")] public string Kind { get; set; } = "";
    [Column("name")] public string Name { get; set; } = "";
    [Column("description")] public string Description { get; set; } = "";
    [Column("days")] public int Days { get; set; }
    [Column("base_price")] public decimal BasePrice { get; set; }
    [Column("capacity")] public int Capacity { get; set; }
    [Column("booked")] public int Booked { get; set; }
    [Column("guide")] public string? Guide { get; set; }
    [Column("pass_name")] public string? PassName { get; set; }
    [Column("reservation_fee")] public decimal? ReservationFee { get; set; }
    [Column("cleaning_fee")] public decimal? CleaningFee { get; set; }
    [Column("studio_fee")] public decimal? StudioFee { get; set; }
    [Column("craft")] public string? Craft { get; set; }
}