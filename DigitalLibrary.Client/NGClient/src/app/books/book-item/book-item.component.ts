import {Component, Input, OnInit} from '@angular/core';
import {Book} from "../../shared/entities/book.entity";

@Component({
  selector: 'app-book-item',
  templateUrl: './book-item.component.html',
  styleUrls: ['./book-item.component.scss']
})
export class BookItemComponent implements OnInit {
  @Input() layout: number;
  @Input() book: Book;

  constructor() { }

  ngOnInit(): void {
  }

}
