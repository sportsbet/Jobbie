import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { RouteGuard } from './components/navmenu/routeguard';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { JobsComponent } from './components/jobs/jobs.component';
import { SchedulesComponent } from './components/schedules/schedules.component';

export const sharedConfig: NgModule = {
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        LoginComponent,
        JobsComponent,
        SchedulesComponent,
        HomeComponent
    ],
    imports: [
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'login', component: LoginComponent },
            { path: 'jobs', component: JobsComponent, canActivate: [RouteGuard] },
            { path: 'schedules', component: SchedulesComponent, canActivate: [RouteGuard] },
            { path: '**', redirectTo: 'home' }
        ])
    ]
};
