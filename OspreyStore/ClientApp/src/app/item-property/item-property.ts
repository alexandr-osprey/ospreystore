import { Entity } from "../models/entity/entity";

export class ItemProperty extends Entity {
  itemVariantId: number;
  characteristicValueId: number;

  public constructor(init?: Partial<ItemProperty>) {
    super(init);
    Object.assign(this, init);
  }
}
