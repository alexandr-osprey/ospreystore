﻿using ApplicationCore.Entities;
using ApplicationCore.ViewModels;

namespace ApplicationCore.ViewModels
{
    public class ItemVariantViewModel: EntityViewModel<ItemVariant>
    {
        public string Title { get; set; }
        public int ItemId { get; set; }
        public decimal Price { get; set; }

        public ItemVariantViewModel(): base()
        {
        }
        public ItemVariantViewModel(ItemVariant itemVariant): base(itemVariant)
        {
            Title = itemVariant.Title;
            Price = itemVariant.Price;
            ItemId = itemVariant.ItemId;
        }

        public override ItemVariant ToModel()
        {
            return new ItemVariant()
            {
                Id = Id,
                Price = Price,
                Title = Title,
                ItemId = ItemId
            };
        }
        public override ItemVariant UpdateModel(ItemVariant modelToUpdate)
        {
            modelToUpdate.Price = Price;
            modelToUpdate.Title = Title;
            return modelToUpdate;
        }
    }
}
