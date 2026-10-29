# Michiyuki (Blazor .NET 8 + Tailwind v4)

Small-group travel across Japan, planned as an interactive rail line and editorial destination guide. The app uses Blazor Interactive Server and has no database; destination photography is loaded from public image hosts.

## Run
    npm install            # first time only
    npm run build:css
    dotnet run             # opens http://localhost:5029

`dotnet build` also runs `npm run build:css` automatically. A pre-built `wwwroot/css/app.css` is included.

## Pages
- `/`         Home: interactive line map, "filling up first" board, how it works
- `/explore`  Search, filter, sort, cost breakdowns, live route panel (add/remove, party size, total)
- `/seasons`  Month picker and timetable matrix of what is in season
- `/plan`     Validated booking form, loading state, ticket on success

## OOP in `Models/`
- `Experience` (abstract, implements `IBookable`): identity, seat tracking, total = sum of `Breakdown()`
  - `GuidedTour`, `RailJourney`, `HomeStay`, `CraftWorkshop` override `Kind`, `Notes`, `Breakdown()` (polymorphic pricing)
- `Destination`, `SeasonalHighlight`: immutable value-like classes with behaviour (`IsInSeason`, `Covers`)
- `TripPlanner`: scoped aggregate that encapsulates route rules (no duplicates, seat checks, totals, `Confirm`)
- `Booking`: price snapshot of a confirmed trip
- `Forms/InquiryForm`: DataAnnotations plus custom `LeadTime` and `MustBeTrue` attributes

## Structure
Styles/app.css (tokens + components) | Models/ | Data/SampleData.cs | Components/Layout | Components/Pages


## Destination photos
Destination photography is loaded from public image hosts so the project archive stays lightweight. An internet connection is required for the photos; the rest of the planner runs locally. The URLs and accessibility alt text live on each `Destination` in `Data/SampleData.cs`. Review the source site's license and attribution requirements before publishing the project beyond coursework.

Photo source pages: [Hokkaido lavender](https://ppap.kinto-jp.com/articles/hokkaido-summer), [Sapporo](https://www.japan.travel/en/spot/473/), [Kakunodate](https://www.timeout.com/tokyo/travel/most-beautiful-traditional-towns-you-should-visit-in-japan), [Tokyo](https://livejapan.com/ja/in-tokyo/in-pref-tokyo/in-asakusa/article-a0004448/), [Kanazawa](https://www.audleytravel.com/us/japan/places-to-go/kanazawa), [Kyoto image page](https://saleemahmadsaleemi606.medium.com/top-10-beautiful-cities-in-the-world-63056fe45242), [Matsuyama](https://selfguidejapan.com/destinations/dogo-onsen), [Beppu](https://tw.japan-travel-note.com/posts/268), [Yakushima](https://www.huffingtonpost.jp/entry/story_jp_67b2bfffe4b0ad102039b9aa), and [Okinawa](https://es.japanspecialist.com/w/isla-principal-de-okinawa).
