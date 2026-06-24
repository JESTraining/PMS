import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { getPmsToken } from '../../utils/storage.util';

@Injectable({ providedIn: 'root' })
export class DoctorService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/doctors`;

  private getAuthHeaders() {
    return { headers: { Authorization: `Bearer ${getPmsToken()}` } };
  }

  getDoctors(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl, this.getAuthHeaders());
  }

  searchDoctors(term: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/search?searchTerm=${term}`, this.getAuthHeaders());
  }
}