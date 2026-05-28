import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Prescription } from '../../models/prescription.model';
import { getPmsToken } from '../../utils/storage.util';

@Injectable({
  providedIn: 'root'
})
export class PrescriptionService {

  private apiUrl = 'http://localhost:5231/api/prescriptions';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Prescription[]> {
  return this.http.get<Prescription[]>(this.apiUrl, {
    headers: {
      Authorization: `Bearer ${getPmsToken()}`
    }
  });
}

  getById(id: number): Observable<Prescription> {
    return this.http.get<Prescription>(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${getPmsToken()}`
      }
    });
  }

  create(data: Prescription): Observable<any> {
    return this.http.post(this.apiUrl, data, {
      headers: {
        Authorization: `Bearer ${getPmsToken()}`
      }
    });
  }

  update(data: Prescription): Observable<any> {
    return this.http.put(this.apiUrl, data, {
      headers: {
        Authorization: `Bearer ${getPmsToken()}`
      }
    });
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${getPmsToken()}`
      }
    });
  }
}