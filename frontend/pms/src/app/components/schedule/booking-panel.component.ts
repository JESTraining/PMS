import { Component, ChangeDetectionStrategy, inject, input, output, signal, computed, linkedSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PatientService } from '../../services/patient/patient.service';
import { ScheduleService } from '../../services/schedule/schedule.service';
import { Patient } from '../../models/patient.model';
import { DoctorOption } from '../../models/doctor.model';
import { Insurance } from '../../models/insurance.model';
import { addMinutesToLabel } from '../../utils/schedule-time.util';

@Component({
  selector: 'app-booking-panel',
  standalone: true,
  imports: [CommonModule, FormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrl: './booking-panel.component.scss',
  template: `
    <div class="sch-bp">

      <!-- Header: title + doctor context + date chip -->
      <div class="sch-bp-head">
        <div class="sch-bp-head-info">
          <h3 class="sch-bp-title">Book appointment</h3>
          <div class="sch-bp-head-meta">
            <span class="sch-bp-doctor-label">
              {{ doc().name }}@if (doc().specialty) { · <em>{{ doc().specialty }}</em> }
            </span>
            <span class="sch-bp-date-chip">
              <span class="material-icons sch-bp-chip-icon">event</span>
              {{ date() | date: 'EEE, MMM d' }}
            </span>
          </div>
        </div>
        <button class="ui-close" (click)="closed.emit()" aria-label="Close booking panel">✕</button>
      </div>

      @if (availableSlots().length === 0) {
        <div class="sch-bp-empty">No free slots on this day. Try another date.</div>
      } @else {
        <div class="sch-bp-body">

          <!-- Time -->
          <div class="sch-bp-time">
            <label class="sch-bp-section-label">
              <span class="material-icons sch-bp-label-icon">schedule</span>
              Time
            </label>
            <div class="sch-bp-slot-grid">
              @for (slot of availableSlots(); track slot) {
                <button type="button" class="sch-bp-slot"
                        [class.sch-bp-slot--active]="selectedTime() === slot"
                        (click)="selectedTime.set(slot)">
                  {{ slot }}
                </button>
              }
            </div>
            <p class="sch-bp-hint">Fixed 30-min · ends {{ endFor(selectedTime()) }}</p>
          </div>

          <!-- Patient -->
          <div class="sch-bp-field">
            <label class="sch-bp-section-label">
              <span class="material-icons sch-bp-label-icon">person</span>
              Patient
            </label>

            @if (selectedPatient(); as p) {
              <!-- Selected patient info card -->
              <div class="sch-bp-patient-card">
                <div class="sch-bp-avatar">{{ patientInitials() }}</div>
                <div class="sch-bp-patient-info">
                  <span class="sch-bp-patient-name">{{ p.firstName }} {{ p.lastName }}</span>
                  <span class="sch-bp-patient-meta">
                    @if (patientAge(); as age) { {{ age }} yrs · }{{ p.phone }}
                  </span>
                  @if (primaryInsurance(); as ins) {
                    <span class="sch-bp-insurance-pill">
                      <span class="material-icons sch-bp-ins-icon">verified</span>
                      {{ ins.payerName }}
                    </span>
                  }
                </div>
                <button class="sch-bp-patient-clear" (click)="clearPatient()" aria-label="Clear patient">✕</button>
              </div>
            } @else {
              <!-- Search -->
              <div class="sch-bp-search-wrap">
                <input type="text" class="ui-input" [(ngModel)]="patientTerm" (ngModelChange)="onSearch($event)"
                       placeholder="Search by name or ID…" />
                @if (patientResults().length > 0) {
                  <div class="sch-bp-results">
                    @for (p of patientResults(); track p.id) {
                      <button class="sch-bp-result" (click)="selectPatient(p)">
                        <div class="sch-bp-result-avatar">{{ resultInitials(p) }}</div>
                        <div class="sch-bp-result-body">
                          <strong>{{ p.firstName }} {{ p.lastName }}</strong>
                          <span class="sch-bp-result-meta">
                            ID {{ p.id }}@if (resultAge(p); as age) { · {{ age }} yrs }
                          </span>
                        </div>
                      </button>
                    }
                  </div>
                }
              </div>
            }
          </div>

          <!-- Reason -->
          <div class="sch-bp-reason">
            <label class="sch-bp-section-label">
              <span class="material-icons sch-bp-label-icon">notes</span>
              Reason for visit
            </label>
            <textarea class="ui-input" [(ngModel)]="reason" rows="3" placeholder="e.g., Routine checkup…"></textarea>
          </div>

          @if (errorMessage()) {
            <div class="sch-bp-error">{{ errorMessage() }}</div>
          }
        </div>

        <div class="sch-bp-foot">
          <!-- Confirmation summary -->
          @if (canSave()) {
            <div class="sch-bp-summary">
              <span class="material-icons sch-bp-summary-icon">schedule</span>
              <span>{{ selectedTime() }} – {{ endFor(selectedTime()) }}</span>
              <span class="sch-bp-summary-sep">·</span>
              <span class="sch-bp-summary-patient">{{ selectedPatient()!.firstName }} {{ selectedPatient()!.lastName }}</span>
              <span class="sch-bp-summary-sep">·</span>
              <span>30 min</span>
            </div>
          }
          <button class="ui-btn ui-btn-primary w-full" style="justify-content:center;"
                  (click)="save()" [disabled]="!canSave()">
            {{ isSaving() ? 'Booking…' : 'Book appointment' }}
          </button>
        </div>
      }
    </div>
  `,
})
export class BookingPanelComponent {
  private readonly patientService = inject(PatientService);
  private readonly scheduleService = inject(ScheduleService);

  readonly doc = input.required<DoctorOption>();
  readonly doctorId = input.required<number>();
  readonly date = input.required<Date>();
  readonly availableSlots = input<string[]>([]);
  readonly initialSlot = input<string | null>(null);

  readonly booked = output<void>();
  readonly closed = output<void>();

  readonly selectedTime = linkedSignal<string>(() => this.initialSlot() ?? this.availableSlots()[0] ?? '');

  readonly patientTerm = signal('');
  readonly patientResults = signal<Patient[]>([]);
  readonly selectedPatient = signal<Patient | null>(null);
  readonly reason = signal('');
  readonly isSaving = signal(false);
  readonly errorMessage = signal('');

  readonly canSave = computed(
    () => !!this.selectedTime() && !!this.selectedPatient() && this.reason().trim().length > 0 && !this.isSaving(),
  );

  readonly patientInitials = computed(() => {
    const p = this.selectedPatient();
    return p ? (p.firstName[0] ?? '') + (p.lastName[0] ?? '') : '';
  });

  readonly patientAge = computed(() => computeAge(this.selectedPatient()?.dateOfBirth));

  readonly primaryInsurance = computed<Insurance | null>(() => {
    const ins = this.selectedPatient()?.insurances;
    if (!ins?.length) return null;
    return ins.find(i => i.isPrimary) ?? ins[0];
  });

  endFor(slot: string): string {
    return addMinutesToLabel(slot, 30);
  }

  resultInitials(p: Patient): string {
    return (p.firstName[0] ?? '') + (p.lastName[0] ?? '');
  }

  resultAge(p: Patient): number | null {
    return computeAge(p.dateOfBirth);
  }

  onSearch(term: string): void {
    if (!term || term.trim().length < 1) {
      this.patientResults.set([]);
      return;
    }
    this.patientService.searchPatients(term.trim()).subscribe({
      next: (results) => this.patientResults.set(results ?? []),
      error: () => this.patientResults.set([]),
    });
  }

  selectPatient(patient: Patient): void {
    this.selectedPatient.set(patient);
    this.patientResults.set([]);
    this.patientTerm.set('');
  }

  clearPatient(): void {
    this.selectedPatient.set(null);
  }

  save(): void {
    const patient = this.selectedPatient();
    const time = this.selectedTime();
    if (!patient?.id || !time || !this.canSave()) return;

    this.isSaving.set(true);
    this.errorMessage.set('');

    const dateKey = toLocalDateKey(this.date());
    const start = `${dateKey}T${time}:00`;
    const end = `${dateKey}T${addMinutesToLabel(time, 30)}:00`;

    this.scheduleService
      .createAppointment({ startTime: start, endTime: end, reason: this.reason().trim(), doctorId: this.doctorId(), patientId: patient.id! })
      .subscribe({
        next: () => {
          this.isSaving.set(false);
          this.booked.emit();
        },
        error: (err) => {
          this.isSaving.set(false);
          this.errorMessage.set(typeof err?.error === 'string' ? err.error : 'Could not book the appointment. Please try another slot.');
        },
      });
  }
}

function computeAge(dateOfBirth?: string): number | null {
  if (!dateOfBirth) return null;
  const dob = new Date(dateOfBirth);
  const now = new Date();
  let age = now.getFullYear() - dob.getFullYear();
  const m = now.getMonth() - dob.getMonth();
  if (m < 0 || (m === 0 && now.getDate() < dob.getDate())) age--;
  return age;
}

function toLocalDateKey(date: Date): string {
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
}
