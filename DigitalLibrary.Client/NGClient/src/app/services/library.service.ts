import { Injectable } from '@angular/core';
import {Library} from "../shared/entities/library.entity";
import {first, map} from "rxjs/operators";
import {Observable, Subject} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {StoredItem} from "../shared/entities/storedItem.entity";
import {Accounting} from "../shared/entities/accounting.entity";
import {accountingDto} from "../shared/entities/accounting.dto";
import {AccountingForUpdateDto} from "../shared/entities/accounting.updateDto";
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LibraryService {
  public librariesSubject: Subject<Library[]> = new Subject<Library[]>();

  constructor(
    private http: HttpClient
  ) { }

  private readonly _baseUrl = environment.apiUrl;

  getLibraries() {
    this.http.get<Library[]>(`${this._baseUrl}/api/library`).pipe(first()).subscribe(
      libraries => {
        this.librariesSubject.next(libraries);
      },
      error => {
        console.log(error);
      }
    )
  }

  getStoredItemsObservable(libraryId: string) {
    return this.http.get<StoredItem[]>(
        `${this._baseUrl}/api/library/${libraryId}/storage`
      );
  }

  addItemsToStore(libraryId: string, bookId: string, quantity = 1) {
    return this.http.post(
      `${this._baseUrl}/api/library/${libraryId}/storage/${bookId}?quantity=${quantity}`,
      {}
    )
  }

  deleteItemFromStore(libraryId: string, itemId: string) {
    return this.http.delete(
      `${this._baseUrl}/api/library/${libraryId}/storage/${itemId}`
    );
  }

  addAccounting(accounting: Accounting) {
    return this.http.post(
      `${this._baseUrl}/api/accounting`,
      accounting
    );
  }

  getAccountings(libraryId: string): Observable<accountingDto[]> {
    return this.http.get<accountingDto[]>(
      `${this._baseUrl}/api/accounting/library/${libraryId}`
    ).pipe(map(x => {
      x.forEach(element => {
        if (element.issueDate.toString() == '0001-01-01T00:00:00') {
          element.issueDate = null;
        }
        if (element.requestDate.toString() == '0001-01-01T00:00:00') {
          element.requestDate = null;
        }
        if (element.returnDate.toString() == '0001-01-01T00:00:00') {
          element.returnDate = null;
        }
        if (element.dateReturned.toString() == '0001-01-01T00:00:00') {
          element.dateReturned = null;
        }
      })
      return x;
    }));
  }

  getAccountingsForUser(): Observable<accountingDto[]> {
    return this.http.get<accountingDto[]>(
      `${this._baseUrl}/api/accounting/user`
    ).pipe(map(x => {
      x.forEach(element => {
        if (element.issueDate.toString() == '0001-01-01T00:00:00') {
          element.issueDate = null;
        }
        if (element.requestDate.toString() == '0001-01-01T00:00:00') {
          element.requestDate = null;
        }
        if (element.returnDate.toString() == '0001-01-01T00:00:00') {
          element.returnDate = null;
        }
        if (element.dateReturned.toString() == '0001-01-01T00:00:00') {
          element.dateReturned = null;
        }
      })
      return x;
    }));
  }

  updateStatusOfAccounting(accountingId: string, status: number) : Observable<accountingDto> {
    return this.http.patch<accountingDto>(
      `${this._baseUrl}/api/accounting`,
      {id: accountingId, status: status})
  }
}
