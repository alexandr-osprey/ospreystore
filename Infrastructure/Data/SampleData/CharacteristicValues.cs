﻿using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Data.SampleData
{
    public class CharacteristicValues: SampleDataEntities<CharacteristicValue>
    {
        protected Characteristics Characteristics { get; }

        public CharacteristicValue GB16 { get; protected set; } 
        public CharacteristicValue GB32 { get; protected set; }
        public CharacteristicValue GB64 { get; protected set; }
        public CharacteristicValue HD { get; protected set; }
        public CharacteristicValue FullHD { get; protected set; }
        public CharacteristicValue MAh1000 { get; protected set; }
        public CharacteristicValue MAh2000 { get; protected set; }
        public CharacteristicValue X { get; protected set; }
        public CharacteristicValue XL { get; protected set; }
        public CharacteristicValue XXL { get; protected set; }
        public CharacteristicValue MuchFashion { get; protected set; }
        public CharacteristicValue NotSoFashion { get; protected set; }
        public CharacteristicValue Black { get; protected set; }
        public CharacteristicValue White { get; protected set; }

        public CharacteristicValues(StoreContext storeContext, Characteristics characteristics) : base(storeContext)
        {
            Characteristics = characteristics;
            Seed();
            Init();
        }

        protected override IEnumerable<CharacteristicValue> GetSourceEntities()
        {
            return new List<CharacteristicValue>
            {
                new CharacteristicValue { Title = "16GB", CharacteristicId = Characteristics.Storage.Id, OwnerId = Users.AdminId }, //0
                new CharacteristicValue { Title = "32GB", CharacteristicId = Characteristics.Storage.Id, OwnerId = Users.AdminId }, //1
                new CharacteristicValue { Title = "64GB", CharacteristicId = Characteristics.Storage.Id, OwnerId = Users.AdminId }, //2
                new CharacteristicValue { Title = "HD", CharacteristicId = Characteristics.Resolution.Id, OwnerId = Users.AdminId }, //3
                new CharacteristicValue { Title = "Full HD", CharacteristicId = Characteristics.Resolution.Id, OwnerId = Users.AdminId }, //4
                new CharacteristicValue { Title = "1000 mAh", CharacteristicId = Characteristics.Battery.Id, OwnerId = Users.AdminId }, //5
                new CharacteristicValue { Title = "2000 mAh", CharacteristicId = Characteristics.Battery.Id, OwnerId = Users.AdminId }, //6

                new CharacteristicValue { Title = "X", CharacteristicId = Characteristics.Size.Id, OwnerId = Users.AdminId }, //7
                new CharacteristicValue { Title = "XL", CharacteristicId = Characteristics.Size.Id, OwnerId = Users.AdminId }, //8
                new CharacteristicValue { Title = "XXL", CharacteristicId = Characteristics.Size.Id, OwnerId = Users.AdminId }, //9
                new CharacteristicValue { Title = "Much fashion", CharacteristicId = Characteristics.Fashion.Id, OwnerId = Users.AdminId }, //10
                new CharacteristicValue { Title = "Not so fashion", CharacteristicId = Characteristics.Fashion.Id, OwnerId = Users.AdminId }, //11
                new CharacteristicValue { Title = "Black", CharacteristicId = Characteristics.Colour.Id, OwnerId = Users.AdminId }, //12
                new CharacteristicValue { Title = "White", CharacteristicId = Characteristics.Colour.Id, OwnerId = Users.AdminId }, //13
            };
        }

        protected override IQueryable<CharacteristicValue> GetQueryable()
        {
            return base.GetQueryable().Include(c => c.Characteristic).Include(c => c.ItemProperties);
        }

        public override void Init()
        {
            base.Init();
            GB16 = Entities[0];
            GB32 = Entities[1];
            GB64 = Entities[2];
            HD = Entities[3];
            FullHD = Entities[4];
            MAh1000 = Entities[5];
            MAh2000 = Entities[6];
            X = Entities[7];
            XL = Entities[8];
            XXL = Entities[9];
            MuchFashion = Entities[10];
            NotSoFashion = Entities[11];
            Black = Entities[12];
            White = Entities[13];
        }
    }
}