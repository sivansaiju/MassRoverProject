import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { imageDtls } from './Models/imageDtls';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'MarsRoverUI';
  private backendApiUrl: String;
  roverPhotos: Array<imageDtls> = [];
  constructor(private httpClient: HttpClient) {
    this.backendApiUrl = environment.backendApiUrl;
  }
  ngOnInit() {
    this.getMarsPhotos().subscribe((resp: Array<imageDtls>) => {
      this.roverPhotos = resp;
      console.log(this.roverPhotos);
    });
  }
  getMarsPhotos(): Observable<Array<imageDtls>> {

    return this.httpClient.get<Array<imageDtls>>(this.backendApiUrl + `api/MarsRoverPhotos/GetMarsPhotos`);
  }

}
