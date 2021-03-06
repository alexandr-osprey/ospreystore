﻿using ApplicationCore.Entities;
using System.Threading.Tasks;
using ApplicationCore.Specifications;
using Infrastructure.Data;
using ApplicationCore.Interfaces;
using System.Collections.Generic;
using ApplicationCore.Identity;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces.Services;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services
{
    public class ItemService: Service<Item>, IItemService
    {
        protected IItemVariantImageService _itemVariantImageService { get; }
        protected ICategoryService _categoryService { get; }

        public ItemService(
            StoreContext context,
            IIdentityService identityService,
            IItemVariantImageService itemVariantImageService,
            ICategoryService categoryService,
            IScopedParameters scopedParameters,
            IAuthorizationParameters<Item> authoriationParameters,
            IAppLogger<Service<Item>> logger)
           : base(context, identityService, scopedParameters, authoriationParameters, logger)
        {
            _itemVariantImageService = itemVariantImageService;
            _categoryService = categoryService;
        }

        public async Task<ItemIndexSpecification> GetIndexSpecificationByParameters(int page, int pageSize, string searchValue, int? categoryId, int? storeId, int? brandId , HashSet<int> characteristicValueIds) 
            => new ItemIndexSpecification(page, pageSize, searchValue, await GetCategoriesAsync(categoryId ?? _categoryService.RootCategory.Id), storeId, brandId, characteristicValueIds);

        public override async Task DeleteRelatedEntitiesAsync(Item item)
        {
            var imageDeleteSpec = new Specification<ItemVariantImage>(i => i.RelatedId == item.Id)
            {
                Description = $"ItemImage with item id={item.Id}"
            };
            await _itemVariantImageService.DeleteAsync(imageDeleteSpec);
            await base.DeleteRelatedEntitiesAsync(item);
        }

        protected async Task<IEnumerable<Category>> GetCategoriesAsync(int categoryId)
        {
            return await _categoryService.EnumerateSubcategoriesAsync(new EntitySpecification<Category>(categoryId));
        }

        protected override async Task ValidateWithExceptionAsync(EntityEntry<Item> entry)
        {
            await base.ValidateWithExceptionAsync(entry);
            var item = entry.Entity;
            if (string.IsNullOrWhiteSpace(item.Title))
                throw new EntityValidationException("Incorrect title");
            if (string.IsNullOrWhiteSpace(item.Description))
                throw new EntityValidationException("Incorrect description");
            var entityEntry = _сontext.Entry(item);
            if (IsPropertyModified(entityEntry, p => p.CategoryId, false))
            {
                var category = await _сontext.ReadByKeyAsync<Category, Service<Item>>(_logger, item.CategoryId, false)
                    ?? throw new EntityValidationException($"Category with Id {item.CategoryId} does not exist. ");
                if (!category.CanHaveItems)
                    throw new EntityValidationException($"Category with Id {item.CategoryId} can't have items. ");
            }
            if (IsPropertyModified(entityEntry, p => p.StoreId, false) 
                && !await _сontext.ExistsBySpecAsync(_logger, new EntitySpecification<Store>(item.StoreId), false))
                throw new EntityValidationException($"Store with Id {item.StoreId} does not exist. ");
            if (IsPropertyModified(entityEntry, p => p.BrandId, false)
                && !await _сontext.ExistsBySpecAsync(_logger, new EntitySpecification<Brand>(item.BrandId), false))
                throw new EntityValidationException($"Brand with Id {item.BrandId} does not exist. ");
        }
    }
}
