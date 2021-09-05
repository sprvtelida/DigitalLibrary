import {Component, Input, OnInit} from '@angular/core';
import {Book} from "../../shared/entities/book.entity";
import {BookService} from "../../services/book.service";
import {ActivatedRoute, Router} from "@angular/router";
import {first} from "rxjs/operators";
import {forkJoin} from "rxjs";
import {ProfileService} from "../../services/profile.service";
import {LibraryService} from "../../services/library.service";
import {Accounting} from "../../shared/entities/accounting.entity";
import {TranslateService} from "@ngx-translate/core";

@Component({
  selector: 'app-book-details',
  templateUrl: './book-details.component.html',
  styleUrls: ['./book-details.component.scss']
})
export class BookDetailsComponent implements OnInit {
  private _daysToReturn = 0;
  public _returnDate: Date;

  get returnDate() {
    return this._returnDate;
  }

  set returnDate(value) {
    let days = this.getDaysFromNow(value);
    if (days < 5) {
      this.validationMessage = 'You need to take book for at least 5 days.';
      this.formInvalid = true;
    } else {
      this.validationMessage = '';
      this.formInvalid = false;
      this._returnDate = value;
    }
  }

  public formInvalid = true;
  public validationMessage = '';
  public successMessage = '';

  get daysToReturn() {
    if (this.returnDate == null) {
      return 0;
    }
    return this.getDaysFromNow(this.returnDate).toFixed();
  }

  libraryId: string;
  book: Book;

  isAccountingMaking = true;

  constructor(
    private bookService: BookService,
    private router: Router,
    private route: ActivatedRoute,
    private profileService: ProfileService,
    private libraryService: LibraryService,
    private translateService: TranslateService
  ) { }

  addAccounting() {
    var accounting = new Accounting();
    accounting.libraryId = this.libraryId;
    accounting.bookId = this.book.id;
    accounting.returnDate = this.returnDate;

    console.log(accounting);
    this.libraryService.addAccounting(accounting).pipe(first()).subscribe(
      x => {
        this.successMessage = this.translateService.instant('BOOK.REQUEST_SENT');
      },
      error => {
        if (error.error == 'User already taken a book') {
          this.validationMessage = this.translateService.instant('BOOK.ALREADY_MADE_REQUEST');
          this.formInvalid = true;
        }
        if (error.error == 'Book is not available') {
          this.validationMessage = this.translateService.instant('BOOK.NOT_AVAILABLE');
          this.formInvalid = true;
        }
      }
    );
  }

  getDaysFromNow(toDate: Date) {
    let toDateMills = (new Date(toDate)).getTime();
    let mills = toDateMills - Date.now();
    let days = (mills / (1000*60*60*24)) + 1;
    return days;
  }

  downloadFile(fileName, title) {
    this.bookService.getFileObservable(fileName).pipe(first()).subscribe(
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

  ngOnInit(): void {
    let id = this.route.snapshot.params.id;

    this.profileService.getProfileObservable().subscribe(
      profile => {
        this.libraryId = profile.registeredLibrary.id;
        this.bookService.getBookObservable(id, profile.registeredLibrary.id).subscribe(
          book =>
        {
          this.book = book;
          console.log(this.book.isInStorage);
        });
      },
      error => {
        this.bookService.getBookObservable(id).subscribe(
          (book) => {
            this.book = book;
          }
        );
      }
    )
  }

}
