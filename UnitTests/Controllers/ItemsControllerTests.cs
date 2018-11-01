﻿//using ApplicationCore.Entities;
//using ApplicationCore.Exceptions;
//using ApplicationCore.Interfaces;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using OctopusStore.Controllers;
//using ApplicationCore.Specifications;
//using ApplicationCore.ViewModels;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;

//namespace UnitTests.Controllers
//{
//    public class ItemControllerTests: ControllerTestBase<Item, ItemsController, IItemService>
//    {
//        private readonly ICategoryService _categoryService;

//        public ItemControllerTests(ITestOutputHelper output): base(output)
//        {
//            _categoryService = Resolve<ICategoryService>();
//        }

//        [Fact]
//        public async Task IndexWithoutFilters()
//        {
//            int page = 2;
//            int pageSize = 3;

//            var actual = (await controller.Index(page, pageSize, null, null, null, null));
//            var items = await GetQueryable()
//                .Skip(pageSize * (page - 1))
//                .Take(pageSize).ToListAsync();
//            int totalCount = await _context.Items.CountAsync();
//            var expected = new IndexViewModel<ItemViewModel>(
//                page,
//                GetPageCount(totalCount, pageSize),
//                totalCount,
//                from i in items select new ItemViewModel(i));
//            Equal(expected, actual);
//        }
//        [Fact]
//        public async Task IndexWithoutFiltersWithoutPageSize()
//        {
//            int page = 2;
//            int take = 50;

//            var actual = (await controller.Index(page, null, null, null, null, null));
//            var items = await GetQueryable()
//                .Skip(take * (page - 1))
//                .Take(take).ToListAsync();
//            int totalCount = await _context.Items.CountAsync();
//            var expected = new IndexViewModel<ItemViewModel>(
//                page,
//                GetPageCount(totalCount, take),
//                totalCount,
//                from i in items select new ItemViewModel(i));
//            Equal(expected, actual);
//        }

//        [Fact]
//        public async Task IndexWithTitle()
//        {
//            int page = 1;
//            int take = 50;

//            var actual = (await controller.Index(null, null, "sa", null, null, null));
//            var items = await GetQueryable().Where(i => i.Title.Contains("Sa")).ToListAsync();
//            int totalCount = items.Count;
//            var expected = new IndexViewModel<ItemViewModel>(
//                page,
//                GetPageCount(totalCount, take),
//                totalCount,
//                from i in items select new ItemViewModel(i));
//            Equal(expected, actual);
//        }
//        [Fact]
//        public async Task IndexWithAllFilters()
//        {
//            var samsung7 = await _context.Items
//            .AsNoTracking()
//            .Where(i => i.Title.Contains("Samsung 7"))
//            .FirstOrDefaultAsync();

//            int page = 1;
//            int pageSize = 5;
//            var query = GetQueryable()
//                .Where(i => i.Title.Contains("Samsung"))
//                .Where(i => i.StoreId == samsung7.StoreId)
//                .Where(i => i.BrandId == samsung7.BrandId);
//            int totalCount = query.Count();
//            var items = await query
//                .Skip(pageSize * (page - 1))
//                .Take(pageSize).ToListAsync();
//            var expected = new IndexViewModel<ItemViewModel>(page, GetPageCount(totalCount, pageSize), totalCount, from i in items select new ItemViewModel(i));
//            var actual = (await controller.Index(page, pageSize, "Samsung", samsung7.CategoryId, samsung7.StoreId, samsung7.BrandId));
//            Equal(expected, actual);
//        }
//        [Fact]
//        public async Task IndexWithAllFiltersThumbnails()
//        {
//            var samsung7 = await _context.Items
//            .AsNoTracking()
//            .Where(i => i.Title.Contains("Samsung 7"))
//            .FirstOrDefaultAsync();

//            int page = 1;
//            int pageSize = 5;
//            var query = GetQueryable()
//                .Where(i => i.Title.Contains("Samsung"))
//                .Where(i => i.StoreId == samsung7.StoreId)
//                .Where(i => i.BrandId == samsung7.BrandId);
//            int totalCount = query.Count();
//            var items = await query
//                .Skip(pageSize * (page - 1))
//                .Take(pageSize).ToListAsync();
//            var expected = new IndexViewModel<ItemThumbnailViewModel>(page, GetPageCount(totalCount, pageSize), totalCount, from i in items select new ItemThumbnailViewModel(i));
//            var actual = (await controller.IndexThumbnails(page, pageSize, "Samsung", samsung7.CategoryId, samsung7.StoreId, samsung7.BrandId));
//            Equal(expected, actual);
//        }
//        [Fact]
//        public async Task IndexByCategory()
//        {
//            var clothesCategory = await _context.Categories
//            .AsNoTracking()
//            .Where(c => c.Title == "Clothes")
//            .Include(c => c.Subcategories)
//            .FirstOrDefaultAsync();

