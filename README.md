# Michiyuki (Blazor .NET 8 + Tailwind v4)

Small-group travel across Japan, planned as an interactive rail line and editorial destination guide. The app uses Blazor Interactive Server and stores its data in a **Supabase (Postgres) database**; destination photography is loaded from public image hosts.

## Database setup (Supabase)
1. Supabase Dashboard -> **SQL Editor** -> run `database/schema.sql` (tables, keys, indexes, grants, row level security).
2. Run `database/seed.sql` (10 destinations, 22 seasonal highlights, 20 experiences).
3. Optional: `test.http` calls the REST API directly. If it returns rows, the database side works.
4. Copy **Project URL** (Connect button) and the **Publishable** key (Settings -> API Keys) into `appsettings.json`. Never the secret key, never the database password.
5. A free project pauses after a week without use. Restore it from the dashboard before submitting.

### Tables (1 per model)
| Model | Table | Relationship |
|---|---|---|
| `Destination` | `destinations` | parent |
| `SeasonalHighlight` | `seasonal_highlights` | `destination_id` -> `destinations.id` |
| `Experience` (+ `GuidedTour`, `RailJourney`, `HomeStay`, `CraftWorkshop`) | `experiences` | `destination_id` -> `destinations.id`; the `kind` column picks the subclass |
| `Booking` | `bookings` | parent of `booking_lines` |
| `BookingLine` | `booking_lines` | `booking_id` -> `bookings.id`, `experience_id` -> `experiences.id` |

Enums are check constraints. `InquiryForm` is only a form, so it has no table (its notes are saved on `bookings`).

### What reads and writes the database
| Action | Where in the UI | Supabase call |
|---|---|---|
| Read | Home, Explore, Seasons, Plan | `Get` on destinations, seasonal_highlights, experiences, bookings, booking_lines (filtered and limited in the database, no query inside a loop) |
| Create | Plan -> Confirm and get ticket | `Update` seats on experiences, `Insert` into bookings, `Insert` into booking_lines |
| Update | Plan -> Recent bookings -> Reschedule | `Update bookings.departure` (seats on experiences also change on create and cancel) |
| Delete | Plan -> Cancel | **soft delete**: `Update bookings.deleted_at`, seats given back |

## Storage pattern: soft delete on `bookings`, history in `booking_lines`
Cancelling a booking only fills `bookings.deleted_at`; the row is not removed, and every list query uses `Where(DeletedAt == null)`. A booking is a record of money and seats, so deleting it for good would erase proof of what was promised and make mistakes impossible to undo. `booking_lines` is a history table: it copies each stop's name and price at the moment of booking, so changing an experience price later never rewrites an old trip. Catalog tables (`destinations`, `experiences`) use plain overwrite because only their current values matter.

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
- `Booking`: price snapshot of a confirmed trip (saved in `bookings` + `booking_lines`)
- `Forms/InquiryForm`: DataAnnotations plus custom `LeadTime` and `MustBeTrue` attributes

## Structure
Styles/app.css (tokens + components) | Models/ | Data/CatalogRepository.cs + Data/Rows/ (Supabase) | database/ (SQL) | Components/Layout | Components/Pages


## Destination photos
Destination photography is loaded from public image hosts so the project archive stays lightweight. An internet connection is required for the photos; the rest of the planner runs locally. The URLs and accessibility alt text live in the `destinations` table (`photo_url`, `photo_alt`). Review the source site's license and attribution requirements before publishing the project beyond coursework.

Photo source pages: [Hokkaido lavender](https://ppap.kinto-jp.com/articles/hokkaido-summer), [Sapporo](https://www.japan.travel/en/spot/473/), [Kakunodate](https://www.timeout.com/tokyo/travel/most-beautiful-traditional-towns-you-should-visit-in-japan), [Tokyo](https://livejapan.com/ja/in-tokyo/in-pref-tokyo/in-asakusa/article-a0004448/), [Kanazawa](https://www.audleytravel.com/us/japan/places-to-go/kanazawa), [Kyoto image page](https://saleemahmadsaleemi606.medium.com/top-10-beautiful-cities-in-the-world-63056fe45242), [Matsuyama](https://selfguidejapan.com/destinations/dogo-onsen), [Beppu](https://tw.japan-travel-note.com/posts/268), [Yakushima](https://www.huffingtonpost.jp/entry/story_jp_67b2bfffe4b0ad102039b9aa), and [Okinawa](https://es.japanspecialist.com/w/isla-principal-de-okinawa).