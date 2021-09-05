import {Component, OnDestroy, OnInit} from '@angular/core';
import {Profile, ProfileService} from "../../services/profile.service";
import {first} from "rxjs/operators";
import {BookService} from "../../services/book.service";
import {LibraryService} from "../../services/library.service";
import {StoredItem} from "../../shared/entities/storedItem.entity";

@Component({
  selector: 'app-moder-storage',
  templateUrl: './moder-storage.component.html',
  styleUrls: ['../moder-book/moder-book.component.scss', './moder-storage.component.scss']
})
export class ModerStorageComponent implements OnInit, OnDestroy {

  libraryId: string = "";
  storedItems: StoredItem[] = [];

  constructor(
    private profileService: ProfileService,
    private bookService: BookService,
    private libraryService: LibraryService
  ) { }

  ngOnInit(): void {
    this.profileService.profileSubject.pipe(first()).subscribe(
      library => {
        this.libraryId = library.registeredLibrary.id;
        this.libraryService.getStoredItemsObservable(this.libraryId).pipe(first()).subscribe(
          storedItems => {
            this.storedItems = storedItems;
            console.log(this.storedItems);
          }
        );
      }
    );
    this.profileService.getProfile();
  }

  ngOnDestroy(): void {
  }

  onDelete(itemId: string) {
    this.libraryService.deleteItemFromStore(this.libraryId, itemId).pipe(first()).subscribe(
      x => {
        this.storedItems = this.storedItems.filter( x => x.id != itemId)
      },
      err => {
        console.log(err);
      }
    )
  }

}
