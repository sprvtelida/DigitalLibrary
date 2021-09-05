import {Component, OnInit, Output} from '@angular/core';
import {OAuthService} from "angular-oauth2-oidc";
import {authCodeFlowConfig} from "./config/sso";
import {ProfileService} from "./services/profile.service";
import jwtDecode, {JwtPayload} from "jwt-decode";
import {LibraryService} from "./services/library.service";
import {TranslateService} from "@ngx-translate/core";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'NGClient';

  constructor(private oauthService: OAuthService,
              private profileService: ProfileService,
              private libService: LibraryService,
              private translateService: TranslateService) {
    this.translateService.setDefaultLang('en');
    this.translateService.use(localStorage.getItem('lang') || 'en');
  }

  ngOnInit(): void {
    this.oauthService.configure(authCodeFlowConfig);
    this.oauthService.loadDiscoveryDocumentAndTryLogin().then(result => {
      if (result) {
        this.oauthService.setupAutomaticSilentRefresh();
        if(this.oauthService.hasValidAccessToken()) {
          this.profileService.getProfile();
          this.profileService.setIsAuthorized(true);

          var decodedToken: {role} = jwtDecode(this.oauthService.getAccessToken());
          if (decodedToken.role != null)
            this.profileService.roleSubject.next(decodedToken.role);
        }
      }
    });
  }
}
