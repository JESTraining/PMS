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

export interface ScheduleFilter {
  doctorsId: number[];
  date?: string;
  scheduleStatus?: string;
  patientName?: string;
}

/** Payload for POST /api/Appointments (matches backend AppointmentRequestRecord). */
export interface CreateAppointmentDto {
  startTime: string;
  endTime: string;
  reason: string;
  doctorId: number;
  patientId: number;
  status?: string; // ignored by backend; retained for the legacy modal during migration
}

/** Result of GET /api/Appointments/availability. */
export interface AvailabilityResult {
  isAvailable: boolean;
  errorMessage: string;
}

export interface DaySummary {
  total: number;
  scheduled: number;
  inProgress: number;
  completed: number;
  free: number;
  restRange: string | null;
}

export interface NextSlot {
  key: string;
  dateLabel: string;
  time: string;
  date: Date;
}
