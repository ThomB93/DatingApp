import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private http = inject(HttpClient);
  protected readonly title = signal('client');

  ngOnInit(): void {
    this.http.get('https://localhost:5001/api/members').subscribe({
      next: (members) => { // Handle the fetched members data
        console.log(members);
      },
      error: (error) => { // Handle error appropriately
        console.error('Error fetching members:', error);
      },
      complete: () => { // Optional: log when the request is complete
        console.log('Request completed');
      }
    });
  }
}