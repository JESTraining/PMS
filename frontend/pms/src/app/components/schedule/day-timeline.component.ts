import { Component, ChangeDetectionStrategy, input, output, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DragDropModule, CdkDragEnd } from '@angular/cdk/drag-drop';
import { ScheduleView } from '../../models/schedule.model';
import { PositionedEvent } from '../../models/timeline.model';
import {
  DAY_TOTAL_MINUTES,
  SLOT_MINUTES,
  generateDaySlots,
  minutesFromIso,
  minutesToLabel,
  timeFromIso,
  slotState,
  nowMinutes,
  isMinutesPast,
  isBeforeDay,
} from '../../utils/schedule-time.util';

const PX_PER_MIN = 1.8; // 30-min slot = 54px

/**
 * Proportional day timeline (08:00–18:00, 30-min slots). Empty, non-past slots are
 * clickable to start a booking; appointments render as duration-proportional blocks
 * that can be dragged to another free slot to reschedule.
 */
@Component({
  selector: 'app-day-timeline',
  standalone: true,
  imports: [CommonModule, DragDropModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrl: './day-timeline.component.scss',
  template: `
    <div class="sch-tl">
      <!-- Time gutter -->
      <div class="sch-tl-gutter">
        @for (slot of slots; track slot.minutes) {
          <div class="sch-tl-gutter-cell" [style.height.px]="slotHeight">{{ slot.label }}</div>
        }
      </div>

      <!-- Track -->
      <div class="timeline-track sch-tl-track" [style.height.px]="totalHeight">
        @if (loading()) {
          <div class="sch-tl-loading">Loading schedule…</div>
        }

        <!-- Past region shading -->
        @if (pastHeight() > 0) {
          <div class="sch-tl-past" [style.height.px]="pastHeight()"></div>
        }

        <!-- Slot backgrounds (booking targets) -->
        @for (slot of slots; track slot.minutes) {
          @let state = stateOf(slot.minutes);
          <button type="button"
                  class="sch-tl-slot"
                  [style.top.px]="slot.minutes * pxPerMin"
                  [style.height.px]="slotHeight"
                  [disabled]="state !== 'free'"
                  [attr.title]="state === 'past' ? 'Past — cannot book' : null"
                  (click)="slotSelected.emit(slot.label)">
            @if (state === 'free') {
              <span class="sch-tl-slot-free">Free — click to book</span>
            } @else if (state === 'past') {
              <span class="sch-tl-slot-past">Past</span>
            }
          </button>
        }

        <!-- Event blocks -->
        @for (pe of positionedEvents(); track pe.ref.type + '-' + pe.ref.id) {
          <div class="sch-tl-event"
               [ngClass]="blockClasses(pe)"
               [class.sch-tl-event--draggable]="isDraggable(pe)"
               [style.top.px]="pe.top"
               [style.height.px]="pe.height"
               cdkDrag
               cdkDragLockAxis="y"
               cdkDragBoundary=".timeline-track"
               [cdkDragDisabled]="!isDraggable(pe)"
               (cdkDragStarted)="onDragStarted()"
               (cdkDragEnded)="onDragEnded($event, pe)"
               (click)="onBlockClick(pe)">
            <div class="sch-tl-event-head">
              <span class="sch-tl-event-time">{{ pe.timeLabel }}</span>
              <span class="sch-tl-event-status">{{ pe.isRest ? 'Rest' : pe.status }}</span>
            </div>

            @if (pe.isRest) {
              <p class="sch-tl-event-text">{{ pe.ref.scheduleDescription }}</p>
            } @else {
              <p class="sch-tl-event-text"><strong>{{ pe.ref.patientName }}</strong> · {{ pe.ref.scheduleDescription }}</p>
            }
          </div>
        }

        <!-- Current-time indicator -->
        @if (nowOffset() !== null) {
          <div class="sch-tl-now" [style.top.px]="nowOffset()"><span class="sch-tl-now-dot"></span></div>
        }
      </div>
    </div>
  `,
})
export class DayTimelineComponent {
  readonly events = input<ScheduleView[]>([]);
  readonly date = input.required<Date>();
  readonly loading = input<boolean>(false);

  readonly slotSelected = output<string>();
  readonly reschedule = output<{ id: number; start: string }>();
  readonly dropBlocked = output<string>();
  readonly editRequested = output<ScheduleView>();

  readonly slots = generateDaySlots();
  readonly pxPerMin = PX_PER_MIN;
  readonly slotHeight = SLOT_MINUTES * PX_PER_MIN;
  readonly totalHeight = DAY_TOTAL_MINUTES * PX_PER_MIN;

  private readonly now = signal(new Date());

  readonly positionedEvents = computed<PositionedEvent[]>(() =>
    this.events().map((ref) => {
      const startMin = minutesFromIso(ref.startTime);
      const endMin = minutesFromIso(ref.endTime);
      return {
        ref,
        top: Math.max(0, startMin) * PX_PER_MIN,
        height: Math.max(SLOT_MINUTES, endMin - startMin) * PX_PER_MIN,
        isRest: ref.type === 'Rest',
        status: ref.scheduleStatus,
        timeLabel: `${timeFromIso(ref.startTime)} – ${timeFromIso(ref.endTime)}`,
      };
    }),
  );

  readonly nowOffset = computed<number | null>(() => {
    const m = nowMinutes(this.date(), this.now());
    if (m === null || m < 0 || m > DAY_TOTAL_MINUTES) return null;
    return m * PX_PER_MIN;
  });

  /** Height of the shaded "past" region: full day for past dates, up to now for today, 0 for future. */
  readonly pastHeight = computed<number>(() => {
    const date = this.date();
    const now = this.now();
    if (isBeforeDay(date, now)) return this.totalHeight;
    const m = nowMinutes(date, now);
    if (m === null) return 0; // future day
    return Math.min(Math.max(m, 0), DAY_TOTAL_MINUTES) * PX_PER_MIN;
  });

  stateOf(slotMinutes: number) {
    return slotState(this.events(), slotMinutes, this.date(), this.now());
  }

  isDraggable(pe: PositionedEvent): boolean {
    return !pe.isRest && pe.status === 'Scheduled';
  }

  private wasDragged = false;

  onDragStarted(): void {
    this.wasDragged = true;
  }

  /** Open the edit dialog on a plain click (ignored right after a drag). */
  onBlockClick(pe: PositionedEvent): void {
    if (this.wasDragged) return; // suppress the click that follows a drag
    if (!pe.isRest && pe.status === 'Scheduled') {
      this.editRequested.emit(pe.ref);
    }
  }

  /** Snap a vertical drag to the nearest 30-min slot, validate, and emit a reschedule. */
  onDragEnded(event: CdkDragEnd, pe: PositionedEvent): void {
    const slotDelta = Math.round(event.distance.y / this.slotHeight);
    event.source.reset(); // return to origin; a successful move re-renders via reload
    setTimeout(() => (this.wasDragged = false)); // clear after the trailing click is suppressed

    if (slotDelta === 0) return;

    const startMin = minutesFromIso(pe.ref.startTime);
    const duration = minutesFromIso(pe.ref.endTime) - startMin;
    const newStart = startMin + slotDelta * SLOT_MINUTES;

    if (newStart < 0 || newStart + duration > DAY_TOTAL_MINUTES) {
      this.dropBlocked.emit('That time is outside working hours.');
      return;
    }
    if (isMinutesPast(newStart, this.date(), this.now())) {
      this.dropBlocked.emit('Cannot move an appointment to a past time.');
      return;
    }

    const conflict = this.events().some((e) => {
      if (e.id === pe.ref.id && e.type === pe.ref.type) return false; // ignore self
      const s = minutesFromIso(e.startTime);
      const en = minutesFromIso(e.endTime);
      return s < newStart + duration && newStart < en;
    });
    if (conflict) {
      this.dropBlocked.emit('That slot is not available.');
      return;
    }

    this.reschedule.emit({ id: pe.ref.id, start: minutesToLabel(newStart) });
  }

  blockClasses(pe: PositionedEvent): string {
    if (pe.isRest) return 'sch-tl-event--rest';
    switch (pe.status) {
      case 'InProgress': return 'sch-tl-event--inprogress';
      case 'Completed': return 'sch-tl-event--completed';
      default: return 'sch-tl-event--scheduled';
    }
  }
}
