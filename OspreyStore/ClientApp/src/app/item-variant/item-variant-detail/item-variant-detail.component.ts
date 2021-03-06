import { Component, OnInit, Input, Output, EventEmitter, OnChanges } from '@angular/core';
import { ParameterService } from '../../parameter/parameter.service';
import { ParameterNames } from '../../parameter/parameter-names';
import { ItemDetail } from 'src/app/item/item-detail';
import { ItemVariant } from '../item-variant';

@Component({
  selector: 'app-item-variant-detail',
  templateUrl: './item-variant-detail.component.html',
  styleUrls: ['./item-variant-detail.component.css']
})
export class ItemVariantDetailComponent implements OnInit, OnChanges {
  @Input() itemDetail: ItemDetail;
  public currentVariant: ItemVariant;
  @Output() itemVariantSelected = new EventEmitter<ItemVariant>();

  constructor(private parameterService: ParameterService) {
  }

  ngOnInit() {

  }

  ngOnChanges() {
    let itemVariantId = +this.parameterService.getParam(ParameterNames.itemVariantId);
    if (this.itemDetail && this.itemDetail.itemVariants) {
      if (this.itemDetail.itemVariants[0]) {
        if (itemVariantId) {
          let itemVariant = this.itemDetail.itemVariants.find(v => v.id == itemVariantId);
          if (itemVariant) {
            this.selectItemVariant(itemVariant);
          }
        } else {
          let firstItemVariant = this.itemDetail.itemVariants[0];
          this.selectItemVariant(firstItemVariant);
        }
      }
    }
  }

  selectItemVariant(itemVariant: ItemVariant) {
    this.currentVariant = this.itemDetail.itemVariants.find(v => v == itemVariant);
    this.parameterService.navigateWithUpdatedParams({ "itemVariantId": itemVariant.id });
    this.itemVariantSelected.emit(itemVariant);
  }
}
