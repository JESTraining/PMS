import { Component, ChangeDetectionStrategy, input, output, computed, linkedSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ScheduleView } from '../../models/schedule.model';
import { timeFromIso, addMinutesToLabel } from '../../utils/schedule-time.util';

/**
 * Appointment details modal. Patient and reason are read-only (locked); the time can be
 * changed (saving emits `save`). Hosts the per-appointment actions: start encounter, cancel.
 */
@Component({
  selector: 'app-appointment-edit',
  standalone: true,
  imports: [CommonModule, FormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="ui-modal-backdrop" (click)="cancel.emit()">
      <div class="ui-modal" (click)="$event.stopPropagation()">
        <div class="ui-modal-head">
          <h3 class="ui-modal-title">Appointment details</h3>
          <button class="ui-close" (click)="cancel.emit()" aria-label="Close">✕</button>
        </div>

        <div class="ui-modal-body">
          <div>
            <span class="ui-label">Patient</span>
            <p class="m-0 text-sm" style="color: var(--pms-text-strong);">{{ appointment().patientName }}</p>
          </div>
          <div>
            <span class="ui-label">Reason</span>
            <p class="m-0 text-sm" style="color: var(--pms-text-strong);">{{ appointment().scheduleDescription || '—' }}</p>
          </div>

          <div>
            <label class="ui-label">Time</label>
            <select class="ui-select" [ngModel]="selectedTime()" (ngModelChange)="selectedTime.set($event)">
              @for (slot of options(); track slot) {
                <option [value]="slot">{{ slot }} – {{ endFor(slot) }}</option>
              }
            </select>
            <p class="m-0 mt-1 text-xs" style="color: var(--pms-muted-2);">Patient and reason can't be changed here.</p>
          </div>

          @if (isScheduled()) {
            <div class="flex gap-2">
              <button class="ui-btn ui-btn-primary" (click)="startEncounter.emit(appointment().id)">
                <span class="material-icons text-lg">play_arrow</span> Start encounter
              </button>
              <button class="ui-btn ui-btn-soft-danger" (click)="cancelAppointment.emit(appointment().id)">Cancel appointment</button>
            </div>
          }
        </div>

        <div class="ui-modal-foot">
          <button class="ui-btn ui-btn-outline" (click)="cancel.emit()">Close</button>
          <button class="ui-btn ui-btn-primary" (click)="onSave()" [disabled]="!changed()">Save changes</button>
        </div>
      </div>
    </div>
  `,
})
export class AppointmentEditComponent {
  readonly appointment = input.required<ScheduleView>();
  readonly availableSlots = input<string[]>([]);

  readonly save = output<{ id: number; start: string }>();
  readonly cancel = output<void>();
  readonly startEncounter = output<number>();
  readonly cancelAppointment = output<number>();

  readonly isScheduled = computed(() => this.appointment().scheduleStatus === 'Scheduled');
  readonly currentStart = computed(() => timeFromIso(this.appointment().startTime));

  /** Free slots plus the appointment's own current time (which otherwise reads as busy). */
  readonly options = computed(() => {
    const set = new Set<string>([this.currentStart(), ...this.availableSlots()]);
    return [...set].sort();
  });

  readonly selectedTime = linkedSignal<string>(() => this.currentStart());
  readonly changed = computed(() => this.selectedTime() !== this.currentStart());

  endFor(slot: string): string {
    return addMinutesToLabel(slot, 30);
  }

  onSave(): void {
    if (!this.changed()) return;
    this.save.emit({ id: this.appointment().id, start: this.selectedTime() });
  }
}
