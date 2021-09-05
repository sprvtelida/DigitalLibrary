import {Book} from "./book.entity";
import {Accounting} from "./accounting.entity";

export class StoredItem {
  id: string;
  arrivalDate: Date;
  inventoryNumber: number;
  book: Book;
  accounting: Accounting;
}
