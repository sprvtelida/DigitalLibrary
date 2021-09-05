import { Component, OnInit } from '@angular/core';
import { accountingDto } from "../../shared/entities/accounting.dto";
import {LibraryService} from "../../services/library.service";
import {ProfileService} from "../../services/profile.service";
import {first} from "rxjs/operators";
import {AccountingForUpdateDto} from "../../shared/entities/accounting.updateDto";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-moder-accounting',
  templateUrl: './moder-accounting.component.html',
  styleUrls: ['../moder-book/moder-book.component.scss', './moder-accounting.component.scss']
})
export class ModerAccountingComponent implements OnInit {

  accountings: accountingDto[];
  libraryId: string;
  isModeration: boolean;

  constructor(
    private libraryService: LibraryService,
    private profileService: ProfileService,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    if (this.route.snapshot.params.moderation === (null || undefined)) {
      this.isModeration = false;
    } else {
      this.isModeration = true;
    }

    if (this.isModeration) {
      this.profileService.profileSubject.pipe(first()).subscribe(
        profile => {
          this.libraryId = profile.registeredLibrary.id;
          this.libraryService.getAccountings(this.libraryId).pipe(first()).subscribe(
            accountings => {
              this.accountings = accountings;
              console.log(this.accountings);
            },
            error => {
              console.log(error);
            }
          );
        },
        error => {
          console.log(error);
        }
      );
      this.profileService.getProfile();
    }

    // else
    if (!this.isModeration) {
      this.libraryService.getAccountingsForUser().pipe(first()).subscribe(
        accountings => {
          this.accountings = accountings;
        }
      );
    }
  }

  updateStatusOfAccounting(accountingId:string, status:number) {
    console.log(accountingId);
    this.libraryService.updateStatusOfAccounting(accountingId, status).subscribe(
      x => {
        this.accountings = this.accountings.filter(element => {
          if (element.id != x.id) {
            return element;
          }
          element.status = x.status;
          if (element.issueDate?.toString() == '0001-01-01T00:00:00')
          {
            element.issueDate = null;
          }
          if (element.issueDate?.toString() == '0001-01-01T00:00:00')
          {
            element.dateReturned = null;
          }
          return element;
        })
      }, error => {
        console.log(error);
      }
    )
  }
}
