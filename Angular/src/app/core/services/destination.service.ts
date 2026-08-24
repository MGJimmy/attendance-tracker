import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Destination {
  id: number;
  name: string;
  details: string;
  cost: number;
  isActive: boolean;
}

export interface AddDestinationDTO {
  name: string;
  details: string;
  cost: number;
}

export interface EditDestinationDTO {
  id: number;
  name: string;
  details: string;
  cost: number;
}

@Injectable({
  providedIn: 'root'
})
export class DestinationService {

  private readonly http = inject(HttpClient);
  private readonly api = environment.apiUrl + '/destination';

  add(payload: AddDestinationDTO): Observable<Destination> {
    return this.http.post<Destination>(`${this.api}/add`, payload);
  }

  edit(payload: EditDestinationDTO): Observable<Destination> {
    return this.http.put<Destination>(`${this.api}/edit`, payload);
  }

  activate(id: number): Observable<any> {
    return this.http.post(`${this.api}/activate/${id}`, {});
  }

  deactivate(id: number): Observable<any> {
    return this.http.post(`${this.api}/deactivate/${id}`, {});
  }

  getActive(): Observable<Destination[]> {
    return this.http.get<Destination[]>(this.api);
  }

  getAll(): Observable<Destination[]> {
    return this.http.get<Destination[]>(`${this.api}/all`);
  }

  getById(id: number): Observable<Destination> {
    return this.http.get<Destination>(`${this.api}/${id}`);
  }

}
