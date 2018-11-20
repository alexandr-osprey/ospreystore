﻿using ApplicationCore.Entities;
using ApplicationCore.Exceptions;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public abstract class Service<TEntity>: IService<TEntity> where TEntity: Entity
    {
        public DbContext Context { get; }
        protected IAppLogger<Service<TEntity>> Logger { get; }
        protected IScopedParameters ScopedParameters { get; }
        protected IAuthorizationParameters<TEntity> AuthoriationParameters { get; }

        public IIdentityService IdentityService { get; }
        public string Name { get; set; }

        public Service(
            DbContext context,
            IIdentityService identityService,
            IScopedParameters scopedParameters,
            IAuthorizationParameters<TEntity> authoriationParameters,
            IAppLogger<Service<TEntity>> logger)
        {
            Context = context;
            ScopedParameters = scopedParameters;
            AuthoriationParameters = authoriationParameters;
            IdentityService = identityService;
            Logger = logger;
            Name = typeof(TEntity).Name + "Service";
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            entity.OwnerId = ScopedParameters.ClaimsPrincipal.Identity.Name;
            await ValidateCreateWithExceptionAsync(entity);
            await ValidateCustomUniquinessWithException(entity);
            if (AuthoriationParameters.CreateAuthorizationRequired)
            {
                await AuthorizeWithException(entity, AuthoriationParameters.CreateOperationRequirement);
            }
            //await ValidateUniquiness
            var result = await Context.CreateAsync(Logger, entity);
            Logger.Trace("{Name} added entity {entity}", Name, result);
            return result;
        }

        public virtual async Task<TEntity> ReadSingleAsync(Specification<TEntity> spec)
        {
            var entity = await Context.ReadSingleBySpecAsync(Logger, spec);
            if (AuthoriationParameters.ReadAuthorizationRequired)
                await AuthorizeWithException(entity, AuthoriationParameters.ReadOperationRequirement);
            Logger.Trace("{Name} retreived single entity {entity} by spec: {spec}", Name, entity, spec);
            return entity;
        }

        public virtual async Task<TEntity> ReadSingleAsync(TEntity entity)
        {
            entity = await Context.ReadSingleAsync(Logger, entity);
            if (AuthoriationParameters.ReadAuthorizationRequired)
                await AuthorizeWithException(entity, AuthoriationParameters.ReadOperationRequirement);
            Logger.Trace("{Name} retreived single entity {entity}", Name, entity);
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await ValidateUpdateWithExceptionAsync(entity);
            if (AuthoriationParameters.UpdateAuthorizationRequired)
                await AuthorizeWithException(entity, AuthoriationParameters.UpdateOperationRequirement);
            var result = await Context.UpdateSingleAsync(Logger, entity);
            Logger.Trace("{Name} updated entity {entity}", Name, result);
            return result;
        }

        public virtual async Task<IEnumerable<TEntity>> EnumerateAsync(Specification<TEntity> spec)
        {
            var entities = await Context.EnumerateAsync(Logger, spec);
            if (AuthoriationParameters.ReadAuthorizationRequired)
                entities = await ReadAuthorizedOnlyFilter(entities);
            Logger.Trace("{Name} listed: {resultCount} entities by spec: {spec}", Name, entities.Count(), spec);
            return entities;
        }

        public virtual async Task<IEnumerable<TRelated>> EnumerateRelatedAsync<TRelated>(Specification<TEntity> spec, Expression<Func<TEntity, TRelated>> relatedSelect) where TRelated: class
        {
            var relatedEntities = await Context.EnumerateRelatedAsync(Logger, spec, relatedSelect);
            if (AuthoriationParameters.ReadAuthorizationRequired)
                relatedEntities = await ReadAuthorizedOnlyFilter(relatedEntities);
            Logger.Trace("{Name} listed related: {resultCount} entities by spec: {spec}", Name, relatedEntities.Count(), spec);
            return relatedEntities;
        }

        public async Task<IEnumerable<TRelated>> EnumerateRelatedEnumAsync<TRelated>(
           Specification<TEntity> listRelatedSpec,
           Expression<Func<TEntity, IEnumerable<TRelated>>> relatedEnumSelect) where TRelated: class
        {
            var relatedEntities = await Context.EnumerateRelatedEnumAsync(Logger, listRelatedSpec, relatedEnumSelect);
            if (AuthoriationParameters.ReadAuthorizationRequired)
                relatedEntities = await ReadAuthorizedOnlyFilter(relatedEntities);
            Logger.Trace("{Name} listed related enum: {resultCount} entities by spec: {spec}", Name, relatedEntities.Count(), listRelatedSpec);
            return relatedEntities;
        }

        public virtual async Task DeleteSingleAsync(Specification<TEntity> spec)
        {
            var entity = await Context.ReadSingleBySpecAsync(Logger, spec);
            await DeleteSingleAsync(entity);
            Logger.Trace("{Name} deleted: {entity} by spec: {spec}", Name, entity, spec);
        }

        public virtual async Task<int> DeleteAsync(Specification<TEntity> spec)
        {
            var entities = await Context.EnumerateAsync(Logger, spec);
            foreach (var entity in entities)
                await DeleteSingleAsync(entity);
            Logger.Trace("{Name} deleted: {resultCount} by spec: {spec}", Name, entities.Count(), spec);
            return entities.Count();
        }

        public virtual async Task DeleteSingleWithRelatedRelink(int id, int idToRelinkTo)
        {
            await RelinkRelatedAsync(id, idToRelinkTo);
            Logger.Trace("{Name} relinked entity from {id} to {idToRelinkTo} before delete", Name, id, idToRelinkTo);
            await DeleteSingleAsync(new EntitySpecification<TEntity>(id));
        }

        /// <summary>
        /// Relinks slave entities to another entity
        /// Checking update rights for linked entities is not mandatory, since entity being deleted is supposed to be master, and related entities are slaves, 
        /// therefore delete rights for master entity already include update right for entities being deleted
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idToRelinkTo"></param>
        /// <returns></returns>
        public abstract Task RelinkRelatedAsync(int id, int idToRelinkTo);

        protected virtual async Task DeleteSingleAsync(TEntity entity)
        {
            if (AuthoriationParameters.DeleteAuthorizationRequired)
                await AuthorizeWithException(entity, AuthoriationParameters.DeleteOperationRequirement);
            await DeleteRelatedEntitiesAsync(entity);
            await Context.DeleteAsync(Logger, entity, false);
            Logger.Trace("{Name} deleted {entity}", Name, entity);
        }

        public virtual async Task DeleteRelatedEntitiesAsync(TEntity entity)
        {
            Logger.Trace("{Name} deleted entity related to {entity} (if such existed)", Name, entity);
            await Task.CompletedTask;
        }

        protected virtual async Task ValidateCustomUniquinessWithException(TEntity entity)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task ValidateCreateWithExceptionAsync(TEntity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.OwnerId))
                throw new EntityValidationException("OwnerId not specified");
            await Task.CompletedTask;
        }

        protected virtual async Task ValidateUpdateWithExceptionAsync(TEntity entity)
        {
            //_logger.Trace("{Name} validated {entity}", Name, entity);
            await Task.CompletedTask;
        }

        public virtual async Task<int> PageCountAsync(Specification<TEntity> spec)
        {
            int totalCount = await Context.CountAsync(Logger, spec);
            int take = spec.Take == 0 ? totalCount : spec.Take;
            int result = (int)Math.Ceiling(((decimal)totalCount / take));
            Logger.Trace("{Name} got page count {count} by spec: {spec}", Name, result, spec);
            return result;
        }

        public virtual async Task<int> CountTotalAsync(Specification<TEntity> spec)
        {
            int result = await Context.CountAsync(Logger, spec);
            Logger.Trace("{Name} got entities count {count} by spec: {spec}", Name, result, spec);
            return result;
        }

        public virtual async Task<bool> ExistsAsync(Specification<TEntity> spec)
        {
            bool result = await Context.ExistsBySpecAsync(Logger, spec);
            Logger.Trace("{Name} checked existance ({result}) by spec: {spec}", Name, result, spec);
            return result;
        }

        public virtual async Task<bool> ExistsAsync(TEntity entity)
        {
            bool result = await Context.ExistsAsync(Logger, entity);
            Logger.Trace("{Name} checked existance ({result})", Name, result, entity);
            return result;
        }

        protected async Task<IEnumerable<TCustom>> ReadAuthorizedOnlyFilter<TCustom>(IEnumerable<TCustom> entities) where TCustom: class
        {
            var authorizedEntities = new List<TCustom>();
            foreach (var entity in entities)
                if (await Authorize(entity, AuthoriationParameters.ReadOperationRequirement))
                    authorizedEntities.Add(entity);
            return authorizedEntities;
        }

        protected async Task AuthorizeWithException<TCustom>(TCustom entity, OperationAuthorizationRequirement requirement) where TCustom: class
        {
            if (!await Authorize(entity, requirement))
            {
                string message = $"{entity} {requirement.Name} authorization failure";
                Logger.Trace(message);
                throw new AuthorizationException(message);
            }
        }

        protected async Task AuthorizeWithException<TCustom>(object key, OperationAuthorizationRequirement requirement) where TCustom: class
        {
            var entity = Context.ReadByKeyAsync<TCustom, Service<TEntity>>(Logger, key, true);
            await AuthorizeWithException(entity, requirement);
        }

        protected async Task<bool> Authorize(object obj, OperationAuthorizationRequirement requirement)
        {
            return await IdentityService.AuthorizeAsync(ScopedParameters.ClaimsPrincipal, obj, requirement);
        }
    }
}
