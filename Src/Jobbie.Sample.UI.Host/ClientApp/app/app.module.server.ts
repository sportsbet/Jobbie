import { NgModule } from '@angular/core';
import { sharedConfig } from './app.module.shared';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
    bootstrap: sharedConfig.bootstrap,
    declarations: sharedConfig.declarations,
    imports: [
        FormsModule,
        ReactiveFormsModule,
        ...sharedConfig.imports
    ]
})
export class AppModule {
}
