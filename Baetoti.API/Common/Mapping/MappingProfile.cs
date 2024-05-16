using AutoMapper;
using Baetoti.Core.Entites;
using Baetoti.Shared.Request.Auth;
using Baetoti.Shared.Request.Category;
using Baetoti.Shared.Request.SubCategory;
using Baetoti.Shared.Request.TagRequest;
using Baetoti.Shared.Request.UnitRequest;
using Baetoti.Shared.Request.Employee;
using Baetoti.Shared.Response.Category;
using Baetoti.Shared.Response.Department;
using Baetoti.Shared.Response.Designation;
using Baetoti.Shared.Response.SubCategory;
using Baetoti.Shared.Response.TagResponse;
using Baetoti.Shared.Response.Unit;
using Baetoti.Shared.Response.Employee;
using System;
using System.Linq;
using System.Reflection;
using Baetoti.Shared.Request.EmployeeRole;
using Baetoti.Shared.Response.Role;
using Baetoti.Shared.Request.Provider;
using Baetoti.Shared.Request.Driver;
using Baetoti.Shared.Response.Provider;
using Baetoti.Shared.Response.Driver;
using Baetoti.Shared.Response.Store;
using Baetoti.Shared.Request.Store;
using Baetoti.Shared.Request.StoreSchedule;
using Baetoti.Shared.Response.StoreSchedule;
using Baetoti.Shared.Request.ChangeItem;
using Baetoti.Shared.Request.DriverConfig;
using Baetoti.Shared.Response.DriverConfig;
using Baetoti.Shared.Request.OperationalConfig;
using Baetoti.Shared.Response.OperationalConfig;
using Baetoti.Shared.Request.Promotion;
using Baetoti.Shared.Request.Country;
using Baetoti.Shared.Request.Cart;
using Baetoti.Shared.Request.Notification;
using Baetoti.Shared.Response.GovtAPI;

namespace Baetoti.API.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());

            CreateMap<AuthRequest, Employee>();
            CreateMap<CategoryRequest, Category>();
            CreateMap<Category, CategoryResponse>();
            CreateMap<SubCategoryRequest, SubCategory>();
            CreateMap<SubCategory, SubCategoryResponse>();
            CreateMap<TagRequest, Tags>();
            CreateMap<Tags, TagResponse>();
            CreateMap<UnitRequest, Unit>();
            CreateMap<Unit, UnitResponse>();
            CreateMap<EmployeeRequest, Employee>();
            CreateMap<Employee, EmployeeResponse>();
            CreateMap<AssignRoleRequest, EmployeeRole>();
            CreateMap<Department, DepartmentResponse>();
            CreateMap<Designation, DesignationResponse>();
            CreateMap<Roles, RoleResponse>();
            CreateMap<ProviderRequest, Provider>();
            CreateMap<Provider, ProviderResponse>();
            CreateMap<DriverRequest, Driver>();
            CreateMap<Driver, DriverResponse>();
            CreateMap<Menu, Shared.Response.Menu.MenuResponse>();
            CreateMap<SubMenu, Shared.Response.SubMenu.SubMenuResponse>();
            CreateMap<Store, StoreResponse>();
            CreateMap<StoreRequest, Store>();
            CreateMap<StoreScheduleRequest, StoreSchedule>();
            CreateMap<StoreSchedule, StoreScheduleResponse>();
            CreateMap<ChangeItemRequest, ChangeItem>();
            CreateMap<DriverConfig, DriverConfigResponse>();
            CreateMap<DriverConfigRequest, DriverConfig>();
            CreateMap<OperationalConfig, OperationalConfigResponse>();
            CreateMap<OperationalConfigRequest, OperationalConfig>();
            CreateMap<PromotionRequest, Promotion>();
            CreateMap<CountryRequest, Country>();
            CreateMap<CartRequest, Cart>();
            CreateMap<RatingAndReviews, DriverReview>();
            CreateMap<RatingAndReviews, BuyerReview>();
            CreateMap<PushNotificationRequest, PushNotification>();
            CreateMap<Region, SharedLookupResponse>();
            CreateMap<City, SharedLookupResponse>();
            CreateMap<CarType, SharedLookupResponse>();
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = type.GetMethod("Mapping")
                    ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");
                methodInfo?.Invoke(instance, new object[] { this });
            }
        }

    }
}
