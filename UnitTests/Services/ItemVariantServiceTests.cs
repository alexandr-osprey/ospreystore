﻿//using ApplicationCore.Entities;
//using ApplicationCore.Interfaces;
//using ApplicationCore.Specifications;
//using Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;

//namespace UnitTests.Services
//{
//    public class ItemVariantServiceTests: ServiceTestBase<ItemVariant, IItemVariantService>
//    {
//        public ItemVariantServiceTests(ITestOutputHelper output)
//           : base(output)
//        {
//        }

//        [Fact]
//        public async Task CreateAsync()
//        {
//            var variant = new ItemVariant() { Price = 100, ItemId = 1, Title = "added" };
//            variant.ItemVariantCharacteristicValues = new List<ItemVariantCharacteristicValue>
//            {
//                new ItemVariantCharacteristicValue() { CharacteristicValueId = 2 },
//                new ItemVariantCharacteristicValue() { CharacteristicValueId = 4 }
//            };
//            await _service.CreateAsync(variant);
//            Assert.True(context.ItemVariants.Contains(variant));
//            Assert.True(context.ItemVariantCharacteristicValues.Contains(variant.ItemVariantCharacteristicValues.ElementAt(1)));

//            variant.Id = 0;
//            variant.ItemVariantCharacteristicValues = null;
//            await _service.CreateAsync(variant);
//            Assert.True(context.ItemVariants.Contains(variant));
//            Assert.True(await context.ItemVariantCharacteristicValues.AnyAsync(i => i.ItemVariantId == variant.Id));
//        }
//        [Fact]
//        public async Task UpdateAsync()
//        {
//            var expected = await GetQueryable().FirstOrDefaultAsync();
//            expected.Title = "new title";
//            expected.ItemVariantCharacteristicValues.Remove(expected.ItemVariantCharacteristicValues.Last());
//            await _service.UpdateAsync(expected);
//            expected.ItemVariantCharacteristicValues = await context
//                .ItemVariantCharacteristicValues
//                .Where(i => i.ItemVariantId == expected.Id).ToListAsync();
//            foreach (var v in expected.ItemVariantCharacteristicValues)
//            {
//                v.ItemVariant = expected;
//            }
//            var actual = await GetQueryable()
//                .FirstOrDefaultAsync(i => i.Id == expected.Id);
//            Assert.Equal(
//                JsonConvert.SerializeObject(expected, Formatting.None, jsonSettings),
//                JsonConvert.SerializeObject(actual, Formatting.None, jsonSettings));
//        }
//        [Fact]
//        public async Task DeleteAsync()
//        {
//            var variant = await GetQueryable().LastOrDefaultAsync();
//            await _service.DeleteAsync(new EntitySpecification<ItemVariant>(variant.Id));
//            Assert.False(context.ItemVariants.Contains(variant));
//            Assert.False(await context.ItemVariantCharacteristicValues.AnyAsync(i => i.ItemVariantId == variant.Id));
//        }
//        protected override IQueryable<ItemVariant> GetQueryable()
//        {
//            return base.GetQueryable().Include(i => i.ItemVariantCharacteristicValues);
//        }
//    }
//}
