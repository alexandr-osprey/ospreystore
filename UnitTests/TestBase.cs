﻿using ApplicationCore.Entities;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Controllers;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Data.SampleData;
using Infrastructure.Identity;
using Infrastructure.Identity.AuthorizationParameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using OspreyStore;
using OspreyStore.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public abstract class TestBase<TEntity> where TEntity : Entity
    {
        public static JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public static IConfiguration AppSetting { get; } = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

        protected static ServiceCollection Services { get; } = new ServiceCollection();

        protected static void OverrideDefaultDI()
        {
            Services.AddSingleton(typeof(IAuthorizationParameters<>), typeof(AuthoriationParametersWithoutAuthorization<>));
            Services.AddScoped<IAuthorizationParameters<CartItem>, AuthoriationParametersWithoutAuthorization<CartItem>>();
            Services.AddScoped<IAuthorizationParameters<Order>, AuthoriationParametersWithoutAuthorization<Order>>();
        }


        protected ServiceProvider _ServiceProvider { get; }

        static TestBase()
        {
            ConfigureDI();
            ConfigureIdentity();
            OverrideDefaultDI();
        }

        protected static DbContextOptions<StoreContext> StoreContextOptions { get; } =
            new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        protected static DbContextOptions<AppIdentityDbContext> IdentityContextOptions { get; } =
            new DbContextOptionsBuilder<AppIdentityDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        protected static void ConfigureIdentity()
        {
            Services.AddSingleton(IdentityContextOptions);
            //IdentityConfiguration.ConfigureTesting(services, configuration);

            //Services.AddSingleton(_ => new TokenValidationParameters());
            //Services.AddDbContext<AppIdentityDbContext>();
            //Services.AddIdentity<ApplicationUser, IdentityRole>()
            //  .AddEntityFrameworkStores<AppIdentityDbContext>()
            //  .AddDefaultTokenProviders();
            // Services.AddScoped<IIdentityService, IdentityService>();
            Services.AddScoped<UserManager<ApplicationUser>, UserManager<ApplicationUser>>();
            Services.AddScoped<SignInManager<ApplicationUser>, SignInManager<ApplicationUser>>();
            Services.AddScoped<IAuthorizationService, DefaultAuthorizationService>();
            Services.AddScoped<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>();
            Services.AddScoped<IAuthorizationHandlerProvider, DefaultAuthorizationHandlerProvider>();
            Services.AddScoped<IAuthorizationHandlerContextFactory, DefaultAuthorizationHandlerContextFactory>();
            Services.AddScoped<IAuthorizationEvaluator, DefaultAuthorizationEvaluator>();
            Services.AddSingleton<IScopedParameters>(_scopedParameters);
            IdentityConfiguration.ConfigureServices(Services, AppSetting);

            //await signInManager.SignInAsync(
            //    await Resolve<UserManager<ApplicationUser>>().FindByIdAsync(adminId), false);
        }

        protected static void ConfigureDI()
        {
            // db options
            Services.AddSingleton(StoreContextOptions);
            //services.AddDbContext<StoreContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            Startup.ConfigureDI(Services);

            //Services.AddScoped<IAuthorizationParameters<CartItem>, AuthoriationParametersWithoutAuthorization<CartItem>>();

            var builder = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json");
            var conf = builder.Build();

            Services.AddSingleton<IConfiguration>(conf);
            Services.AddScoped<IBrandsController, BrandsController>();
            Services.AddScoped<ICartItemsController, CartItemsController>();
            Services.AddScoped<IdentityController>();
            Services.AddScoped<IItemsController, ItemsController>();
            Services.AddScoped<IItemVariantImagesController, ItemVariantImagesController>();
            Services.AddScoped<IItemVariantsController, ItemVariantsController>();
            Services.AddScoped<IItemPropertiesController, ItemPropertiesController>();
            Services.AddScoped<IStoresController, StoresController>();
            Services.AddScoped<ICategoriesController, CategoriesController>();
            Services.AddScoped<ICharacteristicsController, CharacteristicsController>();
            Services.AddScoped<ICharacteristicValuesController, CharacteristicValuesController>();
            Services.AddScoped<IOrdersController, OrdersController>();
            //Services.AddScoped<IOrderItemsController, OrderItemsController>();

            //ConfigureIdentity();
        }

        protected IAppLogger<TestBase<TEntity>> _logger { get; }
        protected UserManager<ApplicationUser> _userManager { get; }
        protected ITestOutputHelper _output { get; }
        protected int _maxTake { get; } = 200;
        protected StoreContext _context { get; }
        protected AppIdentityDbContext _identityContext { get; }
        protected TestSampleData _data { get; }
        protected static IScopedParameters _scopedParameters { get; } = new ScopedParameters()
        {
            ClaimsPrincipal = Users.JohnPrincipal
        };

        public TestBase(ITestOutputHelper output)
        {
            _ServiceProvider = Services.BuildServiceProvider();
            SeedIdentity();
            _context = Resolve<StoreContext>();
            _data = new TestSampleData(Resolve<StoreContext>(), Resolve<IConfiguration>());
            NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("NLog.config", false);
            _output = output;
        }

        protected void SeedIdentity()
        {
            AppIdentityDbContextSeed.SeedAsync(_ServiceProvider, Resolve<UserManager<ApplicationUser>>()).Wait();
            AppIdentityDbContextSeed.AddClaim(Resolve<UserManager<ApplicationUser>>(),
                new Claim(CustomClaimTypes.Administrator, CustomClaimValues.Content), Users.AdminId).Wait();
            var signInManager = Resolve<SignInManager<ApplicationUser>>();
            signInManager.Context = new DefaultHttpContext();
        }

        protected T Resolve<T>()
        {
            return _ServiceProvider.GetRequiredService<T>();
        }

        protected virtual IQueryable<TEntity> GetQueryable()
        {
            return _context.Set<TEntity>();
        }

        protected async Task GetCategoryHierarchyAsync(int id, HashSet<Category> hierarchy)
        {
            await GetCategoryParentsAsync(id, hierarchy);
            await GetCategorySubcategoriesAsync(id, hierarchy);
        }

        protected async Task GetCategoryParentsAsync(int categoryId, HashSet<Category> hierarchy)
        {
            var category = await _context
                .Set<Category>()

                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();
            if (category != null)
            {
                hierarchy.Add(category);
                if (category.ParentCategoryId != 0)
                    await GetCategoryParentsAsync(category.ParentCategoryId, hierarchy);
            }
        }

        public async Task GetCategorySubcategoriesAsync(int categoryId, HashSet<Category> hierarchy)
        {
            var category = await _context
                .Set<Category>()
                .Where(c => c.Id == categoryId)
                .Include(c => c.Subcategories)
                .FirstOrDefaultAsync();
            hierarchy.Add(category);

            foreach (var subCategory in category.Subcategories)
            {
                await GetCategorySubcategoriesAsync(subCategory.Id, hierarchy);
            }
        }

        protected virtual void Equal<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (expected.Count() == actual.Count())
            {
                int count = expected.Count();
                for (int i = 0; i < count; i++)
                    Equal(expected.ElementAt(i), actual.ElementAt(i));
            }
            else
                Assert.True(false);
        }

        protected virtual void Equal<T>(T expected, T actual)
        {
            Assert.True(expected.Equals(actual));
            //Assert.Equal(
            //    JsonConvert.SerializeObject(expected, Formatting.None, jsonSettings),
            //    JsonConvert.SerializeObject(actual, Formatting.None, jsonSettings));
        }
    }
}
