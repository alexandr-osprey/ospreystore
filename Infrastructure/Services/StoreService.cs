﻿using ApplicationCore.Entities;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Infrastructure.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class StoreService: Service<Store>, IStoreService
    {
        protected IItemService _itemService;

        public StoreService(
           StoreContext context,
            IIdentityService identityService,
            IScopedParameters scopedParameters,
            IItemService itemService,
            IAuthorizationParameters<Store> authoriationParameters,
            IAppLogger<Service<Store>> logger)
           : base(context, identityService, scopedParameters, authoriationParameters, logger)
        {
            _itemService = itemService;
        }

        public override async Task<Store> CreateAsync(Store entity)
        {
            var store = await base.CreateAsync(entity);
            await IdentityService.AddClaim(entity.OwnerId, new Claim(entity.OwnerId, entity.Id.ToString()));
            return store;
        }

        override public async Task<int> DeleteAsync(Specification<Store> spec)
        {
            spec.AddInclude((s => s.Items));
            spec.Description += ", includes Items";
            return await base.DeleteAsync(spec);
        }

        protected override async Task DeleteSingleAsync(Store entity)
        {
            await base.DeleteSingleAsync(entity);
            await IdentityService.RemoveFromUsersAsync(new Claim(entity.OwnerId, entity.Id.ToString()));
        }
        public override async Task DeleteRelatedEntitiesAsync(Store store)
        {
            var spec = new Specification<Item>(i => store.Items.Contains(i))
            {
                Description = $"Item with StoreId={store.Id}"
            };
            await _itemService.DeleteAsync(spec);
            await base.DeleteRelatedEntitiesAsync(store);
        }
    }
}
