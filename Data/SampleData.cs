using Michiyuki.Models;

namespace Michiyuki.Data;

/// <summary>All content is built from the model classes. Destinations are shared; experiences are created per visitor so seat counts are theirs.</summary>
public static class SampleData
{
    private static SeasonalHighlight H(int s, int e, string title, string note) => new(s, e, title, note);

    public static readonly IReadOnlyList<Destination> Destinations = new List<Destination>
    {
        // Region-level Hokkaido entry sits before Sapporo, so both the station map and gallery
        // make the northern island discoverable as its own destination.
        new(1, "Hokkaido", "北海道", Region.Hokkaido, "Hokkaido",
            "Lavender fields, wide-open national parks and seafood straight from the northern sea.", 91, 4,
            new[] {
                H(7, 8, "Lavender country", "Furano's purple fields and clear summer skies peak in July."),
                H(2, 2, "Snow country", "Powder snow and winter festivals turn the island into a snowy playground.") }, labelLeft: true,
            photoUrl: "https://ppap.kinto-jp.com/cms-images/assets/da7c0038a35e4eca89b8e56f040e91df/a04ae63395ba483ca26ba3bab5c2bf4c/pixta_89494312_M.webp",
            photoAlt: "Rows of purple lavender stretching toward the mountains of Hokkaido"),
        new(2, "Sapporo", "札幌", Region.Hokkaido, "Hokkaido",
            "Wide grid streets, miso ramen and a winter that is part of the city's identity.", 75, 14,
            new[] {
                H(2, 2, "Snow Festival", "Ice and snow sculptures fill Odori Park for a week in early February."),
                H(7, 8, "Beer gardens in the park", "Long summer evenings with grilled lamb under the trees of Odori."),
                H(12, 12, "Illuminations", "The Odori and Ekimae streets are lit for the whole month.") }, labelLeft: true,
            photoUrl: "https://res-4.cloudinary.com/jnto/image/upload/w_750%2Ch_503%2Cc_fill%2Cf_auto%2Cfl_lossy%2Cq_auto/v1508549578/hokkaido/Hokkaido1397_3.jpg",
            photoAlt: "Sapporo's Odori Park lit up during the winter snow festival"),
        new(3, "Kakunodate", "角館", Region.Tohoku, "Akita",
            "A castle town where black-fenced samurai houses sit under old weeping cherries.", 62, 25,
            new[] {
                H(4, 5, "Weeping cherries", "Trees line the Hinokinai river bank and the samurai district."),
                H(10, 11, "Autumn colour", "Maples and ginkgo turn the samurai lanes red and gold.") }, labelLeft: true,
            photoUrl: "https://media.timeout.com/images/106164075/1536/864/image.jpg",
            photoAlt: "Weeping cherry blossoms arch over the traditional samurai district in Kakunodate"),
        new(4, "Tokyo", "東京", Region.Kanto, "Tokyo",
            "Thirty-seven million people and still a city of neighbourhoods you can walk in an afternoon.", 70, 40,
            new[] {
                H(3, 4, "Cherry blossom", "Chidorigafuchi moat and Ueno Park fill with picnic tarps."),
                H(11, 12, "Ginkgo avenues", "The gold tunnel at Jingu Gaien peaks in the last week of November.") },
            photoUrl: "https://rimage.gnst.jp/livejapan.com/public/article/detail/a/00/04/a0004448/img/ja/a0004448_parts_5ee1657d2b498.jpg?20210108093247=&q=80",
            photoAlt: "Traditional temple roofs and modern buildings across Tokyo's Asakusa district"),
        new(5, "Kanazawa", "金沢", Region.Chubu, "Ishikawa",
            "Preserved tea districts, a famous garden and the country's gold-leaf craft capital.", 44, 44,
            new[] {
                H(4, 4, "Kenrokuen in bloom", "Cherry trees open over the garden ponds."),
                H(11, 3, "Snow crab and yukitsuri", "Rope-hung pines in the garden, and crab on every menu.") }, labelLeft: true,
            photoUrl: "https://cdn.audleytravel.com/1050/749/79/16027038-spring-in-kanazawa.jpg",
            photoAlt: "A reflective pond, pines and stone lantern at Kenrokuen Garden in Kanazawa"),
        new(6, "Kyoto", "京都", Region.Kansai, "Kyoto",
            "Temples, lanes and working ateliers. Go early, go on foot, avoid the main street at noon.", 52, 60,
            new[] {
                H(3, 4, "Philosopher's Path", "Canal-side cherry trees, best before 8 a.m."),
                H(7, 7, "Gion Matsuri", "A month of festival, with the float processions in mid-July."),
                H(11, 12, "Maple season", "Tofuku-ji and the Higashiyama temples turn deep red.") },
            photoUrl: "https://miro.medium.com/v2/resize%3Afit%3A1280/1%2AuupDDwHy6e1xwnXqCcnmKg.jpeg",
            photoAlt: "Traditional Kyoto architecture among trees and temple rooftops"),
        new(7, "Matsuyama", "松山", Region.Shikoku, "Ehime",
            "A castle on a hill, a tram that circles the town and Japan's oldest hot spring bathhouse.", 32, 72,
            new[] {
                H(3, 4, "Castle in spring", "Cherry trees on the castle hill, a ropeway up and a walk down."),
                H(10, 10, "Autumn festivals", "Shrine floats and drums around Dogo Onsen.") },
            photoUrl: "https://images.selfguidejapan.com/location/ehime/Dogo2",
            photoAlt: "Dogo Onsen's historic wooden bathhouse glowing at night in Matsuyama"),
        new(8, "Beppu", "別府", Region.Kyushu, "Oita",
            "Steam rises from the streets. Hot springs here are a public utility.", 20, 83,
            new[] {
                H(10, 12, "Clear autumn air", "The best weeks for hopping between the hot-spring districts."),
                H(1, 2, "Winter steam", "Cold air makes every vent and bath more dramatic. Fewer crowds.") },
            photoUrl: "https://cdn.japan-travel-note.com/images/00268/05.jpg",
            photoAlt: "A turquoise geothermal hot spring sends clouds of steam into the air in Beppu"),
        new(9, "Yakushima", "屋久島", Region.Kyushu, "Kagoshima",
            "A round granite island with cedars older than most countries.", 34, 90,
            new[] {
                H(5, 6, "Moss at its greenest", "Rain-fed forests. Bring a proper shell."),
                H(10, 11, "Drier trail weeks", "Typically steadier weather for the Shiratani trail.") },
            photoUrl: "https://img.huffingtonpost.com/asset/67b2c80c19000025002aa827.jpeg?ops=scalefit_1440",
            photoAlt: "Ancient cedar trunks and moss carpeting Yakushima's rainforest floor"),
        new(10, "Okinawa", "沖縄", Region.Okinawa, "Okinawa",
            "Coral-blue water, Ryukyu heritage and island time at the southern edge of Japan.", 61, 98,
            new[] {
                H(4, 6, "Island sea season", "Clear water and warm days make spring an inviting time for coastal walks."),
                H(10, 11, "Gentler shoulder season", "Warm seas and a little more breathing room beyond the summer peak.") }, labelLeft: true,
            photoUrl: "https://es.japanspecialist.com/documents/439822/3836087/1_Okinawa%2BIsland.jpg/636d9d95-fc79-6ef1-ab30-7355b0afd15f?download=true&t=1756118468430&version=1.0",
            photoAlt: "Pale sand and clear turquoise water along Okinawa's main island")
    };