//            var categories = await _categoryService.EnumerateSubcategoriesAsync(new CategoryDetailSpecification(clothesCategory.Id));

//            int page = 1;
//            int pageSize = 5;
//            var query = GetQueryable()
//                .Where(i => categories.Where(c => c.Id == i.CategoryId).Any());
//            int totalCount = query.Count();
//            var items = await query
//                .Skip(pageSize * (page - 1))
//                .Take(pageSize).ToListAsync();
//            var expected = new IndexViewModel<ItemViewModel>(page, GetPageCount(totalCount, pageSize), totalCount, from i in items select new ItemViewModel(i));
//            var actual = (await controller.Index(null, null, null, clothesCategory.Id, null, null));
//            Equal(expected, actual);
//        }
//        [Fact]
//        public async Task Get()
//        {
//            var samsungItem = await _context.Items
//            .AsNoTracking()
//            .Where(i => i.Title.Contains("Samsung 7"))
//            .FirstOrDefaultAsync();
//            var expectedItem = await GetQueryable()
//                .FirstOrDefaultAsync(i => i.Id == samsungItem.Id);
//            var expected = new ItemViewModel(expectedItem);
//            var actual = await controller.Get(samsungItem.Id);
//            Equal(expected, actual);

//            await Assert.ThrowsAsync<EntityNotFoundException>(() => controller.Get(9999));
//        }
//        [Fact]
//        public async Task GetDetail()
//        {
//            var samsungItem = await _context.Items
//                .AsNoTracking()
//                .Where(i => i.Title.Contains("Samsung 7"))
//                .FirstOrDefaultAsync();
//            var expectedItem = await GetQueryable()
//                .FirstOrDefaultAsync(i => i.Id == samsungItem.Id);
//            var expected = new ItemDetailViewModel(expectedItem);
//            var actual = await controller.GetDetail(samsungItem.Id);
//            Equal(expected, actual);

//            await Assert.ThrowsAsync<EntityNotFoundException>(() => controller.Get(9999));
//        }
//        [Fact]
//        public async Task Post()
//        {
//            var samsungItem = await _context.Items
//                .AsNoTracking()
//                .Where(i => i.Title.Contains("Samsung 7"))
//                .FirstOrDefaultAsync();
//            samsungItem.Title += "_ADDED";
//            samsungItem.Id = 0;

//            var itemViewModel = new ItemViewModel(samsungItem);
//            var actual = await controller.Post(itemViewModel);
//            var expected = new ItemViewModel(
//                    await GetQueryable()
//                        .FirstOrDefaultAsync(i => i.Id == actual.Id));
//            Equal(expected, actual);
//        }
//        [Fact]
//        public async Task Put()
//        {
//            var samsungItem = await _context.Items
//               .AsNoTracking()
//               .Where(i => i.Title.Contains("Samsung 7"))
//               .FirstOrDefaultAsync();
//            samsungItem = await GetQueryable().FirstOrDefaultAsync(i => i.Id == samsungItem.Id);
//            samsungItem.Title += "_UPDATED";
//            var itemViewModel = new ItemViewModel(samsungItem);
//            var actual = await controller.Put(itemViewModel.Id, itemViewModel);
//            var expItem = await GetQueryable()
//                .FirstOrDefaultAsync(i => i.Id == samsungItem.Id);
//            var expected = new ItemViewModel(expItem);
//            Equal(expected, actual);
//            itemViewModel.Id = 9999;
//            await Assert.ThrowsAsync<EntityNotFoundException>(() => controller.Put(itemViewModel.Id, itemViewModel));
//        }
//        [Fact]
//        public async Task Delete()
//        {
//            int id = 3;
//            var item = await GetQueryable().FirstOrDefaultAsync(i => i.Id == id);
//            foreach (var image in item.Images)
//                await CreateItemImageCopy(image);
//            await controller.Delete(item.Id);
//            Assert.False(_context.Items.Where(i => i.Id == id).Any());
//            Assert.False(_context.ItemVariants.Where(v => v.ItemId == item.Id).Any());
//            Assert.False(_context.ItemImages.Where(i => i.RelatedId == item.Id).Any());
//            await Assert.ThrowsAsync<EntityNotFoundException>(() => controller.Delete(9999));
//        }
//        protected override IQueryable<Item> GetQueryable()
//        {
//            return _context
//                .Set<Item>()
//                .AsNoTracking()
//                .Include(i => i.Brand)
//                .Include(i => i.Store)
//                .Include(i => i.MeasurementUnit)
//                .Include(i => i.Images)
//                .Include(i => i.Category)
//                .Include(i => i.ItemVariants)
//                    .ThenInclude(j => j.ItemProperties)
//                        .ThenInclude(o => o.CharacteristicValue)
//                            .ThenInclude(c => c.Characteristic);
//        }
//    }
//}