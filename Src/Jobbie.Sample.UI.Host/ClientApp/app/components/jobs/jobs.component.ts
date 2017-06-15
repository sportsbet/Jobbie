import { Component } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { Router } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Subject } from 'rxjs/Subject';
import { environment } from '../environment/environment';
import { DatePipe } from '@angular/common';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/distinctUntilChanged';
import 'rxjs/add/operator/switchMap';

@Component({
    selector: 'jobs',
    templateUrl: './jobs.component.html',
    providers: [DatePipe]
})
export class JobsComponent {
    public jobs: Job[];
    public selectedJob: Job;
    public page: Page;
    public sort: Sort;
    public showJobForm: boolean;
    public showScheduleForm: boolean;
    public success: string;
    public jobForm: FormGroup;
    public scheduleForm: FormGroup;
    public errors: string[];
    public searchTerms: string;
    public searchTermStream = new Subject<string>();
    public searching: boolean;
    private apiUrl = environment.apiUrl;
    private apiVersion = environment.apiVersion;

    constructor(private http: Http, private router: Router, public fb: FormBuilder, title: Title, datePipe: DatePipe) {
        title.setTitle("Jobs - Jobbie Sample Admin UI");
        this.sort = new Sort();
        this.errors = [];

        this.jobForm = fb.group({
            "description": [""],
            "callbackUrl": [""],
            "httpVerb": ["POST"],
            "payload": [""],
            "contentType": ["application/json"],
            "headers": [""]
        });

        this.scheduleForm = fb.group({
            "description": [""],
            "startLocal": [datePipe.transform(Date.now(), "yyyy-MM-dd HH:mm:ss")],
            "cron": [""],
            "endLocal": [""],
            "cron1": [""],
            "cron2": [""],
            "cron3": [""],
            "cron4": [""],
            "cron5": [""],
            "cron6": [""]
        });

        this.searchTermStream
            .debounceTime(1000)
            .distinctUntilChanged()
            .switchMap(s => {
                this.searchTerms = s.toString();
                this.fetchJobs(0);
                return this.jobs;
            })
            .subscribe();

        this.fetchJobs(0);
    }

    public fetchJobs(pageIndex: number) {
        this.searching = true;
        this.selectedJob = null;
        this.http
            .get(this.apiUrl + this.apiVersion)
            .map(r => r.json())
            .subscribe(v => {
                var url = v._links["job:job-query"].href;
                if (this.searchTerms) {
                    url = v._links["job:job-queryby-description"].href.replace("{description}", this.searchTerms);
                }
                url = url
                    .replace("{pageIndex}", pageIndex)
                    .replace("{sortProperty}", this.sort.property)
                    .replace("{sortDirection}", this.sort.direction);
                this.http
                    .get(this.apiUrl + url)
                    .map(r => r.json())
                    .subscribe(p => {
                        this.searching = false;
                        this.page = p as Page;
                        if (this.page.totalItemsCount === 0) {
                            this.jobs = [];
                        }
                        else if (this.searchTerms) {
                            this.jobs = p._embedded["job:job-queryby-description"] as Job[];
                        }
                        else {
                            this.jobs = p._embedded["job:job-query"] as Job[];
                        }
                    });
            });
    }

    public showJob(job: Job) {
        this.selectedJob = job;
    }

    public range(number) {
        const items: number[] = [];
        for (let i = 0; i < number; i++) {
            items.push(i);
        }
        return items;
    }

    public viewSchedules() {
        this.router.navigate(["/schedules"], { queryParams: { jobId: this.selectedJob.jobId } });
    }

    public sortJobs(property: string) {
        this.sort.property = property;
        this.sort.direction = this.sort.direction === 0 ? 1 : 0;
        this.fetchJobs(0);
    }

