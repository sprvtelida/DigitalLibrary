import {Component, Input, OnChanges, OnDestroy, OnInit} from '@angular/core';
import {OAuthService} from "angular-oauth2-oidc";
import {CookieService} from "ngx-cookie-service";
import {ProfileService, Profile} from "../services/profile.service";
import {Observable, Subscription} from "rxjs";
import {first} from "rxjs/operators";
import {TranslateService} from "@ngx-translate/core";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit, OnDestroy {

  lang;

  private profile: Profile = new Profile();
  public isAuthorized: boolean = false;
  public role: string = null;
  private profileSub: Subscription;
  private isAuthorizedSub: Subscription;
  private roleSub: Subscription;
  isImageLoading: boolean;
  imageToShow: string | ArrayBuffer = null;

  constructor(private oauthService: OAuthService,
              private cookieService: CookieService,
              private profileService: ProfileService,
              private translateService: TranslateService) {
  }


  login() {
    this.oauthService.initCodeFlow();
  }

  logout() {
    this.oauthService.logOut();
  }

  createImageFromBlob(image: Blob) {
    let reader = new FileReader();
    reader.addEventListener("load", () => {
      this.imageToShow = reader.result;
    }, false);

    if (image) {
      reader.readAsDataURL(image);
    }
  }

  getImageFromService() {
    this.isImageLoading = true;
    this.profileService.downloadImage().pipe(first()).subscribe(data => {
      this.createImageFromBlob(data);
      this.isImageLoading = false;
    }, error => {
      this.isImageLoading = false;
      console.log(error);
    });
  }

  changeLang(lang) {
    this.cookieService.set(".AspNetCore.Culture", `c=${lang}|uic=${lang}`);
    localStorage.setItem('lang', lang);
    this.translateService.use(lang);
    //window.location.reload();
  }

  ngOnInit(): void {
    this.lang = localStorage.getItem('lang') || 'en';

    console.log(this.role);
    this.getImageFromService();

    this.profileSub = this.profileService.profileSubject.subscribe(profile => {
      this.profile = profile;
    });
    this.isAuthorizedSub = this.profileService.isAuthorizedSubject.subscribe(isAuth => {
      this.isAuthorized = isAuth;
    });
    this.roleSub = this.profileService.roleSubject.subscribe(role => {
      this.role = role;
    });
  }

  ngOnDestroy(): void {
    this.profileSub.unsubscribe();
    this.isAuthorizedSub.unsubscribe();
    this.roleSub.unsubscribe();
  }
}
