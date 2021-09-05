import { Component, OnInit } from '@angular/core';
import {BookService} from "../../services/book.service";
import {first} from "rxjs/operators";
import {Book} from "../../shared/entities/book.entity";
import {LibraryService} from "../../services/library.service";
import {ProfileService} from "../../services/profile.service";
import {Router} from "@angular/router";
import { BookParameters } from 'src/app/shared/parameters/book-parameters';

@Component({
  selector: 'app-moder-storage-add',
  templateUrl: './moder-storage-add.component.html',
  styleUrls: ['../moder-book/moder-book.component.scss', './moder-storage-add.component.scss']
})
export class ModerStorageAddComponent implements OnInit {

  public searchTerm: string;
  private books: Book[] = [];
  public isSearching = false;
  private quantity: number = 0;
  public selectedBook: Book;

  private pageNumber = 1;
  private totalPages = 0;
  private pageSize = 5;

  private libraryId: string;

  constructor(
    private bookService: BookService,
    private profileService: ProfileService,
    private libraryService: LibraryService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.profileService.profileSubject.pipe(first()).subscribe(
      profile => {
        this.libraryId = profile.registeredLibrary.id;
        console.log("libID", this.libraryId);
      }
    );
    this.profileService.getProfile();
  }

  searchBooks() {
    let params = new BookParameters();
    params.pageSize = this.pageSize;
    params.searchTerm = this.searchTerm;
    params.pageNumber = 1;
    this.bookService.getBooksObservable(params).pipe(first()).subscribe(
      resp => {
        this.updatePagination(resp);
        this.books = resp.body;
        this.isSearching = true;
      }
    );

    this.pageNumber = 1;
  };

  updatePagination(resp) {
    var resp = JSON.parse(resp.headers.get("X-Pagination"));
    this.totalPages = resp.TotalPages;
  }

  onSelect(book: Book) {
    this.selectedBook = book;
  }

  increment() {
    this.quantity++;
  }

  decrement() {
    this.quantity--;
  }

  changePage(pageNumber: number) {
    this.pageNumber = pageNumber;
    
    let params = new BookParameters();
    params.pageSize = this.pageSize;
    params.pageNumber = this.pageNumber;

    this.bookService.getBooksObservable(params).subscribe(
      resp => {
        this.books = resp.body;
      }
    )
  }

  addItems() {
    this.libraryService.addItemsToStore(this.libraryId, this.selectedBook.id, this.quantity).pipe(first()).subscribe(
      x => {
        this.router.navigate(['mod', 'moder-storage']);
      },
      error => {
        console.log(error);
      });
  }


}