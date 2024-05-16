using Baetoti.Core.Interface.Base;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Core.Interface.Services;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using System.Net.Http;

namespace Baetoti.IoC
{
    public class DependencyContainer
    {
        public static void RegisterBaetotiApiServices(IServiceCollection services)
        {

            #region Singleton
            services.AddSingleton<IEncryptionService, EncryptionService>();
            services.AddSingleton<ISiteConfigService, SiteConfigService>();
            services.TryAdd(ServiceDescriptor.Singleton(typeof(IAppLogger<>), typeof(AppLoggerService<>)));
            #endregion

            #region Scoped
            services.AddScoped<HttpClient>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserJwtService, UserJwtService>();
            services.AddScoped<IArgon2Service, HashingService>();
            services.AddScoped<IRijndaelEncryptionService, RijndaelEncryptionService>();
            services.AddScoped<ISMSService, SMSService>();
            services.AddScoped<IGovtAPIService, GovtAPIService>();
            services.AddScoped<IChatAPIService, ChatAPIService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPaymentAPIService, PaymentAPIService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<IInstagramAPIService, InstagramAPIService>();
            services.AddScoped<IEmailService, EmailService>();
            #endregion

        }

        public static void RegisterBaetotiApiRepositories(IServiceCollection services)
        {
            ServiceProvider provider = services.BuildServiceProvider();
            var loggingDbContext = provider.GetRequiredService<LoggingDbContext>();

            #region Singleton
            services.AddSingleton<IExceptionRepository>(x => new ExceptionRepository(loggingDbContext));
            #endregion

            #region Scoped
            services.AddScoped<IDapper, DapperRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeLoginHistoryRepository, EmployeeLoginHistoryRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRolePrivilegeRepository, RolePrivilegeRepository>();
            services.AddScoped<IEmployeeRoleRepository, EmployeeRoleRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<ITagsRepository, TagsRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDesignationRepository, DesignationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IItemTagRepository, ItemTagRepository>();
            services.AddScoped<IOTPRepository, OTPRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<IItemReviewRepository, ItemReviewRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<ISubMenuRepository, SubMenuRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IDriverOrderRepository, DriverOrderRepository>();
            services.AddScoped<IProviderOrderRepository, ProviderOrderRepository>();
            services.AddScoped<IChangeItemRepository, ChangeItemRepository>();
            services.AddScoped<IChangeItemTagRepository, ChangeItemTagRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<IStoreTagRepository, StoreTagRepository>();
            services.AddScoped<IStoreScheduleRepository, StoreScheduleRepository>();
            services.AddScoped<IDriverConfigRepository, DriverConfigRepository>();
            services.AddScoped<IOperationalConfigRepository, OperationalConfigRepository>();
            services.AddScoped<IFenceRepository, FenceRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepositoy>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();
            services.AddScoped<IUserPromotionRepository, UserPromotionRepository>();
            services.AddScoped<IUserLocationRepository, UserLocationRepository>();
            services.AddScoped<IReportedStoreRepository, ReportedStoreRepository>();
            services.AddScoped<IStoreReviewRepository, StoreReviewRepository>();
            services.AddScoped<IFavouriteStoreRepository, FavouriteStoreRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICountryRepositroy, CountryRepository>();
            services.AddScoped<IItemImageRepository, ItemImageRepository>();
            services.AddScoped<IStoreImageRepository, StoreImageRepository>();
            services.AddScoped<IChangeItemImageRepository, ChangeItemImageRepository>();
            services.AddScoped<ISiteConfigRepository, SiteConfigRepository>();
            services.AddScoped<IDriverReviewRepository, DriverReviewRepository>();
            services.AddScoped<IBuyerReviewRepository, BuyerReviewRepository>();
            services.AddScoped<IFavouriteDriverRepository, FavouriteDriverRepository>();
            services.AddScoped<IAccountVisitRepository, AccountVisitRepository>();
            services.AddScoped<IOrderRequestRepository, OrderRequestRepository>();
            services.AddScoped<IReportedOrderRepository, ReportedOrderRepository>();
            services.AddScoped<ISupportRequestRepository, SupportRequestRepository>();
            services.AddScoped<INotificationTypeRepository, NotificationTypeRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IPushNotificationRepository, PushNotificationRepository>();
            services.AddScoped<IUserLoginHistoryRepository, UserLoginHistoryRepository>();
            services.AddScoped<IProviderBusinessRepository, ProviderBusinessRepository>();
            services.AddScoped<IInstragramTokenRepository, InstragramTokenRepository>();
            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<ICarTypeRepository, CarTypeRepository>();
            services.AddScoped<IItemVisitRepository, ItemVisitRepository>();
            services.AddScoped<IOTPAdminRepository, OTPAdminRepository>();
            services.AddScoped<IEmailConfigurationRepository, EmailConfigurationRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<ISMSTemplateRepository, SMSTemplateRepository>();
            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<IStoreAddressRepository, StoreAddressRepository>();

            #endregion

        }

    }
}
