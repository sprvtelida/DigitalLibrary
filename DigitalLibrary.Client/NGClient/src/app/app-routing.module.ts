import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {HomeComponent} from "./home/home.component";
import {BooksComponent} from "./books/books.component";
import {ProfileComponent} from "./profile/profile.component";
import {BookDetailsComponent} from "./books/book-details/book-details.component";
import {ModerComponent} from "./moder/moder.component";
import {ModerBookComponent} from "./moder/moder-book/moder-book.component";
import {ModerStorageComponent} from "./moder/moder-storage/moder-storage.component";
import {ModerAccountingComponent} from "./moder/moder-accounting/moder-accounting.component";
import {ModerBookEditComponent} from "./moder/moder-book-edit/moder-book-edit.component";
import {ModerStorageAddComponent} from "./moder/moder-storage-add/moder-storage-add.component";
import {AboutComponent} from "./about/about.component";

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'books', component: BooksComponent},
  { path: 'books/:id', component: BookDetailsComponent },
  { path: 'profile', component: ProfileComponent },
  { path: 'about', component: AboutComponent },
  { path: 'mod', component: ModerComponent , children: [
      { path: 'moder-book', component: ModerBookComponent },
      { path: 'moder-book-edit', component: ModerBookEditComponent },
      { path: 'moder-book-edit/:bookId', component: ModerBookEditComponent },
      { path: 'moder-storage', component: ModerStorageComponent },
      { path: 'moder-storage-add', component: ModerStorageAddComponent },
      { path: 'moder-accounting', component: ModerAccountingComponent },
      { path: 'moder-accounting/:moderation', component: ModerAccountingComponent },
    ]},
  { path: 'moder-accounting', component: ModerAccountingComponent },
  { path: 'moder-accounting/:moderation', component: ModerAccountingComponent },
  { path: 'adm', component: BooksComponent },
  { path: '**', component:  HomeComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
