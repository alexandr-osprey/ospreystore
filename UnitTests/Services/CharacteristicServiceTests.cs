﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace UnitTests.Services
{
    public class CharacteristicServiceTests : ServiceTests<Characteristic, ICharacteristicService>
    {
        public CharacteristicServiceTests(ITestOutputHelper output)
           : base(output)
        {
        }

        protected override IEnumerable<Characteristic> GetCorrectNewEntites()
        {
            return new List<Characteristic>()
            {
                new Characteristic() { Title = "title 1", CategoryId = Data.Categories.Jackets.Id },
                new Characteristic() { Title = "title 2", CategoryId = Data.Categories.Smartphones.Id },
            };
        }

        protected override IEnumerable<Characteristic> GetIncorrectNewEntites()
        {
            return new List<Characteristic>()
            {
                new Characteristic() { Title = null, CategoryId = Data.Categories.Jackets.Id },
                new Characteristic() { Title = "new2", CategoryId = 0 },
            };
        }

        protected override Specification<Characteristic> GetEntitiesToDeleteSpecification()
        {
            return new EntitySpecification<Characteristic>(c => c == Data.Characteristics.Fashion);
        }

        protected override IEnumerable<Characteristic> GetCorrectEntitesForUpdate()
        {
            Data.Characteristics.Storage.Title = "Updated storage";
            return new List<Characteristic>() { Data.Characteristics.Storage };
        }

        protected override IEnumerable<Characteristic> GetIncorrectEntitesForUpdate()
        {
            Data.Characteristics.Size.Title = "";
            return new List<Characteristic>()
            {
                //   new Characteristic() { Id = first.Id, Title = first.Title, CategoryId = first.CategoryId, OwnerId = first.OwnerId },
                Data.Characteristics.Size
            };
        }
    }
}
