﻿using ApplicationCore.Entities;
using ApplicationCore.Exceptions;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Specifications;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ItemVariantService: Service<ItemVariant>, IItemVariantService
    {
        public ItemVariantService(
            StoreContext context,
            IIdentityService identityService,
            IScopedParameters scopedParameters,
            IAuthorizationParameters<ItemVariant> authoriationParameters,
            IAppLogger<Service<ItemVariant>> logger)
           : base(context, identityService, scopedParameters, authoriationParameters, logger)
        {
        }

        protected override async Task ValidateWithExceptionAsync(EntityEntry<ItemVariant> entry)
        {
            await base.ValidateWithExceptionAsync(entry);
            var itemVariant = entry.Entity;
            if (string.IsNullOrWhiteSpace(itemVariant.Title))
                throw new EntityValidationException($"Incorrect title. ");
            if (itemVariant.Price <= 0)
                throw new EntityValidationException($"Price can't be zero or less. ");
            var entityEntry = _сontext.Entry(itemVariant);
            if (IsPropertyModified(entry, v => v.ItemId, false) 
                && !await _сontext.ExistsBySpecAsync(_logger, new EntitySpecification<Item>(entry.Entity.ItemId)))
                throw new EntityValidationException($"Item with Id {entry.Entity.ItemId} does not exist. ");
        }
    }
}
