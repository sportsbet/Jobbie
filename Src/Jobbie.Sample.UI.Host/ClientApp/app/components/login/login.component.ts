import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { AdalService } from 'ng2-adal/core';
import { environment } from '../environment/environment';

@Component({
    selector: 'Login',
    templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
    constructor(title: Title, private adalService: AdalService, private router: Router) {
        title.setTitle("Signing in... - Jobbie Sample Admin UI");
    }

    ngOnInit(): void {
        if (this.adalService.userInfo.isAuthenticated) {
            this.adalService
                .acquireToken(environment.authResourceId)
                .subscribe(token => localStorage.setItem("id_token", token));
            this.router.navigate(["/home"]);
        }
        else {
            this.adalService.login();
        }
    }
}
