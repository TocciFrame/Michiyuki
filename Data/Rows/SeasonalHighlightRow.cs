using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Michiyuki.Data.Rows;

/// <summary>One row of public.seasonal_highlights (many per destination).</summary>
[Table("seasonal_highlights")]
public class SeasonalHighlightRow : BaseModel
{
    [PrimaryKey("id", false)] public long Id { get; set; }
    [Column("destination_id")] public long DestinationId { get; set; }
    [Column("start_month")] public int StartMonth { get; set; }
    [Column("end_month")] public int EndMonth { get; set; }
    [Column("title")] public string Title { get; set; } = "";
    [Column("note")] public string Note { get; set; } = "";
}