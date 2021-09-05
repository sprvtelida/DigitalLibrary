import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from "../../environments/environment";

export const authCodeFlowConfig: AuthConfig = {
  // Url of the Identity Provider
  issuer: environment.apiUrl,

  redirectUri: environment.clientUrl,
  postLogoutRedirectUri: environment.clientUrl,

  clientId: environment.clientId,

  responseType: 'code',

  scope: 'openid profile email offline_access IdentityServerApi administration moderation',

  showDebugInformation: true,
};
