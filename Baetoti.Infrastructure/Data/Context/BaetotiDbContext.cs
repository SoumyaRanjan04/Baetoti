using Baetoti.Core.Entites;
using Microsoft.EntityFrameworkCore;
using System;

namespace Baetoti.Infrastructure.Data.Context
{
    public class BaetotiDbContext : DbContext
    {
        public BaetotiDbContext(DbContextOptions<BaetotiDbContext> options)
            : base(options)
        {
        }

        #region DbSets

        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeeLoginHistory> EmployeesLoginHistory { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<EmployeeRole> EmployeeRoles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<SubMenu> SubMenus { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<RolePrivilege> RolePrivileges { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemReview> ItemReviews { get; set; }
        public DbSet<ItemTag> ItemTags { get; set; }
        public DbSet<ChangeItem> ChangeItem { get; set; }
        public DbSet<ChangeItemTag> ChangeItemTag { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<OTPAdmin> OTPAdmins { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProviderOrder> ProviderOrders { get; set; }
        public DbSet<DriverOrder> DriverOrders { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreTag> StoreTags { get; set; }
        public DbSet<StoreSchedule> StoreSchedules { get; set; }
        public DbSet<DriverConfig> DriverConfigs { get; set; }
        public DbSet<OperationalConfig> OperationalConfigs { get; set; }
        public DbSet<Fence> Fences { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<UserPromotion> UserPromotions { get; set; }
        public DbSet<UserLocation> UserLocations { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<FavouriteStore> FavouriteStores { get; set; }
        public DbSet<StoreReview> StoreReviews { get; set; }
        public DbSet<ReportedStore> ReportedStores { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<ItemImage> ItemImages { get; set; }
        public DbSet<ChangeItemImage> ChangeItemImages { get; set; }
        public DbSet<StoreImage> StoreImages { get; set; }
        public DbSet<SiteConfig> SiteConfigs { get; set; }
        public DbSet<BuyerReview> BuyerReviews { get; set; }
        public DbSet<DriverReview> DriverReviews { get; set; }
        public DbSet<FavouriteDriver> FavouriteDrivers { get; set; }
        public DbSet<AccountVisit> AccountVisits { get; set; }
        public DbSet<OrderRequest> OrderRequests { get; set; }
        public DbSet<ReportedOrder> ReportedOrders { get; set; }
        public DbSet<SupportRequest> SupportRequests { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<PushNotification> PushNotifications { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserLoginHistory> UserLoginHistory { get; set; }
        public DbSet<ProviderBusiness> ProviderBusiness { get; set; }
        public DbSet<InstragramToken> InstragramTokens { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<CarType> CarTypes { get; set; }
        public DbSet<ItemVisit> ItemVisits { get; set; }
        public DbSet<EmailConfiguration> EmailConfigurations { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<StoreAddress> StoreAddress { get; set; }
        public DbSet<SMSTemplate> SMSTemplates { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer(@"");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ConfigureEntities(builder);

            builder.Entity<Privilege>().HasData(
                new { ID = Convert.ToInt64(1), Name = "View" },
                new { ID = Convert.ToInt64(2), Name = "Add" },
                new { ID = Convert.ToInt64(3), Name = "Edit" },
                new { ID = Convert.ToInt64(4), Name = "Delete" }
                );

            builder.Entity<Menu>().HasData(
                new { ID = Convert.ToInt64(1), Name = "Dashboard" },
                new { ID = Convert.ToInt64(2), Name = "Staff" },
                new { ID = Convert.ToInt64(3), Name = "Categories" },
                new { ID = Convert.ToInt64(4), Name = "Users" },
                new { ID = Convert.ToInt64(5), Name = "Items" },
                new { ID = Convert.ToInt64(6), Name = "Orders" },
                new { ID = Convert.ToInt64(7), Name = "Transactions" },
                new { ID = Convert.ToInt64(8), Name = "Analytics" },
                new { ID = Convert.ToInt64(9), Name = "Notifications" },
                new { ID = Convert.ToInt64(10), Name = "Configs" },
                new { ID = Convert.ToInt64(11), Name = "Feature Ads" },
                new { ID = Convert.ToInt64(12), Name = "Invoice" },
                new { ID = Convert.ToInt64(13), Name = "Project Management" },
                new { ID = Convert.ToInt64(14), Name = "Complaints" },
                new { ID = Convert.ToInt64(15), Name = "Profile" },
                new { ID = Convert.ToInt64(16), Name = "User Profile" }
                );

            builder.Entity<SubMenu>().HasData(
                new { ID = Convert.ToInt64(1), MenuID = Convert.ToInt64(1), Name = "Primary" },
                new { ID = Convert.ToInt64(2), MenuID = Convert.ToInt64(1), Name = "Secondary" },
                new { ID = Convert.ToInt64(3), MenuID = Convert.ToInt64(2), Name = "Employees" },
                new { ID = Convert.ToInt64(4), MenuID = Convert.ToInt64(2), Name = "Role & Privileges" },
                new { ID = Convert.ToInt64(5), MenuID = Convert.ToInt64(2), Name = "Last Login" },
                new { ID = Convert.ToInt64(6), MenuID = Convert.ToInt64(3), Name = "Category" },
                new { ID = Convert.ToInt64(7), MenuID = Convert.ToInt64(3), Name = "Units" },
                new { ID = Convert.ToInt64(8), MenuID = Convert.ToInt64(3), Name = "Tags" },
                new { ID = Convert.ToInt64(9), MenuID = Convert.ToInt64(4), Name = "User List" },
                new { ID = Convert.ToInt64(10), MenuID = Convert.ToInt64(4), Name = "Join Req." },
                new { ID = Convert.ToInt64(11), MenuID = Convert.ToInt64(5), Name = "Items List" },
                new { ID = Convert.ToInt64(12), MenuID = Convert.ToInt64(5), Name = "Change Item Req." },
                new { ID = Convert.ToInt64(13), MenuID = Convert.ToInt64(8), Name = "Map" },
                new { ID = Convert.ToInt64(14), MenuID = Convert.ToInt64(8), Name = "Statics" },
                new { ID = Convert.ToInt64(15), MenuID = Convert.ToInt64(8), Name = "Cohort Analysis" },
                new { ID = Convert.ToInt64(16), MenuID = Convert.ToInt64(8), Name = "Revenue" },
                new { ID = Convert.ToInt64(17), MenuID = Convert.ToInt64(8), Name = "Finance" },
                new { ID = Convert.ToInt64(18), MenuID = Convert.ToInt64(9), Name = "Notification" },
                new { ID = Convert.ToInt64(19), MenuID = Convert.ToInt64(9), Name = "Push Notification" },
                new { ID = Convert.ToInt64(20), MenuID = Convert.ToInt64(10), Name = "Commission" },
                new { ID = Convert.ToInt64(21), MenuID = Convert.ToInt64(10), Name = "VAT" },
                new { ID = Convert.ToInt64(22), MenuID = Convert.ToInt64(10), Name = "Driver Config" },
                new { ID = Convert.ToInt64(23), MenuID = Convert.ToInt64(10), Name = "Conutry Config" },
                new { ID = Convert.ToInt64(24), MenuID = Convert.ToInt64(10), Name = "Currency Config" }
                );

        }

        private void ConfigureEntities(ModelBuilder builder)
        {
            //builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        }

    }
}