    public createJob(job: Job) {
        this.resetErrors();

        this.http
            .get(this.apiUrl + this.apiVersion)
            .map(r => r.json())
            .subscribe(v => {
                var url = this.apiUrl + v._links["job:job-create"].href;
                var headers = new Headers({ "Content-Type": "application/json" });
                var payload = JSON.stringify(job);
                this.http
                    .post(url, payload, { headers: headers })
                    .map(r => r.json())
                    .subscribe(
                        () => {
                            this.showJobForm = false;
                            this.success = "Successfully created!";
                            this.fetchJobs(0);
                        },
                        error => {
                            console.error(error);
                            if (error.status === 400) {
                                var body = error.json();
                                const state = body.ModelState;
                                for (let e in state) {
                                    this.errors.push(state[e]);
                                }
                            }
                            else {
                                this.errors.push("Failed to create.");
                            }
                        });
            });
    }

    public createSchedule(schedule: Schedule) {
        this.resetErrors();
        schedule.startUtc = this.toUtc(schedule.startLocal);
        schedule.endUtc = this.toUtc(schedule.endLocal);

        this.http
            .get(this.apiUrl + this.apiVersion)
            .map(r => r.json())
            .subscribe(v => {
                var url = v._links["job:job"].href.replace("{jobId}", this.selectedJob.jobId);
                this.http
                    .get(this.apiUrl + url)
                    .map(r => r.json())
                    .subscribe(j => {
                        url = this.apiUrl + j._links["job:schedule-create"].href;
                        var headers = new Headers({ "Content-Type": "application/json" });
                        var payload = JSON.stringify(schedule);
                        this.http
                            .post(url, payload, { headers: headers })
                            .map(r => r.json())
                            .subscribe(
                                () => {
                                    this.showScheduleForm = false;
                                    this.success = "Successfully created!";
                                },
                                error => {
                                    console.error(error);
                                    if (error.status === 400) {
                                        var body = error.json();
                                        const state = body.ModelState;
                                        for (let e in state) {
                                            this.errors.push(state[e]);
                                        }
                                    }
                                    else {
                                        this.errors.push("Failed to create.");
                                    }
                                }
                            );
                    });
            });
    }

    public deleteJob(job: Job) {
        if (confirm("Are you super sure?")) {
            this.http
                .get(this.apiUrl + this.apiVersion)
                .map(r => r.json())
                .subscribe(v => {
                    var url = v._links["job:job"].href.replace("{jobId}", job.jobId);
                    this.http
                        .get(this.apiUrl + url)
                        .map(r => r.json())
                        .subscribe(j => {
                            url = this.apiUrl + j._links["job:job-delete"].href;
                            this.http
                                .delete(url)
                                .subscribe(
                                    () => {
                                        this.success = "Successfully deleted!";
                                        this.fetchJobs(0);
                                    },
                                    error => {
                                        console.error(error);
                                        this.errors.push("Failed to delete.");
                                    });
                        });
                });
        }
    }

    public resetErrors() {
        this.errors = [];
    }

    public searchJobs(terms: string): void {
        this.searchTermStream.next(terms);
    }

    public setCron() {
        var cron1 = this.scheduleForm.get("cron1").value;
        var cron2 = this.scheduleForm.get("cron2").value;
        var cron3 = this.scheduleForm.get("cron3").value;
        var cron4 = this.scheduleForm.get("cron4").value;
        var cron5 = this.scheduleForm.get("cron5").value;
        var cron6 = this.scheduleForm.get("cron6").value;
        this.scheduleForm.get("cron").setValue(`${cron1} ${cron2} ${cron3} ${cron4} ${cron5} ${cron6}`);
    }

    private toUtc(local: Date) {
        if (local) {
            const d = new Date(local);
            return new Date(Date.UTC(d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate(), d.getUTCHours(), d.getUTCMinutes(), d.getUTCSeconds()));
        }
        return null;
    }
}

interface Page {
    pageIndex: number;
    pageSize: number;
    knownPagesAvailable: number;
    totalItemsCount: number;
}

interface Job {
    jobId: string;
    description: string;
    createdUtc: string;
    callbackUrl: string;
    httpVerb: string;
    payload: string;
    contentType: string;
    headers: string;
}

interface Schedule {
    description: string;
    startLocal: Date;
    startUtc: Date;
    cron: string;
    endLocal: Date;
    endUtc: Date;
}

class Sort {
    property: string;
    direction: number;

    constructor() {
        this.property = "";
        this.direction = 0;
    }
}
