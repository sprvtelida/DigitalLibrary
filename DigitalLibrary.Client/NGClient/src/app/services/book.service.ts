import { Injectable } from '@angular/core';
import {HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { Book } from "../shared/entities/book.entity";
import { map } from "rxjs/operators";
import { Genre } from "../shared/entities/genre.entity";
import { Subject } from "../shared/entities/subject.entity";
import { Author } from "../shared/entities/author.entity";
import { Publisher} from "../shared/entities/publisher.entity";
import { BookParameters } from '../shared/parameters/book-parameters';
import { environment } from 'src/environments/environment';


@Injectable({
  providedIn: 'root'
})
export class BookService {

  private _baseUrl = `${environment.apiUrl}`;
  private _apiUrl = `${environment.apiUrl}/api/books`;

  constructor(
    private http: HttpClient
  ) { }

  getBooksObservable(bookParameters: BookParameters) {
    let params = this.generateHttpParams(bookParameters);
    console.log(params);
    return this.http.get<{body: Book[], headers: {headers: any}}>(
            this._apiUrl,
            {observe: 'response', params}
      ).pipe(
      map((response: any) => {
        return response;
      })
    );
  }

  private  generateHttpParams(data: object): HttpParams {
      let params = new HttpParams();
      for (const key of Object.keys(data)) {
        if (data[key]) {
          if (data[key] instanceof Array) {
            data[key].forEach((item) => {
              params = params.append(`${key.toString()}`, item);
            });
          }
          else {
            params = params.append(key.toString(), data[key]);
          }
        }
      }
      return params;
  }

  getBookObservable(id: string, libraryId?: string) {
    var query = libraryId?("?libraryId=" + libraryId) : "";
    return this.http.get<Book>(`${this._apiUrl}/${id}${query}`);
  }

  getGenresObservable() {
    return this.http.get<Genre[]>(
      `${this._apiUrl}/genreCollection`
    );
  }

  getSubjectsObservable() {
    return this.http.get<Subject[]>(
      `${this._apiUrl}/subjectCollection`
    );
  }

  getAuthorsObservable() {
    return this.http.get<Author[]>(
      `${this._baseUrl}/api/authors`
    );
  }

  getPublisherObservable() {
    return this.http.get<Publisher[]>(
      `${this._baseUrl}/api/publishers`
    );
  }

  addFiles(fd: FormData, bookId: string) {
    return this.http.post(
      `${this._apiUrl}/files`, fd, {
        headers: {bookId}
      }
    );
  }

  addBookObservable(book: {title:string, description:string, isbn:string, pages:number, year:number, language:number}) {
    return this.http.post<Book>(
      `${this._apiUrl}`,
       book
    );
  }

  updateBookObservable(book: {id:string, title:string, description:string, isbn:string, pages:number, year:number, language:number}) {
    return this.http.put(
     `${this._apiUrl}`,
      book
    );
  }

  deleteBookObservable(id: string) {
    return this.http.delete(
      `${this._apiUrl}/${id}`
    );
  }

  getFileObservable(name: string): Observable<Blob> {
    return this.http.get(
      `${this._apiUrl}/file?fileName=${name}`, { responseType: 'blob' }
    );
  }
}