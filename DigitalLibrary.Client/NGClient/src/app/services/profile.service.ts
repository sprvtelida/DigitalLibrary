import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, Observable, Observer, Subject} from "rxjs";
import {first, take} from "rxjs/operators";
import {Library} from "../shared/entities/library.entity";
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  public profileSubject: Subject<Profile> = new Subject<Profile>();
  public isAuthorizedSubject: Subject<boolean> = new Subject<boolean>();
  public roleSubject: Subject<string> = new Subject<string>();
  public imageSubject: Subject<any> = new Subject<any>();
  public image: any;


  constructor(
    private httpClient: HttpClient
  ) { }

  private readonly _apiUrl = `${environment.apiUrl}/api/profile`;

  getProfile() {
    this.httpClient.get<Profile>(this._apiUrl)
      .pipe(take(1))
      .subscribe(profile => {
        this.profileSubject.next(profile);
      }, err => {
        console.log(err);
      });

    this.downloadImage();
  }

  getProfileObservable() {
    return this.httpClient.get<Profile>(this._apiUrl);
  }

  createProfile(profile: Profile) {
    return this.httpClient.post<Profile>(this._apiUrl, profile);
  }

  updateProfile(profile: Profile) {
    return this.httpClient.put<Profile>(this._apiUrl, profile);
  }

  uploadImage(fd: FormData) {
    return this.httpClient.post(`${this._apiUrl}/image`, fd);
  }

  downloadImage(): Observable<Blob> {
    return this.httpClient.get(`${this._apiUrl}/image`, { responseType: 'blob' });
  }

  setIsAuthorized(isAuthorized: boolean) {
    this.isAuthorizedSubject.next(isAuthorized);
  }
}

export class Profile {
  firstName: string;
  lastName: string;
  city: string;
  email: string;
  address: string;
  iin: string;
  phoneNumber: string;
  userName: string;
  registeredLibrary: Library;
}

