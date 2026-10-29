namespace Michiyuki.Models;

/// <summary>A station on the Michiyuki diagram. Immutable once built.</summary>
public sealed class Destination
{
    private readonly List<SeasonalHighlight> _highlights = new();

    public Destination(int id, string name, string jpName, Region region, string prefecture,
        string blurb, int mapX, int mapY, IEnumerable<SeasonalHighlight> highlights, bool labelLeft = false,
        string photoUrl = "", string photoAlt = "")
    {
        LabelLeft = labelLeft;
        Id = id;
        Name = name;
        JpName = jpName;
        Region = region;
        Prefecture = prefecture;
        Blurb = blurb;
        MapX = mapX;
        MapY = mapY;
        PhotoUrl = photoUrl;
        PhotoAlt = photoAlt;
        _highlights.AddRange(highlights);
    }

    public int Id { get; }
    public string Name { get; }
    public string JpName { get; }
    public Region Region { get; }
    public string Prefecture { get; }
    public string Blurb { get; }
    /// <summary>Destination photography used by the editorial location cards.</summary>
    public string PhotoUrl { get; }
    public string PhotoAlt { get; }
    /// <summary>Position on the 0-100 diagram grid.</summary>
    public int MapX { get; }
    public int MapY { get; }
    /// <summary>Which side of the station dot the map label sits on.</summary>
    public bool LabelLeft { get; }
    public IReadOnlyList<SeasonalHighlight> Highlights => _highlights;

    /// <summary>Station code such as K03.</summary>
    public string Code => $"{Region.Code()}{Id:00}";

    public bool IsInSeason(int month) => _highlights.Any(h => h.Covers(month));
    public IEnumerable<SeasonalHighlight> HighlightsIn(int month) => _highlights.Where(h => h.Covers(month));
}
