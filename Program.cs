using Michiyuki.Components;
using Michiyuki.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ---- Supabase ------------------------------------------------------------------------------
// Blazor Server runs this code on the server, so the key never ships to the browser.
// Put the Project URL and the *publishable* key in appsettings.json. Never the secret key.
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];
if (string.IsNullOrWhiteSpace(supabaseUrl) || string.IsNullOrWhiteSpace(supabaseKey)
    || supabaseUrl.Contains("YOUR-REF") || supabaseKey.Contains("YOUR-PUBLISHABLE-KEY"))
{
    throw new InvalidOperationException(
        "Supabase is not configured. Set Supabase:Url and Supabase:Key in appsettings.json (see README.md).");
}

var supabase = new Supabase.Client(supabaseUrl, supabaseKey,
    new Supabase.SupabaseOptions { AutoConnectRealtime = false });
await supabase.InitializeAsync();

builder.Services.AddSingleton(supabase);
builder.Services.AddSingleton<CatalogRepository>();
builder.Services.AddScoped<Michiyuki.Models.TripPlanner>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();