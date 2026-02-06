using Paynow.Sdk.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- KONFIGURACJA PAYNOW ---
builder.Services.AddPaynow(options =>
{
    options.ApiUrl = builder.Configuration["Paynow:ApiUrl"] ?? "https://api.sandbox.paynow.pl";
    options.ApiKey = builder.Configuration["Paynow:ApiKey"];
    options.SignatureKey = builder.Configuration["Paynow:SignatureKey"];
});
// ---------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();