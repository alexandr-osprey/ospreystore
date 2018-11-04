﻿using Xunit.Abstractions;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Interfaces.Services;

namespace UnitTests.Services
{
    public class StoreServiceTests : ServiceTests<Store, IStoreService>
    {
        public StoreServiceTests(ITestOutputHelper output)
           : base(output)
        {
        }

        protected override IEnumerable<Store> GetCorrectEntitesForUpdate()
        {
            var store = Data.Stores.Jennifers;
            store.Title = "Upd 1";
            store.Description = "Upd 1";
            store.Address = "Upd 1";
            return new List<Store>()
            {
                store
            };
        }

        protected override IEnumerable<Store> GetCorrectNewEntites()
        {
            return new List<Store>()
            {
                new Store() { Title = "New store",  Address = "New address", Description = "New desc "}
            };
        }

        protected override Specification<Store> GetEntitiesToDeleteSpecification()
        {
            return new Specification<Store>(s => s.Title.Contains("Jennifer"));
        }

        protected override IEnumerable<Store> GetIncorrectEntitesForUpdate()
        {
            var first = Data.Stores.Jennifers;
            first.Title = null;
            first.Description = "Upd 1";
            first.Address = "Upd 1";
            return new List<Store>()
            {
                first
            };
        }

        protected override IEnumerable<Store> GetIncorrectNewEntites()
        {
            var first = Data.Stores.Johns;
            first.Title = null;
            first.Description = "Upd 1";
            first.Address = "Upd 1";
            return new List<Store>()
            {
                //new Store() { Title = first.Title, Description = first.Description, Address = first.Address },
                new Store() { Title = null, Description = first.Description, Address = first.Address },
                new Store() { Title = first.Title, Description = "", Address = first.Address },
                new Store() { Title = first.Title, Description = first.Description, Address = null },
            };
        }
    }
}
