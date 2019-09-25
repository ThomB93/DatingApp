import { Component } from '@angular/core';

@Component({ // a decorator that is used to give components angular features
  selector: 'app-root', // used in html to mark where the component goes
  templateUrl: './app.component.html', // the view
  styleUrls: ['./app.component.css']
})
export class AppComponent { // provides data for the view
  title = 'app';
}
