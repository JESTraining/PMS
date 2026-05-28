import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { getPmsToken } from '../../utils/storage.util';

export interface ScheduleFilter {
  doctorsId: number[];
  date?: string; // Formato YYYY-MM-DD para DateOnly
  scheduleStatus?: string;
  patientName?: string;
}

export interface ScheduleView {
  type: string; // 'Appointment' | 'Rest'
  id: number;
  doctorId: number;
  doctorName: string;
  patientId?: number;
  patientName?: string;
  date: string;
  startTime: string;
  endTime: string;
  scheduleStatus: string;
  scheduleDescription: string;
}

@Injectable({ providedIn: 'root' })
export class ScheduleService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Appointments`;

  public selectedDate = signal<Date>(new Date());
  public selectedDoctorId = signal<number | null>(null);
  public events = signal<any[]>([]);
  public isLoading = signal<boolean>(false);

  private getAuthHeaders() {
    return {
      headers: {
        Authorization: `Bearer ${getPmsToken()}`
      }
    };
  }

  loadSchedule() {
    const doctorId = this.selectedDoctorId();
    if (!doctorId) return; 

    this.isLoading.set(true);
    const dateStr = this.selectedDate().toISOString().split('T')[0];

    const filter = {
      doctorsId: [doctorId],
      date: dateStr
    };

    this.http.post<any[]>(`${this.apiUrl}/search`, filter, this.getAuthHeaders()).subscribe({
      next: (data) => {
        this.events.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error cargando schedule:', err);
        this.events.set([]);
        this.isLoading.set(false);
      }
    });
  }

  createAppointment(appointmentData: any) {
    return this.http.post(`${this.apiUrl}`, appointmentData, this.getAuthHeaders());
  }
}