import { Component, OnInit } from '@angular/core';
import {BookService} from "../../services/book.service";
import {Book} from "../../shared/entities/book.entity";
import {first} from "rxjs/operators";
import { BookParameters } from 'src/app/shared/parameters/book-parameters';

@Component({
  selector: 'app-moder-book',
  templateUrl: './moder-book.component.html',
  styleUrls: ['./moder-book.component.scss']
})
export class ModerBookComponent implements OnInit {
  books: Book[] = [];
  pageSize: number = 8;
  pageNumber: number = 1;
  totalPages: number = 1;

  constructor(
    private bookSer: BookService
  ) { }

  ngOnInit(): void {
    this.changePage(1);
  }

  changePage(pageNumber: number) {
    this.pageNumber = pageNumber;
    let bookParams = new BookParameters();
    bookParams.pageSize = this.pageSize;
    bookParams.pageNumber = this.pageNumber;
    this.bookSer.getBooksObservable(bookParams).subscribe(
      resp => {
        this.books = resp.body;
        this.updatePagination(resp);
      }
    )
  }

  updatePagination(resp) {
    var pagesInfo = JSON.parse(resp.headers.get("X-Pagination"));
    this.totalPages = pagesInfo.TotalPages;
  }

  downloadFile(fileName, title) {
   this.bookSer.getFileObservable(fileName).pipe(first()).subscribe(
     blob => {
       var blob = new Blob([blob], {type: blob.type});

       var downloadURL = window.URL.createObjectURL(blob);
       var link = document.createElement('a');
       link.href = downloadURL;
       link.download = title;
       link.click();
     }
   )
  }


}