import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {Book} from "../shared/entities/book.entity";
import {BookService} from "../services/book.service";
import {first} from "rxjs/operators";
import {Genre} from "../shared/entities/genre.entity";
import {Subject} from "../shared/entities/subject.entity";
import {FormArray, FormBuilder, FormControl, FormGroup} from "@angular/forms";
import {Subscription} from "rxjs";
import {forkJoin} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {ProfileService} from "../services/profile.service";
import {Router} from "@angular/router";
import { BookParameters } from '../shared/parameters/book-parameters';

@Component({
  selector: 'app-books',
  templateUrl: './books.component.html',
  styleUrls: ['./books.component.scss']
})
export class BooksComponent implements OnInit, OnDestroy {
  sub: Subscription;

  currentPage = 1;
  totalPages = 1;
  itemsPerPage = 18;
  itemsPerPageToggle: number = 4;
  subjectToggle = false;
  genreToggle = false;
  minPage:number;
  maxPage:number;
  minYear:number;
  maxYear:number;
  onlyInStorage: boolean = false;
  sortBy = "Title";
  layout = 0;

  libraryId: string;

  searchField = "Title";
  searchText = "";

  books: Book[] = [];
  genres: Genre[] = [];
  subjects: Subject[] = [];

  private fb: FormBuilder = new FormBuilder();
  public form: FormGroup;

  get genresArray(): FormArray {
        return <FormArray>this.form.get('genres');
  }

  get subjectsArray(): FormArray {
        return <FormArray>this.form.get('subjects');
  }

  selectedGenreIds = [];
  selectedSubjectIds = [];

  constructor(
    private bookService: BookService,
    private http: HttpClient,
    private profileService: ProfileService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
        genres: this.fb.array([]),
        subjects: this.fb.array([])
    });

    this.sub = forkJoin([this.bookService.getGenresObservable(), this.bookService.getSubjectsObservable()]).subscribe(
      (res) => {
        this.profileService.getProfileObservable().pipe(first()).subscribe(
          profile => {
            if (profile) {
              this.libraryId = (profile).registeredLibrary.id;
            }
            this.UpdateBooks();
        });

        this.UpdateBooks();
        this.genres = res[0];
        this.subjects = res[1];
        
        this.form = this.fb.group({
          genres: [this.addCheckBoxArrayControl(this.genres)],
          subjects: [this.addCheckBoxArrayControl(this.subjects)]
        });
      }
    );
  }

  navigate(id: string) {
    this.router.navigate(['books', id]);
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  getQueryParamOutOfControlArray(formArray: FormArray, array: Array<Subject>|Array<Genre>): string[] {
    var resultArray = [];
    formArray.value.controls.forEach((control, i) => {
      if (control.value) {
        resultArray.push(array[i].id);
      }
    });
    return resultArray;
  }

  addCheckBoxArrayControl(array: Array<any>) {
    const controlArray = array.map(element => {
      return this.fb.control(false);
    })
    return this.fb.array(controlArray);
  }

  search() {
    this.selectedGenreIds = this.getQueryParamOutOfControlArray(this.genresArray, this.genres);
    this.selectedSubjectIds = this.getQueryParamOutOfControlArray(this.subjectsArray, this.subjects);
    this.UpdateBooks();
  }

  changeSearchField(field: string) {
    this.searchField = field;
  }

  changePage(page: number) {
    this.currentPage = page;
    this.UpdateBooks();
  }

  changeLayout(layout: number) {
    this.layout = layout;
    if (this.layout == 1) {
      this.itemsPerPage = 4;
      this.itemsPerPageSelected();
    } else {
      this.itemsPerPage = 18;
      this.itemsPerPageSelected();
    }
  }

  itemsPerPageSelected() {
    if (this.itemsPerPage != this.itemsPerPageToggle) {
      this.itemsPerPageToggle = this.itemsPerPage;
      this.currentPage = 1;
      this.UpdateBooks();
    }
  }

  sortBySelected() {
    this.UpdateBooks();
  }

  UpdateBooks() {
    let parameters = new BookParameters(this.itemsPerPage, this.selectedGenreIds, this.selectedSubjectIds, this.searchText, this.minYear, this.maxYear, this.minPage, this.maxPage, this.onlyInStorage, this.sortBy, this.searchField);
    parameters.pageNumber = this.currentPage;
    parameters.onlyInStorage = this.onlyInStorage;
    parameters.libraryId = this.libraryId;

    this.bookService.getBooksObservable(parameters).pipe(first()).subscribe(resp => {
      this.books = <Book[]>resp.body;
      console.log(this.books);
      let pagesInfo = JSON.parse(resp.headers.get("X-Pagination"));
      this.totalPages = pagesInfo.TotalPages;
    }, err => {
        this.books = [];
    });
  }
}