﻿using ApplicationCore.Entities;

namespace OctopusStore.ViewModels
{
    public abstract class ImageDetailViewModel<TImageDetails, TEntity>
        : FileDetailsDetailViewModel<TImageDetails, TEntity>
        where TImageDetails : FileInfo<TEntity>, new()
        where TEntity : Entity, new()
    {
        public ImageDetailViewModel(TImageDetails image)
            : base(image)
        { }
    }
}
