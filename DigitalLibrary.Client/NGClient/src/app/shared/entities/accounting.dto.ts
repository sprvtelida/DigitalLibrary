import {Library} from "./library.entity";
import {Book} from "./book.entity";
import {StoredItem} from "./storedItem.entity";
import { Profile } from "../../services/profile.service";

export class accountingDto {
  id: string;
  status: string;
  requestDate: Date;
  issueDate: Date;
  returnDate: Date;
  dateReturned: Date;
  library: Library;
  book: Book;
  storage: StoredItem;
  profile: Profile;
}
