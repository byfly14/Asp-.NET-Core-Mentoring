import { Component, OnInit } from '@angular/core';
import { Observable, forkJoin } from 'rxjs';
import { HttpClient }   from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'simpleClient-app';

  productsList: any;
  categoriesList: any;

  constructor(private http: HttpClient) {     
  }
  ngOnInit(): void {
    let categoriesRequest = this.http.get('http://localhost:8088/api/categories');
    let productsRequest = this.http.get('http://localhost:8088/api/products');

    forkJoin([categoriesRequest, productsRequest]).subscribe(results => {
      this.categoriesList = results[0];
      this.productsList = results[1];  });
  }
}