    public static Destination Dest(int id) => Destinations.First(d => d.Id == id);

    public static List<Experience> CreateExperiences() => new()
    {
        new GuidedTour(1, "Snow Lantern Night Walk", Dest(2), 1, 6800, 12, 7,
            "Two hours through Odori and the Susukino back streets as the lanterns are lit, ending with a bowl of miso ramen.", "Haruka Ono"),
        new CraftWorkshop(2, "Cheese and Butter by Hand", Dest(2), 1, 3500, 10, 4,
            "A dairy farm east of the city. You press a wheel of fresh cheese and churn your own butter.", 5000, "Dairy making"),

        new RailJourney(3, "The Akita Line to the Samurai Town", Dest(3), 2, 17800, 16, 5,
            "A reserved window seat on the Komachi service, then the local line into Kakunodate.", "Tohoku Rail Pass", 1500),
        new HomeStay(4, "Farmhouse Room above the Orchards", Dest(3), 2, 15000, 6, 1,
            "Tatami rooms in a working apple farm, ten minutes from the samurai district by bicycle.", 2000),

        new GuidedTour(5, "Back-Alley Eats, Asakusa to Ueno", Dest(4), 1, 9500, 10, 9,
            "Seven stops in four hours: croquettes, grilled skewers, tiny bars and a final bowl of soba.", "Daichi Mori"),
        new CraftWorkshop(6, "Knife Sharpening on Kitchen Street", Dest(4), 1, 5500, 6, 3,
            "A bladesmith in Kappabashi teaches whetstone basics on a knife you choose yourself.", 4000, "Knife sharpening"),

        new RailJourney(7, "Hokuriku Arch: Tokyo to Kanazawa", Dest(5), 2, 14500, 20, 11,
            "The Shinkansen along the Sea of Japan side, with a stop to see the Alps from the window.", "Hokuriku Arch Pass", 2000),
        new CraftWorkshop(8, "Gold-Leaf Tray Workshop", Dest(5), 1, 4200, 8, 5,
            "Apply leaf thinner than a hair to a lacquered tray in a family studio near Higashi Chaya.", 6000, "Gold-leaf lacquer"),

        new GuidedTour(9, "Higashiyama Dawn Pilgrimage", Dest(6), 1, 11000, 8, 3,
            "Leave at 5:30 a.m. to walk the temple slopes before the first tour buses arrive.", "Mio Tanabe"),
        new HomeStay(10, "Machiya Townhouse in Nishijin", Dest(6), 3, 24000, 6, 2,
            "A restored weaver's house with an inner courtyard and a wooden bath.", 2500),
        new CraftWorkshop(11, "Indigo Dyeing at a Kyoto Atelier", Dest(6), 1, 3800, 8, 8,
            "Fold, bind and dip a silk scarf in a working indigo vat. This one is fully booked right now.", 5000, "Indigo dyeing"),

        new RailJourney(12, "Seto Inland Sea Slow Train", Dest(7), 3, 22000, 14, 12,
            "Three days of local and limited express services along the water, with island ferries between.", "Setouchi Area Pass", 3000),
        new CraftWorkshop(13, "Iyo Kasuri Weaving, then Dogo Onsen", Dest(7), 1, 3200, 8, 2,
            "A loom lesson in the morning, an evening soak in the oldest bathhouse in the country.", 3500, "Weaving"),

        new HomeStay(14, "Ryokan with a Private Onsen", Dest(8), 2, 32000, 10, 4,
            "Kaiseki dinner served in your room, a cypress bath on the balcony.", 3000),
        new CraftWorkshop(15, "Jigoku-Mushi Steam Cooking", Dest(8), 1, 3000, 8, 3,
            "Cook vegetables and eggs in natural steam vents at Kannawa, guided by a local cook.", 3500, "Steam cooking"),

        new GuidedTour(16, "Moss Forest Trek", Dest(9), 2, 38000, 8, 6,
            "A full day on the Shiratani trail, then a night near the coast. Rain gear included.", "Kenta Shimabukuro"),
        new CraftWorkshop(17, "Cedar Spoon Carving", Dest(9), 1, 2800, 6, 1,
            "Carve a spoon from fallen Yakushima cedar with a local craftsman.", 3000, "Wood carving"),

        new GuidedTour(18, "Lavender Fields & Farm Kitchen", Dest(1), 1, 7600, 10, 2,
            "Walk the fragrant Furano fields with a local grower, then gather around a seasonal farm lunch with views of the Daisetsuzan range.", "Aiko Sato"),
        new GuidedTour(19, "Okinawa Coast & Ryukyu Stories", Dest(10), 1, 8900, 10, 3,
            "Follow a local guide along a turquoise-water coast, hear stories of the Ryukyu Kingdom, and finish with a small island-produce tasting.", "Chie Shimabukuro"),
        new CraftWorkshop(20, "Ryukyu Glass Studio", Dest(10), 1, 4200, 8, 2,
            "Shape recycled glass into a colourful keepsake with a glassworker whose studio carries on Okinawan craft traditions.", 4500, "Glass blowing")
    };
}
