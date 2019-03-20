import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { ItemImage } from '../item-image';
import { Item } from 'src/app/item/item';
import { ItemImageService } from '../item-image.service';
import { MessageService } from 'src/app/message/message.service';
import { EntityIndex } from 'src/app/models/entity/entity-index';

@Component({
  selector: 'app-item-image-create-update',
  templateUrl: './item-image-create-update.component.html',
  styleUrls: ['./item-image-create-update.component.css']
})
export class ItemImageCreateUpdateComponent implements OnInit {
  itemImageIndexIndexSubscription: Subscription;
  itemImageSubscription: Subscription;
  itemImageFormData: FormData;
  itemImages: ItemImage[];
  @Input() item: Item;


  constructor(
    private itemImageService: ItemImageService,
    private messageService: MessageService) {
    this.itemImages = [];
  }

  ngOnInit() {
    this.initializeComponent()
  }

  initializeComponent() {
    if (this.item.id) {
      this.itemImageService.index({ itemId: this.item.id }).subscribe((itemImageIndex: EntityIndex<ItemImage>) => {
        itemImageIndex.entities.forEach(i => this.itemImages.push(new ItemImage(i)));
      });
    }
  }
  saveItemImage() {
    this.itemImageService.postFile(this.itemImageFormData, this.item.id).subscribe((data: ItemImage) => {
      if (data) {
        this.itemImages.push(new ItemImage(data));
        this.messageService.sendSuccess("Item image saved");
      }
    });
  }
  updateItemImage(itemImage: ItemImage) {
    let i = this.itemImages.indexOf(itemImage);
    this.itemImageService.postOrPut(itemImage).subscribe((data: ItemImage) => {
      if (data) {
        this.itemImages[i] = new ItemImage(data);
        this.messageService.sendSuccess("Item image updated");
      }
    });;
  }
  getImageUrl(itemImage: ItemImage): string {
    return this.itemImageService.getImageUrl(itemImage.id)
  }

  deleteItemImage(itemImage: ItemImage) {
    if (itemImage.id) {
      this.itemImageService.delete(itemImage.id).subscribe((data) => {
        if (data) {
          this.itemImages = this.itemImages.filter(i => i != itemImage);
          this.messageService.sendSuccess("Item image deleted");
        }
      })
    }
  }

  onFileChanged(event) {
    let selectedFile = event.target.files[0];
    this.itemImageFormData = new FormData();
    this.itemImageFormData.append('formFile', selectedFile, selectedFile.name);
  }
}