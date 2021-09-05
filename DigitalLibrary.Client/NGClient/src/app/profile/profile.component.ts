import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {Profile, ProfileService} from "../services/profile.service";
import {first} from "rxjs/operators";
import {Library} from "../shared/entities/library.entity";
import {HttpClient} from "@angular/common/http";
import {LibraryService} from "../services/library.service";
import {Router} from "@angular/router";


@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit, OnDestroy {
  profileForm: FormGroup;
  onChange: boolean = false;
  profileFromRequest: Profile = new Profile();
  imageToShow: any;
  isImageLoading: boolean;

  selectedFile: File;
  fd: FormData = null;

  city: string;
  citiesDict = {
    "Almaty" : 1,
    "Karaganda" : 2,
    "Astana": 3,
    "Moscow" : 4
  }

  libraries: Library[] = [];

  constructor(
    private profileService: ProfileService,
    private http: HttpClient,
    private libService: LibraryService,
    private router: Router
  ) { }

  onFileSelected(event) {
    if (event.target.files.length > 0) {
      const file = <File>event.target.files[0];
      this.readURL(event.target);
      this.fd = new FormData();
      this.fd.append('file', file, file.name);
    }
  }

  readURL(input) {
    if (input.files && input.files[0]) {
      var reader = new FileReader();

      reader.onload = (e) => {
        this.imageToShow = e.target.result
      }

      reader.readAsDataURL(input.files[0]); // convert to base64 string
    }
  }


  onSubmit() {
    this.profileService.createProfile(this.profileForm.value).pipe(first()).subscribe(
      profile =>{
        console.log(profile);
        this.router.navigate(['']);
      },
      err => {
        if (err.error == "ProfileExist") {
          this.profileService.updateProfile(this.profileForm.value).pipe(first()).subscribe(
            _ => {
              this.onChange = false;
              this.router.navigate(['']);
            },
            err => {
              console.log(err);
            }
          );
        }
        console.log(err);
      }
    );

    this.profileService.uploadImage(this.fd).subscribe(
      () => {
        this.getImageFromService();
      },
      err => console.log(err)
    );
  }

  onChangeStatusChange() {
    // if the form is changing now
    if (this.onChange) {
      // download data and avatar into the form
      this.getImageFromService();
      this.profileService.getProfileObservable().pipe(first()).subscribe(
        profile => {
          if (profile != null) {
            this.setForm(profile);
            this.city = profile.city;
          }
        },
        err => {
          console.log(err);
        }
      );
    }
    this.onChange = !this.onChange;
  }

  returnInvalidIfInvalid(formControlName: string) : string {
    if (this.profileForm.get(formControlName).invalid && this.profileForm.get(formControlName).touched) {
      return 'invalid';
    } else {
      return '';
    }
  }

  setForm(profile: Profile) {
    this.profileForm.patchValue({
      firstName: profile.firstName,
      lastName: profile.lastName,
      image: null,
      userName: profile.userName,
      email: profile.email,
      registeredLibraryId: profile.registeredLibrary.id,
      phoneNumber: profile.phoneNumber,
      address: profile.address,
      city: this.city,
      iin: profile.iin
    });
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

  calculateSelection(i: number) {
    if (this.city != null) {
      var cityNum = this.citiesDict[this.city];
      if (cityNum == i) {
        return true;
      }
    }
    return false;
  }

  calculateSelectionForLibrary(id: string): boolean {
    var lib = this.profileForm.get('registeredLibraryId').value;
    if (lib == null) {
      return false;
    }
    if (id == lib.id) {
      return true;
    }
    return false;
  }

  ngOnInit(): void {
    this.libService.librariesSubject.subscribe(
      libraries => {
        this.libraries = libraries;
      },
      error => {
        console.log(error);
      }
    )

    this.libService.getLibraries();

    this.getImageFromService();

    this.profileForm = new FormGroup({
      firstName: new FormControl(null, Validators.required),
      lastName: new FormControl(null, Validators.required),
      image: new FormControl(null),
      userName: new FormControl(null),
      email: new FormControl(null),
      registeredLibraryId: new FormControl(null, Validators.required),
      phoneNumber: new FormControl(null, Validators.required),
      address: new FormControl(null, Validators.required),
      city: new FormControl(null, Validators.required),
      iin: new FormControl(null, Validators.required),
    });

    this.profileService.getProfileObservable().pipe(first()).subscribe(
      profile => {
            if (profile != null) {
              this.setForm(profile);
              console.log(profile);
              this.city = profile.city;
            }
      },
      err => {
        console.log(err);
      });
  }


  ngOnDestroy(): void {
  }

}