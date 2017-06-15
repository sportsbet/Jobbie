import { Component } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent {
    constructor(title: Title) {
        title.setTitle("Home - Jobbie Sample Admin UI");
    }
}
