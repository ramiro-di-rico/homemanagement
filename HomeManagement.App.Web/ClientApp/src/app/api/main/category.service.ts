import { Injectable } from '@angular/core';
import { Category } from "../../models/category";
import { Subject, Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../../auth/authentication.service';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable()
export class CategoryService {

    private categories: Array<Category> = [];
    private activeCategories: Array<Category> = [];

    private categoriesSource = new Subject<boolean>();
    categoriesChanged = this.categoriesSource.asObservable(); 

    private activeCategoriesSource = new Subject<boolean>();
    activeCategoriesChanged = this.activeCategoriesSource.asObservable(); 
    endpoint: string;

    httpOptions = {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };

    constructor(private http: HttpClient,
        private authenticationService: AuthService) {

        this.httpOptions.headers = this.httpOptions.headers.append('Authorization', this.authenticationService.getToken());
        this.endpoint = environment.api + '/api/category';
    }

    getCategories() : Observable<Array<Category>> {

        if(this.categories.length > 0){
            return new Observable(obs => obs.next(this.categories));
        }

        return this.http.get<Array<Category>>(this.endpoint, this.httpOptions)
        .pipe(map(result => {
            this.categories = result;
            return this.categories;
        }));
    }

    getActiveCategories() : Observable<Array<Category>> {

        if(this.activeCategories.length > 0){
            return new Observable(obs => obs.next(this.activeCategories));
        }

        return this.http.get<Array<Category>>(this.endpoint + '/active', this.httpOptions)
        .pipe(map(result => {
            this.activeCategories = result;
            return this.activeCategories;
        }));
    }

    update(c: Category) {
        return this.http.put(this.endpoint, c, this.httpOptions)
        .pipe(map(_ => {
            this.updateState(c);
            return true;
        }));
    }

    add(c: Category) {
        return this.http.post(this.endpoint, c, this.httpOptions)
        .pipe(map(_ => {
         
            this.reload();
            return true;
        }));
    }

    delete(c: Category) {
        return this.http.delete(this.endpoint + "/" + c.id, this.httpOptions)
        .pipe(map(_ => {
            
            this.reload();
            return true;
        }));
    }

    private updateState(c: Category){
        if(c.isActive){
            this.activeCategories.push(c);
        }else{
            let index = this.activeCategories.indexOf(c);
            this.activeCategories.splice(index,1); 
        }
    }

    reload() {
        this.categories.length = 0;
        this.getCategories().subscribe(_ => {
            this.categoriesSource.next(true);
        });

        this.activeCategories.length = 0;
        this.getActiveCategories().subscribe(_ => {
            this.activeCategoriesSource.next(true);
        });
    }

    getCategoryByName(name:string){
        return this.activeCategories.find(s => s.name === name);
    }

    export(){
        return this.http.get(this.endpoint + "/download")
        .pipe(map(_ => {
            return _;
        }));
    }

    import(formData: FormData){
        return this.http.post(this.endpoint + "/upload", formData)
        .pipe(map(_ => {
            return _;
        }));
    }
}
