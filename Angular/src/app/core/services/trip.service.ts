import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Trip {
  id: number;
  courierId: number;
  courierName: string;
  destinationId: number;
  destinationName: string;
  cost: number;
  tripDate: string;
}

export interface AddTripDTO {
  courierId: number;
  destinationId: number;
  cost?: number | null;
  tripDate: string;
}

export interface EditTripDTO {
  id: number;
  courierId: number;
  destinationId: number;
  cost: number;
  tripDate: string;
}

export interface TripFilter {
  courierId?: number;
  from?: string;
  to?: string;
}

@Injectable({
  providedIn: 'root'
})
export class TripService {

  private readonly http = inject(HttpClient);
  private readonly api = environment.apiUrl + '/trip';

  add(payload: AddTripDTO): Observable<Trip> {
    return this.http.post<Trip>(`${this.api}/add`, payload);
  }

  edit(payload: EditTripDTO): Observable<Trip> {
    return this.http.put<Trip>(`${this.api}/edit`, payload);
  }

  getAll(filter: TripFilter = {}): Observable<Trip[]> {
    let params = new HttpParams();

    if (filter.courierId != null) {
      params = params.set('courierId', filter.courierId);
    }
    if (filter.from) {
      params = params.set('from', filter.from);
    }
    if (filter.to) {
      params = params.set('to', filter.to);
    }

    return this.http.get<Trip[]>(this.api, { params });
  }

  getById(id: number): Observable<Trip> {
    return this.http.get<Trip>(`${this.api}/${id}`);
  }

}
