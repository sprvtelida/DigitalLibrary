import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Route} from "@angular/router";
import {FormArray, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {Book} from "../../shared/entities/book.entity";
import {BookService} from "../../services/book.service";
import {first} from "rxjs/operators";
import {Genre} from "../../shared/entities/genre.entity";
import {Subject} from "../../shared/entities/subject.entity";
import {Author} from "../../shared/entities/author.entity";
import {Publisher} from "../../shared/entities/publisher.entity";

@Component({
  selector: 'app-moder-book-edit',
  templateUrl: './moder-book-edit.component.html',
  styleUrls: ['./moder-book-edit.component.scss']
})
export class ModerBookEditComponent implements OnInit {
  testRadio = "";
  bookId = null;

  _fb: FormBuilder = new FormBuilder();
  form: FormGroup = this._fb.group({
    title: this._fb.control(null, [Validators.required]),
    description: this._fb.control(null, Validators.required),
    isbn: this._fb.control(null, Validators.required),
    pages: this._fb.control(null, Validators.required),
    year: this._fb.control(null, Validators.required),
    language: this._fb.control(null, Validators.required),
  });

  fileData: FormData = new FormData();

  isEditing = false;

  classToggle = {
    genre: false,
    subject: false,
    author: false,
    publisher: false
  }

  selected = {
    genre: {
      id: null,
      text: 'Select genre'
    },
    subject: {
      id: null,
      text: 'Select subject'
    },
    author: {
      id: null,
      text: 'Select author'
    },
    publisher: {
      id: null,
      text: 'Select publisher'
    },
  }

  collection = {
    genres: new Array<Genre>(),
    subjects: new Array<Subject>(),
    authors: new Array<Author>(),
    publishers: new Array<Publisher>()
  }

  constructor(
    private route: ActivatedRoute,
    private bookSer: BookService
  ) {
  }


  ngOnInit(): void {
    console.log(this.route.snapshot);
    if (this.route.snapshot.params.bookId === (null || undefined)) {
      this.isEditing = false;
    } else {
      this.isEditing = true;
      this.bookId = this.route.snapshot.params.bookId;
      this.bookSer.getBookObservable(this.route.snapshot.params.bookId).pipe(first()).subscribe(
        book => {
          this.initializeForm(book);
          if (book.genre) {
            this.selected.genre.text = book.genre.title;
            this.selected.genre.id = book.genre.id;
          }
          if (book.subject) {
            this.selected.subject.text = book.subject.title;
            this.selected.subject.id = book.subject.id;
          }
          if (book.author) {
            this.selected.author.text = book.author.firstName + " " + book.author.lastName;
            this.selected.author.id = book.author.id;
          }
          if (book.publisher) {
            this.selected.publisher.text = book.publisher.name;
            this.selected.publisher.id = book.publisher.id;
          }
        },
        error => {
          console.log(error);
        }
      )
    }

    this.bookSer.getGenresObservable().pipe(first()).subscribe(
      genres => {
        this.collection.genres = genres;
      }, err => {
        console.log(err);
      }
    );

    this.bookSer.getSubjectsObservable().pipe(first()).subscribe(
      subjects => {
        this.collection.subjects = subjects;
      }, err => {
        console.log(err)
      }
    );

    this.bookSer.getAuthorsObservable().pipe(first()).subscribe(
      authors => {
        this.collection.authors = authors;
      }, err => {
        console.log(err)
      }
    );

    this.bookSer.getPublisherObservable().pipe(first()).subscribe(
      publisher => {
        this.collection.publishers = publisher;
      }, err => {
        console.log(err)
      }
    );
  }

  initializeForm(book: Book) {
    console.log("initializing Form");
    this.form.patchValue({
      title: book.title,
      description: book.description,
      isbn: book.isbn,
      pages: book.pages,
      year: book.year,
      language: book.language
    })
  }


  Selected(event) {
    let id = event.target.id.split('.')[1];
    let name = event.target.name;

    switch (name) {
      case 'genre':
        this.selected.genre.text = this.collection.genres[id].title;
        this.selected.genre.id = this.collection.genres[id].id;
        this.classToggle.genre = false;
        break;
      case 'subject':
        this.selected.subject.text = this.collection.subjects[id].title;
        this.selected.subject.id = this.collection.subjects[id].id;
        this.classToggle.subject = false;
        break;
      case 'author':
        this.selected.author.text = this.collection.authors[id].firstName + " " + this.collection.authors[id].lastName;
        this.selected.author.id = this.collection.authors[id].id;
        this.classToggle.author = false;
        break;
      case 'publisher':
        this.selected.publisher.text = this.collection.publishers[id].name;
        this.selected.publisher.id = this.collection.publishers[id].id;
        this.classToggle.publisher = false;
        break;
      default:
        console.log('nothing');
    }
    console.log(this.selected);
  }

  onImageSelected(event) {
      if (event.target.files.length > 0) {
        const file = <File>event.target.files[0];
        this.fileData.append(file.name, file, file.name);
      }
  }

  onFilesSelected(event) {
      if (event.target.files.length > 0) {
        const files = <File[]>event.target.files;
        for (var file of files) {
          this.fileData.append(file.name, file, file.name);
        }
      }
  }

  onSubmit() {
    console.log("OnSubmit()")
    var book = {
      id: '',
      title: this.form.get('title').value,
      description: this.form.get('description').value,
      isbn: this.form.get('isbn').value,
      pages: this.form.get('pages').value,
      year: this.form.get('year').value,
      language: this.form.get('language').value,
      authorId: this.selected.author.id?this.selected.author.id:null,
      publisherId: this.selected.publisher.id?this.selected.publisher.id:null,
      genreId: this.selected.genre.id?this.selected.genre.id:null,
      subjectId: this.selected.subject.id?this.selected.subject.id:null,
    };
    console.log(book);

    if (this.isEditing) {
      book.id = this.bookId;
      this.bookSer.addFiles(this.fileData, book.id).pipe(first()).subscribe(
        msg => {
          this.bookSer.updateBookObservable(book).subscribe(
            x => {
            }
          );
        }
      );
    } else {
      this.bookSer.addBookObservable(book).pipe(first()).subscribe(
        book => {
          console.log("BOOK");
          console.log(book);
          this.bookSer.addFiles(this.fileData, book.id).pipe(first()).subscribe(
            msg => {
              console.log(msg);
            }
          );
        }
      );
    }
  }
}
