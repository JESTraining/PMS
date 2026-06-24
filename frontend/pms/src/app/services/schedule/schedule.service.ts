import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient, httpResource } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { getPmsToken } from '../../utils/storage.util';
import { ScheduleView, CreateAppointmentDto, AvailabilityResult } from '../../models/schedule.model';

@Injectable({ providedIn: 'root' })
export class ScheduleService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/Appointments`;

  readonly selectedDoctorId = signal<number | null>(null);
  readonly selectedDate = signal<Date>(new Date());

  /** Local YYYY-MM-DD for the selected day (avoids the UTC off-by-one from toISOString()). */
  private readonly selectedDateKey = computed(() => toLocalDateKey(this.selectedDate()));

  /** Reactive day schedule; refetches automatically whenever the doctor or date changes. */
  readonly scheduleResource = httpResource<ScheduleView[]>(
    () => {
      const doctorId = this.selectedDoctorId();
      if (!doctorId) return undefined; // no doctor selected -> no request
      return {
        url: `${this.apiUrl}/search`,
        method: 'POST',
        headers: this.authHeaders(),
        body: { doctorsId: [doctorId], date: this.selectedDateKey() },
      };
    },
    { defaultValue: [] },
  );

  /** The current day's events (appointments + rest periods). */
  readonly events = computed<ScheduleView[]>(() => this.scheduleResource.value());
  readonly isLoading = computed(() => this.scheduleResource.isLoading());

  /** Refetch the current day's schedule. */
  loadSchedule(): void {
    this.scheduleResource.reload();
  }

  checkAvailability(doctorId: number, start: string, end: string) {
    return this.http.get<AvailabilityResult>(`${this.apiUrl}/availability`, {
      headers: this.authHeaders(),
      params: { doctorId, start, end },
    });
  }

  createAppointment(appointment: CreateAppointmentDto) {
    return this.http.post(this.apiUrl, appointment, { headers: this.authHeaders() });
  }

  rescheduleAppointment(id: number, start: string, end: string) {
    return this.http.put(`${this.apiUrl}/${id}`, { startTime: start, endTime: end }, { headers: this.authHeaders() });
  }

  cancelAppointment(id: number) {
    return this.http.put(`${this.apiUrl}/${id}`, { status: 'Cancelled' }, { headers: this.authHeaders() });
  }

  /** Restores a cancelled appointment back to Scheduled (used by the cancel Undo action). */
  reactivateAppointment(id: number) {
    return this.http.put(`${this.apiUrl}/${id}`, { status: 'Scheduled' }, { headers: this.authHeaders() });
  }

  private authHeaders(): Record<string, string> {
    const token = getPmsToken();
    return token ? { Authorization: `Bearer ${token}` } : {};
  }
}

/** Formats a Date as a local YYYY-MM-DD key (no timezone conversion). */
function toLocalDateKey(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}
