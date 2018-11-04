﻿using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.ViewModels;
using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Interfaces.Controllers;

namespace OctopusStore.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CharacteristicsController: CRUDController<ICharacteristicService, Characteristic, CharacteristicViewModel>, ICharacteristicsController
    {
        public CharacteristicsController(
            ICharacteristicService service,
            IScopedParameters scopedParameters,
            IAppLogger<IController<Characteristic, CharacteristicViewModel>> logger)
           : base(service, scopedParameters, logger)
        {
        }

        [AllowAnonymous]
        [HttpGet]
        [HttpGet("/api/categories/{categoryId:int}/characteristics")]
        public async Task<IndexViewModel<CharacteristicViewModel>> IndexAsync([FromQuery(Name = "categoryId")]int categoryId)
        {
            return await base.IndexByFunctionNotPagedAsync(Service.EnumerateByCategoryAsync, new EntitySpecification<Category>(categoryId));
        }

        [HttpGet("{id:int}/checkUpdateAuthorization")]
        public override async Task<Response> CheckUpdateAuthorizationAsync(int id) => await base.CheckUpdateAuthorizationAsync(id);
    }
}
