import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './header/header.component';

import {HTTP_INTERCEPTORS, HttpClient, HttpClientModule} from "@angular/common/http";
import { OAuthModule } from "angular-oauth2-oidc";
import { CookieService } from "ngx-cookie-service";
import { FooterComponent } from './footer/footer.component';
import { HomeComponent } from './home/home.component';
import { BooksComponent } from './books/books.component';
import { ProfileComponent } from './profile/profile.component';

import { authCodeFlowConfig } from './config/sso';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import { BookItemComponent } from './books/book-item/book-item.component';
import { BookDetailsComponent } from './books/book-details/book-details.component';
import {AuthTokenInterceptor} from "./interceptors/auth-token.interceptor";
import { ModerComponent } from './moder/moder.component';
import { ModerBookComponent } from './moder/moder-book/moder-book.component';
import { ModerStorageComponent } from './moder/moder-storage/moder-storage.component';
import { ModerAccountingComponent } from './moder/moder-accounting/moder-accounting.component';
import { ModerBookEditComponent } from './moder/moder-book-edit/moder-book-edit.component';
import { ModerStorageAddComponent } from "./moder/moder-storage-add/moder-storage-add.component";
import {TranslateLoader, TranslateModule} from "@ngx-translate/core";
import {TranslateHttpLoader} from "@ngx-translate/http-loader";
import { AboutComponent } from './about/about.component';

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    HomeComponent,
    BooksComponent,
    ProfileComponent,
    BookItemComponent,
    BookDetailsComponent,
    ModerComponent,
    ModerBookComponent,
    ModerStorageComponent,
    ModerAccountingComponent,
    ModerBookEditComponent,
    ModerStorageAddComponent,
    AboutComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    OAuthModule.forRoot(
      {
      resourceServer: {
        allowedUrls: [
          authCodeFlowConfig.issuer
        ],
        sendAccessToken: true,
      }
    }
    )
  ],
  providers: [
    CookieService,
    HttpClient
    //{ provide: HTTP_INTERCEPTORS, useClass: AuthTokenInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
