import { Entity } from "../models/entity/entity";

export enum OrderStatus {
  Created,
  Finished,
  Cancelled
}

export class Order extends Entity {
  storeId: number;
  dateTimeCreated: Date;
  dateTimeFinished: Date;
  dateTimeCancelled: Date;
  status: OrderStatus;
  sum: number;
}
