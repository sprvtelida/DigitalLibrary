import {Subject} from "./subject.entity";
import {Genre} from "./genre.entity";
import {Author} from "./author.entity";
import {Publisher} from "./publisher.entity";

export class Book {
  id: string;
  title: string;
  description: string;
  image: string;
  isbn: string;
  pages: number;
  year: number;
  language: string;

  epub: string;
  pdf: string;
  fb2: string;

  subject: Subject;
  genre: Genre;
  author: Author;
  publisher: Publisher;

  isInStorage: boolean;
}
