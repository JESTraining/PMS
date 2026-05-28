import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardSparklines, DashboardSummary } from '../../models/analytics/dashboard-summary.model';

@Injectable({ providedIn: 'root' })
export class AnalyticsService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Dashboard`;

  getSummary(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(`${this.apiUrl}/summary`);
  }

  getSparklines(): Observable<DashboardSparklines> {
  return this.http.get<DashboardSparklines>(`${this.apiUrl}/sparklines`);
}
}