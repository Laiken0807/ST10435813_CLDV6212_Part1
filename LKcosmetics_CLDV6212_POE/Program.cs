using LKcosmetics_CLDV6212_POE.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

// TableStorageService
builder.Services.AddSingleton(new TableStorageService(
    configuration.GetConnectionString("AzureStorage")
));

// BlobStorageService
builder.Services.AddSingleton(new BlobStorageService(
    configuration.GetConnectionString("AzureStorage"),
    "lkcosmetics"   
));
// QueueService
builder.Services.AddSingleton(new QueueService(
    configuration.GetConnectionString("AzureStorage"),
    "orders-queue"
));
// FileService
builder.Services.AddSingleton(new FileService(
    configuration.GetConnectionString("AzureStorage"),
    "lkcosmeticsfiles"

    ));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
