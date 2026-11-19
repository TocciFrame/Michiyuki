using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Michiyuki.Data.Rows;

/// <summary>One row of public.bookings. DeletedAt is the soft-delete marker.</summary>
[Table("bookings")]
public class BookingRow : BaseModel
{
    [PrimaryKey("id", false)] public long Id { get; set; }
    [Column("reference")] public string Reference { get; set; } = "";
    [Column("leader_name")] public string LeaderName { get; set; } = "";
    [Column("email")] public string Email { get; set; } = "";
    [Column("travelers")] public int Travelers { get; set; }
    [Column("departure")] public DateTime Departure { get; set; }
    [Column("pace")] public string Pace { get; set; } = "";
    [Column("notes")] public string Notes { get; set; } = "";
    [Column("deleted_at")] public DateTime? DeletedAt { get; set; }
    // Ignored when null, so an insert lets the database fill in now().
    [Column("created_at", NullValueHandling.Ignore)] public DateTime? CreatedAt { get; set; }
}