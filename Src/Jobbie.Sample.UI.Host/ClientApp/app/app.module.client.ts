import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { sharedConfig } from './app.module.shared';
import { DatePipe } from '@angular/common';
import { AdalService } from 'ng2-adal/core';
import { provideAuth } from 'angular2-jwt/angular2-jwt';
import { RouteGuard } from './components/navmenu/routeguard';

@NgModule({
    bootstrap: sharedConfig.bootstrap,
    declarations: sharedConfig.declarations,
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        ReactiveFormsModule,
        ...sharedConfig.imports
    ],
    providers: [
        DatePipe,
        AdalService,
        RouteGuard,
        provideAuth({
            tokenGetter: (() => localStorage.getItem("id_token"))
        })
    ]
})
export class AppModule {
}
