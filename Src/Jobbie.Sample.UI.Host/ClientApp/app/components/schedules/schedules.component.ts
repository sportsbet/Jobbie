import { Component } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { environment } from '../environment/environment';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/distinctUntilChanged';
import 'rxjs/add/operator/switchMap';

@Component({
    selector: 'schedules',
    templateUrl: './schedules.component.html'
})
export class SchedulesComponent {
    public schedules: Schedule[];
    public selectedJob: Job;
    public filterByJob: Job;
    public selectedScheduleId: string;
    public success: string;
    public page: Page;
    public sort: Sort;
    public errors: string[];
    public searchTerms: string;
    public searchTermStream = new Subject<string>();
    public searching: boolean;
    private apiUrl: string = environment.apiUrl;
    private apiVersion: string = environment.apiVersion;

    constructor(private http: Http, private route: ActivatedRoute, title: Title) {
        title.setTitle("Schedules - Jobbie Sample Admin UI");
        this.sort = new Sort();
        this.errors = [];

        this.searchTermStream
            .debounceTime(1000)
            .distinctUntilChanged()
            .switchMap(s => {
                this.searchTerms = s.toString();
                this.fetchSchedules(0);
                return this.schedules;
            })
            .subscribe();

        this.route.queryParams.subscribe(p => {
            if (p["jobId"] != null) {
                this.fetchSchedulesForJob(p["jobId"], 0);
                return;
            }

            this.fetchSchedules(0);
        });
    }

    public fetchSchedules(pageIndex: number) {
        this.searching = true;
        this.selectedJob = null;
        this.selectedScheduleId = null;

        if (this.filterByJob) {
            this.fetchSchedulesForJob(this.filterByJob.jobId, pageIndex);
            return;
        }

        this.http
            .get(this.apiUrl + this.apiVersion)
            .map(r => r.json())
            .subscribe(v => {
                var url = v._links["job:schedule-query"].href;
                if (this.searchTerms) {
                    url = v._links["job:schedule-queryby-description"].href.replace("{description}", this.searchTerms);
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
                            this.schedules = [];
                        }
                        else if (this.searchTerms) {
                            this.schedules = p._embedded["job:schedule-queryby-description"] as Schedule[];
                        }
                        else {
                            this.schedules = p._embedded["job:schedule-query"] as Schedule[];
                        }
                    });
            });
    }

    public fetchSchedulesForJob(jobId: string, pageIndex: number) {
        this.searching = true;
        this.http
            .get(this.apiUrl + this.apiVersion)
            .map(r => r.json())
            .subscribe(v => {
                this.http
                    .get(this.apiUrl + v._links["job:job"].href.replace("{jobId}", jobId))
                    .map(r => r.json())
                    .subscribe(j => {
                        this.filterByJob = j as Job;
                        var url = j._links["job:schedule-queryby-job"].href
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
                                    this.schedules = [];
                                }
                                else {
                                    this.schedules = p._embedded["job:schedule-queryby-job"] as Schedule[];
                                }
                            });
                    });
            });
    }

    public showJob(schedule: Schedule) {
        this.http
            .get(this.apiUrl + schedule._links["job:job"].href)
            .map(r => r.json())
            .subscribe(j => {
                this.selectedJob = j as Job;
                this.selectedScheduleId = schedule.scheduleId;
            });
    }

    public range(number) {
        var items: number[] = [];
        for (var i = 0; i < number; i++) {
            items.push(i);
        }
        return items;
    }

    public sortSchedules(property: string) {
        this.sort.property = property;
        this.sort.direction = this.sort.direction === 0 ? 1 : 0;
        this.fetchSchedules(0);
    }

    public deleteSchedule(schedule: Schedule) {
        if (confirm("Are you super sure?")) {
            this.http
                .get(this.apiUrl + this.apiVersion)
                .map(r => r.json())
                .subscribe(v => {
                    var url = v._links["job:schedule"].href.replace("{scheduleId}", schedule.scheduleId);
                    this.http
                        .get(this.apiUrl + url)
                        .map(r => r.json())
                        .subscribe(j => {
                            url = this.apiUrl + j._links["job:schedule-delete"].href;
                            this.http
                                .delete(url)
                                .subscribe(
                                    () => {
                                        this.success = "Successfully deleted!";
                                        this.fetchSchedules(0);
                                    },
                                    error => {
                                        console.error(error);
                                        this.errors.push("Failed to delete.");
                                    });
                        });
                });
        }
    }

    public searchSchedules(terms: string): void {
        this.searchTermStream.next(terms);
    }

    public resetErrors() {
        this.errors = [];
    }
}

interface Link {
    href: string;
}

interface Schedule {
    scheduleId: string;
    description: string;
    createdUtc: string;
    startUtc: string;
    nextUtc: string;
    previousUtc: string;
    endUtc: string;
    cron: string;
    _links: Link[];
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
    isOnceOff: boolean;
    timeout: string;
}

class Sort {
    property: string;
    direction: number;

    constructor() {
        this.property = "";
        this.direction = 0;
    }
}
