import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CourierSummaryFilter {
  from?: string;
  to?: string;
}

export interface Courier {
  id: number;
  name: string;
  mobileNumber: string;
  isActive: boolean;
}

export interface AddCourierDTO {
  name: string;
  mobileNumber: string;
}

export interface EditCourierDTO {
  id: number;
  name: string;
  mobileNumber: string;
}

export interface CourierTripRow {
  id: number;
  destinationName: string;
  tripDate: string;
  cost: number;
}

export interface CourierSummary {
  id: number;
  name: string;
  mobileNumber: string;
  isActive: boolean;
  totalTrips: number;
  totalPay: number;
  trips: CourierTripRow[];
}

@Injectable({
  providedIn: 'root'
})
export class CourierService {

  private readonly http = inject(HttpClient);
  private readonly api = environment.apiUrl + '/courier';

  add(payload: AddCourierDTO): Observable<Courier> {
    return this.http.post<Courier>(`${this.api}/add`, payload);
  }

  edit(payload: EditCourierDTO): Observable<Courier> {
    return this.http.put<Courier>(`${this.api}/edit`, payload);
  }

  activate(id: number): Observable<any> {
    return this.http.post(`${this.api}/activate/${id}`, {});
  }

  deactivate(id: number): Observable<any> {
    return this.http.post(`${this.api}/deactivate/${id}`, {});
  }

  getActive(): Observable<Courier[]> {
    return this.http.get<Courier[]>(this.api);
  }

  getAll(): Observable<Courier[]> {
    return this.http.get<Courier[]>(`${this.api}/all`);
  }

  getById(id: number): Observable<Courier> {
    return this.http.get<Courier>(`${this.api}/${id}`);
  }

  getSummary(id: number, filter: CourierSummaryFilter = {}): Observable<CourierSummary> {
    let params = new HttpParams();

    if (filter.from) {
      params = params.set('from', filter.from);
    }
    if (filter.to) {
      params = params.set('to', filter.to);
    }

    return this.http.get<CourierSummary>(`${this.api}/${id}/summary`, { params });
  }

}
