import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { EncounterListItem } from '../../Entities/Encounters/Encounter';
import { EncounterInterface } from '../../Entities/Encounters/Encounter';
import { EncounterSummaryDto } from '../../Entities/Encounters/Encounter';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class EncounterService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/encounter`;
  private mapEncounter(data: any): EncounterInterface {
    return {
      EncounterId: data.id,

      patientName: data.patientName,

      startTime: data.startTime,

      endTime: data.endTime,

      encounterReason: data.encounterReason,

      conditions: data.conditions,

      clinicalObservations: data.clinicalObservations,

      clinicalNotes: data.clinicalNotes,
      appointment: {
        id: data.appointmentId,
        reason: data.reason,
        startTime: data.appointmentStart,
        endTime: data.appointmentEnd
      },

      prescriptions: data.prescriptions
    };
  }

  private getAuthHeaders() {
    const token = localStorage.getItem('pms_token');
    return { headers: { Authorization: `Bearer ${token}` } };
  }

  getEncounters(): Observable<EncounterListItem[]> {
  return this.http.get<EncounterListItem[]>(
    this.apiUrl,
    this.getAuthHeaders()
  );
}
  updateAppointment(encounterId: number, payload: any) {
    return this.http.put(
      `${this.apiUrl}/${encounterId}/appointment`,
      payload,
      this.getAuthHeaders()
    );
  }
  getEncounterDetail(id: number): Observable<EncounterInterface> {
    return this.http
      .get<any>(
        `${this.apiUrl}/${id}`,
        this.getAuthHeaders()
      )
      .pipe(
        map(data => this.mapEncounter(data))
      );
  }
  updateObservation(encounterId: number, obsId: number, data: any) {
    return this.http.put(
      `${this.apiUrl}/${encounterId}/observations/${obsId}`,
      data,
      this.getAuthHeaders()
    );
  }

  startEncounter(appointmentId: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/start/${appointmentId}`, {}, this.getAuthHeaders());
  }

  getEncounterSummary(encounterId: number): Observable<EncounterSummaryDto> {
    return this.http.get<EncounterSummaryDto>(`${this.apiUrl}/${encounterId}/summary`, this.getAuthHeaders());
  }

  updateClinicalNote(id: number, data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/note`, data, this.getAuthHeaders());
  }

  addObservation(id: number, data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/observations`, data, this.getAuthHeaders());
  }
  addCondition(encounterId: number, data: any) {
    return this.http.post(
      `${this.apiUrl}/${encounterId}/conditions`,
      data,
      this.getAuthHeaders()
    );
  }

  updateCondition(encounterId: number, conditionId: number, data: any) {
    return this.http.put(
      `${this.apiUrl}/${encounterId}/conditions/${conditionId}`, 
      data,
      this.getAuthHeaders() 
    );
  }
  updatePrescription(encounterId: number, prescriptionId: number, payload: any) {
    return this.http.put(
      `${this.apiUrl}/${encounterId}/prescriptions/${prescriptionId}`, 
      payload,
      this.getAuthHeaders() 
    );
  }
  getMedications(): Observable<any[]> {
    return this.http.get<any[]>(
      `${environment.apiUrl}/api/medications`,
      this.getAuthHeaders()
    );
  }

  createPrescription(encounterId: number, payload: any) {
    return this.http.post(
      `${this.apiUrl}/${encounterId}/prescriptions`,
      payload,
      this.getAuthHeaders()
    );
  }

  addAllergy(id: number, data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/allergies`, data, this.getAuthHeaders());
  }

  completeEncounter(id: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/complete`, {}, this.getAuthHeaders());
  }

  deleteObservation(encounterId: number, observationId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${encounterId}/observations/${observationId}`, this.getAuthHeaders());
  }


  deleteCondition(encounterId: number, conditionId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${encounterId}/conditions/${conditionId}`, this.getAuthHeaders());
  }

  deleteAllergy(encounterId: number, allergyId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${encounterId}/allergies/${allergyId}`, this.getAuthHeaders());
  }
  deletePrescription(encounterId: number, prescriptionId: number) {
  return this.http.delete(
    `${this.apiUrl}/${encounterId}/prescriptions/${prescriptionId}`,
    this.getAuthHeaders()
  );
}
}