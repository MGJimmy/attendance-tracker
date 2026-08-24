import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Employee {
  id: number;
  name: string;
  salaryPerHour: number;
  userId: string;
  isActive: boolean;
}

export interface AddEmployeeDTO {
  name: string;
  salaryPerHour: number;
  userId: string;
}

export interface EditEmployeeDTO {
  id: number;
  name: string;
  salaryPerHour: number;
  userId: string;
}

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

  private readonly http = inject(HttpClient);
  private readonly api = environment.apiUrl + '/employee';

  add(payload: AddEmployeeDTO): Observable<Employee> {
    return this.http.post<Employee>(`${this.api}/add`, payload);
  }

  edit(payload: EditEmployeeDTO): Observable<Employee> {
    return this.http.put<Employee>(`${this.api}/edit`, payload);
  }

  activate(id: number): Observable<any> {
    return this.http.post(`${this.api}/activate/${id}`, {});
  }

  deactivate(id: number): Observable<any> {
    return this.http.post(`${this.api}/deactivate/${id}`, {});
  }

  getActive(): Observable<Employee[]> {
    return this.http.get<Employee[]>(this.api);
  }

  getAll(): Observable<Employee[]> {
    return this.http.get<Employee[]>(`${this.api}/all`);
  }

  getById(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.api}/${id}`);
  }

}
