﻿using ApplicationCore.Interfaces;
using System.Collections.Generic;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Item for selling
    /// </summary>
    public class Item: Entity, IGenericMemberwiseClonable<Item>
    {
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public int StoreId { get; set; }
        public int BrandId { get; set; }
        public int MeasurementUnitId { get; set; }
        public string Description { get; set; }

        public Category Category { get; set; }
        public Store Store { get; set; }
        public Brand Brand { get; set; }
        public MeasurementUnit MeasurementUnit { get; set; }
        public ICollection<ItemImage> Images { get; set; } = new List<ItemImage>();
        public ICollection<ItemVariant> ItemVariants { get; set; } = new List<ItemVariant>();

        public Item(): base()
        {
        }
        protected Item(Item item): base(item)
        {
            Title = item.Title;
            CategoryId = item.CategoryId;
            StoreId = item.StoreId;
            BrandId = item.BrandId;
            MeasurementUnitId = item.MeasurementUnitId;
            Description = item.Description;
        }

        public Item ShallowClone()
        {
            return (Item)MemberwiseClone();
        }
    }
}
