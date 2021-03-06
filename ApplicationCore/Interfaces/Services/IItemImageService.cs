﻿using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces.Services
{
    /// <summary>
    /// Maintains full lifecycle of ItemVariantImage entities
    /// </summary>
    public interface IItemVariantImageService: IImageService<ItemVariantImage, ItemVariant>
    {
    }
}
