import { Component, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";

@Component({
  selector: "app-value",
  templateUrl: "./value.component.html",
  styleUrls: ["./value.component.css"]
})
export class ValueComponent implements OnInit {
  values: any; // property to save data from API
  constructor(private http: HttpClient) {} // lifecycle event that gets called first, good place to inject services

  ngOnInit() {
    this.getValues();
  }

  getValues() {
    this.http.get("http://localhost:5000/api/values").subscribe(
      response => {
        this.values = response; // save the response data in values property
      },
      error => {
        console.log(error); // if error occurs, log to console
      }
    ); // returns an observable, which is a stream of data fetched from the API
  }
}
