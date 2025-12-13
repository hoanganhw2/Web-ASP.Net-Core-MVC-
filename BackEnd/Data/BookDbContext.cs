    using BackEnd.Models.Entity;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    namespace BackEnd.Data
    {
        public class BookDbContext : DbContext
        {
            public DbSet<User> Users{ get; set; }
            public DbSet<Role> Roles { get; set; }
            public DbSet<Category> Categories { get; set; }
            public DbSet<SubCategory> SubCategories { get; set; }
            public DbSet<Book> Books { get; set; }
            public DbSet<Image> Images { get; set; }
            public DbSet<Order> Orders { get; set; }
            public DbSet<OrderItem> OrderItems { get; set; }
            public DbSet<DiscountCode> DiscountCodes { get; set; }
            
            public BookDbContext(DbContextOptions<BookDbContext> options)
            : base(options)
            {
               
            }
        

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Cấu hình mối quan hệ 1-n
                modelBuilder.Entity<User>().HasOne(u => u.role).WithMany(r => r.users).HasForeignKey(u => u.role_id);

                //see data cho role 
                modelBuilder.Entity<Role>().HasData(new Role
                {
                    id = 1,
                    name = "ADMIN",
                    description = "Toàn quyền"
                }, new Role
                {
                    id = 2,
                    name = "USER",
                    description="Mua hàng,quản lý giỏ hàng"
                }
                );
            modelBuilder.Entity<SubCategory>().HasOne(sc=>sc.Category).WithMany(c=>c.subCategories).HasForeignKey(c=>c.CategoryId);
            modelBuilder.Entity<SubCategory>().HasMany(b => b.Books).WithOne(b => b.subCategory).HasForeignKey(b => b.subCategoryId);
            modelBuilder.Entity<Image>().HasOne(img => img.Book).WithMany(b => b.Images).HasForeignKey(img => img.product_id);

            // Order - User (n-1)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);

            // Order - DiscountCode (n-1, optional)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.DiscountCode)
                .WithMany(d => d.Orders)
                .HasForeignKey(o => o.DiscountCodeId)
                .IsRequired(false);

            // OrderItem - Order (n-1)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            // OrderItem - Book (n-1)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Book)
                .WithMany()
                .HasForeignKey(oi => oi.BookId);

            // DiscountCode - Code unique
            modelBuilder.Entity<DiscountCode>()
                .HasIndex(d => d.Code)
                .IsUnique();

            // Seed data cho DiscountCode
            modelBuilder.Entity<DiscountCode>().HasData(
                new DiscountCode
                {
                    Id = 1,
                    Code = "WELCOME10",
                    Description = "Giảm 10% cho khách hàng mới",
                    DiscountType = 0, // Theo %
                    DiscountValue = 10,
                    MinOrderAmount = 100000,
                    MaxDiscountAmount = 50000,
                    UsageLimit = 1000,
                    UsedCount = 0,
                    StartDate = new DateTime(2024, 1, 1),
                    EndDate = new DateTime(2025, 12, 31),
                    IsActive = true
                },
                new DiscountCode
                {
                    Id = 2,
                    Code = "SALE20",
                    Description = "Giảm 20% đơn hàng từ 200k",
                    DiscountType = 0, // Theo %
                    DiscountValue = 20,
                    MinOrderAmount = 200000,
                    MaxDiscountAmount = 100000,
                    UsageLimit = 500,
                    UsedCount = 0,
                    StartDate = new DateTime(2024, 1, 1),
                    EndDate = new DateTime(2025, 12, 31),
                    IsActive = true
                },
                new DiscountCode
                {
                    Id = 3,
                    Code = "FREESHIP",
                    Description = "Miễn phí vận chuyển (giảm 30,000đ)",
                    DiscountType = 1, // Số tiền cố định
                    DiscountValue = 30000,
                    MinOrderAmount = 150000,
                    MaxDiscountAmount = 30000,
                    UsageLimit = 2000,
                    UsedCount = 0,
                    StartDate = new DateTime(2024, 1, 1),
                    EndDate = new DateTime(2025, 12, 31),
                    IsActive = true
                },
                new DiscountCode
                {
                    Id = 4,
                    Code = "BOOK50K",
                    Description = "Giảm 50,000đ cho đơn từ 300k",
                    DiscountType = 1, // Số tiền cố định
                    DiscountValue = 50000,
                    MinOrderAmount = 300000,
                    MaxDiscountAmount = 50000,
                    UsageLimit = 300,
                    UsedCount = 0,
                    StartDate = new DateTime(2024, 1, 1),
                    EndDate = new DateTime(2025, 12, 31),
                    IsActive = true
                },
                new DiscountCode
                {
                    Id = 5,
                    Code = "VIP30",
                    Description = "Giảm 30% dành cho VIP",
                    DiscountType = 0, // Theo %
                    DiscountValue = 30,
                    MinOrderAmount = 500000,
                    MaxDiscountAmount = 200000,
                    UsageLimit = 100,
                    UsedCount = 0,
                    StartDate = new DateTime(2024, 1, 1),
                    EndDate = new DateTime(2025, 12, 31),
                    IsActive = true
                }
            );

        }
       
        }
    }
