import { Component, OnInit, Input } from '@angular/core';
import { Item } from '../../view-models/item';
import { ItemVariant } from '../../view-models/item-variant';
import { ItemVariantService } from '../../services/item-variant.service';
import { MessageService } from '../../services/message.service';
import { Subscription } from 'rxjs';
import { EntityIndex } from '../../view-models/entity-index';


@Component({
  selector: 'app-item-variant-create-update',
  templateUrl: './item-variant-create-update.component.html',
  styleUrls: ['./item-variant-create-update.component.css']
})
export class ItemVariantCreateUpdateComponent implements OnInit {
  itemVariantIndexSubsription: Subscription;
  itemVariantSubscription: Subscription;
  @Input() item: Item;
  public currentVariant: ItemVariant;
  public itemVariants: ItemVariant[];
  public isUpdating = false;

  constructor(
    private messageService: MessageService,
    private itemVariantService: ItemVariantService) {
  }

  ngOnInit() {
    this.initializeComponent();
  }

  initializeComponent() {
    if (this.item.id) {
      this.isUpdating = true;
      this.itemVariants = [];
      this.currentVariant = this.itemVariants[0];
      this.itemVariantService.index({ itemId: this.item.id }).subscribe((itemVariantIndex: EntityIndex<ItemVariant>) => {
        itemVariantIndex.entities.forEach(v => this.itemVariants.push(new ItemVariant(v)));
        this.selectItemVariant(this.itemVariants[0]);
      });
    }
  }
  addItemVariant(): ItemVariant {
    let newVariant = new ItemVariant({
      id: 0,
      title: "new variant",
      price: 0,
      itemId: this.item.id
    });
    this.itemVariants.push(newVariant);
    return newVariant;
  }
  selectItemVariant(itemVariant: ItemVariant) {
    if (this.itemVariants) {
      this.currentVariant = itemVariant;
    }
  }

  saveItemVariant() {
    let i: number = this.itemVariants.indexOf(this.currentVariant);
    this.itemVariantService.postOrPut(this.currentVariant).subscribe((data: ItemVariant) => {
      if (data) {
        this.itemVariants[i] = new ItemVariant(data);
        this.currentVariant = this.itemVariants[i];
        this.messageService.sendSuccess("Item variant saved");
      }
      });
  }
  deleteItemVariant() {
    if (this.currentVariant.id) {
      this.itemVariantService.delete(this.currentVariant.id).subscribe((data: any) => {
        if (data) {
          this.itemVariants = this.itemVariants.filter(v => v != this.currentVariant);
          this.messageService.sendSuccess("Item variant deleted");
          if (this.itemVariants[0])
            this.currentVariant = this.itemVariants[0];
          else
            this.currentVariant = this.addItemVariant();
        }
      });
    }
  }

  getUpdatedParams(paramName: string, param: number): any {
    var params: any = {};
    params[paramName] = param;
    return params;
  }
}
