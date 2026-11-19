using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Michiyuki.Data.Rows;

/// <summary>One row of public.destinations. Column names must match the table exactly.</summary>
[Table("destinations")]
public class DestinationRow : BaseModel
{
    [PrimaryKey("id", false)] public long Id { get; set; }
    [Column("name")] public string Name { get; set; } = "";
    [Column("jp_name")] public string JpName { get; set; } = "";
    [Column("region")] public string Region { get; set; } = "";
    [Column("prefecture")] public string Prefecture { get; set; } = "";
    [Column("blurb")] public string Blurb { get; set; } = "";
    [Column("map_x")] public int MapX { get; set; }
    [Column("map_y")] public int MapY { get; set; }
    [Column("label_left")] public bool LabelLeft { get; set; }
    [Column("photo_url")] public string PhotoUrl { get; set; } = "";
    [Column("photo_alt")] public string PhotoAlt { get; set; } = "";
}