using BackEnd.Data;
using BackEnd.Models.Entity;
using BackEnd.Repository;
using BackEnd.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("BookStoreConnection");
builder.Services.AddDbContext<BookDbContext>(options=>options.UseSqlServer(connectionString));
builder.Services.AddScoped<IPasswordHasher<User>,PasswordHasher<User>>();

// DI
    //User
    builder.Services.AddScoped<IUserRepository,UserRepository>();
    builder.Services.AddScoped<IUserService,UserService>();
    //Catgegory
    builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
    builder.Services.AddScoped<ICategoryService,CategoryService>();
    //Book
    builder.Services.AddScoped<IBookReopository,BookRepository>();
    builder.Services.AddScoped<IBookService,BookService>();
    //Cart
    builder.Services.AddScoped<ICartService,CartService>();
    //Wishlist
    builder.Services.AddScoped<IWishlistService,WishlistService>();

// config sestion 
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options => {
    options.IdleTimeout= TimeSpan.FromDays(1);
    
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseSession();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
 
app.Run();
