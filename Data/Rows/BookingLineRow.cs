using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Michiyuki.Data.Rows;

/// <summary>One row of public.booking_lines: the price snapshot of one stop in a booking.</summary>
[Table("booking_lines")]
public class BookingLineRow : BaseModel
{
    [PrimaryKey("id", false)] public long Id { get; set; }
    [Column("booking_id")] public long BookingId { get; set; }
    [Column("experience_id")] public long ExperienceId { get; set; }
    [Column("name")] public string Name { get; set; } = "";
    [Column("place")] public string Place { get; set; } = "";
    [Column("code")] public string Code { get; set; } = "";
    [Column("region")] public string Region { get; set; } = "";
    [Column("days")] public int Days { get; set; }
    [Column("cost")] public decimal Cost { get; set; }
}