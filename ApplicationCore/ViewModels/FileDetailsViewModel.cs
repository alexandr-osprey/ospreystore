﻿using ApplicationCore.Entities;
using ApplicationCore.ViewModels;

namespace ApplicationCore.ViewModels
{
    public abstract class FileDetailsViewModel<TFileDetails, TEntity> 
       : EntityViewModel<TFileDetails> 
        where TFileDetails: FileInfo<TEntity>, new() 
        where TEntity: Entity, new()
    {
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string OwnerUsername { get; set; }
        public int RelatedId { get; set; }

        public FileDetailsViewModel()
           : base()
        {
        }
        public FileDetailsViewModel(TFileDetails fileDetail)
           : base(fileDetail)
        {
            Title = fileDetail.Title;
            ContentType = fileDetail.ContentType;
            RelatedId = fileDetail.RelatedId;
            OwnerUsername = fileDetail.OwnerId;
        }

        public override TFileDetails UpdateModel(TFileDetails modelToUpdate)
        {
            modelToUpdate.Title = Title;
            //modelToUpdate.RelatedId = modelToUpdate.RelatedId;
            return modelToUpdate;
        }
    }
}