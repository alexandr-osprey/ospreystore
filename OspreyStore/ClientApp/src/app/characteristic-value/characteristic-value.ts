import { Entity } from "../models/entity/entity";

export class CharacteristicValue extends Entity {
  title: string;
  characteristicId: number;

  public constructor(init?: Partial<CharacteristicValue>) {
    super(init);
    Object.assign(this, init);
  }
}
