import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CharacteristicService } from 'src/app/services/characteristic.service';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { MessageService } from 'src/app/services/message.service';
import { CharacteristicValueService } from 'src/app/services/characteristic-value.service';
import { CharacteristicValue } from 'src/app/view-models/characteristic-value/characteristic-value';
import { Characteristic } from 'src/app/view-models/characteristic/characteristic';

@Component({
  selector: 'app-characteristic-value-create-update',
  templateUrl: './characteristic-value-create-update.component.html',
  styleUrls: ['./characteristic-value-create-update.component.css']
})
export class CharacteristicValueCreateUpdateComponent implements OnInit {
  protected characteristicValue: CharacteristicValue;
  protected characteristics: Characteristic[] = [];
  public isUpdating = false;
  @Output() characteristicValueSaved = new EventEmitter<CharacteristicValue>();

  constructor(
    private characteristicService: CharacteristicService,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private characteristicValueService: CharacteristicValueService,
    private location: Location) { }

  ngOnInit() {
    this.initializeComponent();
  }

  initializeComponent() {
    this.updateCharacteristics();
    let id = +this.route.snapshot.paramMap.get('id') || 0;
    if (id != 0) {
      this.isUpdating = true;
      this.characteristicValueService.get(id).subscribe(data => {
        if (data) {
          this.characteristicValue = new CharacteristicValue(data);
        }
      });
    } else {
      this.characteristicValue = new CharacteristicValue();
      this.isUpdating = false;
    }
  }

  updateCharacteristics() {
    this.characteristicService.index().subscribe(data => {
      if (data) {
        this.characteristics = [];
        data.entities.forEach(c => {
          this.characteristics.push(new Characteristic(c));
        });
      }
    });
  }

  createOrUpdate() {
    this.characteristicValueService.postOrPut(this.characteristicValue).subscribe(
      (data) => {
        if (data) {
          this.characteristicValue = new CharacteristicValue(data);
          this.messageService.sendSuccess("Characteristic value saved");
          this.characteristicValueSaved.emit(this.characteristicValue);
          if (this.characteristicValueSaved.observers.length == 0) {
            this.location.back();
          }
        }
      });
  }

}