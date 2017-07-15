import { Component, OnInit } from '@angular/core';
import { AdalService } from 'ng2-adal/core';
import { environment } from '../environment/environment';

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    constructor(private adalService: AdalService) {
        const config = {
            tenant: environment.authTenant,
            clientId: environment.authClientId,
            redirectUri: window.location.origin + "/login",
            postLogoutRedirectUri: window.location.origin + "/login",
            resourceId: environment.authResourceId
        };
        this.adalService.init(config);
    }

    ngOnInit(): void {
        this.adalService.handleWindowCallback();
        this.adalService.getUser();
    }
}
