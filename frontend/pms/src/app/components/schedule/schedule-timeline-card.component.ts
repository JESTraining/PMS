import { Component, ChangeDetectionStrategy, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DayTimelineComponent } from './day-timeline.component';
import { DoctorOption } from '../../models/doctor.model';
import { DaySummary, ScheduleView } from '../../models/schedule.model';

@Component({
  selector: 'app-schedule-timeline-card',
  standalone: true,
  imports: [CommonModule, DayTimelineComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrl: './schedule-timeline-card.component.scss',
  template: `
    <div class="sch-tl-card-head">

      <!-- Doctor identity — the only thing that drives header height -->
      <div class="sch-tl-head-identity">
        <h2 class="ui-card-title">{{ doc().name }}</h2>
        @if (doc().specialty) { <span class="ui-card-meta">{{ doc().specialty }}</span> }
        @if (isDefaultView()) {
          <p class="sch-tl-default-hint">No doctor selected — showing the next available {{ doc().specialty || 'General Medicine' }} slot.</p>
        }
      </div>

      <!-- Corner meta: section tag + stat pills + date badge.
           On desktop this is lifted to position:absolute in the card's top-right corner
           so it takes up zero layout height, letting the header be as slim as possible. -->
      <div class="sch-tl-corner-meta">
        <span class="sch-tl-corner-tag">
          @if (isDefaultView()) { Next available } @else { Timeline }
        </span>
        @let s = summary();
        <span class="sch-tl-stat">
          <span class="sch-tl-stat-n">{{ s.total }}</span>
          <span class="sch-tl-stat-cap">booked</span>
        </span>
        <span class="sch-tl-stat">
          <span class="sch-tl-stat-n sch-tl-stat-n--free">{{ s.free }}</span>
          <span class="sch-tl-stat-cap">free</span>
        </span>
        <div class="sch-tl-date-badge">
          <span class="material-icons" style="font-size:0.875rem;line-height:1;">event</span>
          {{ date() | date: 'EEE, MMM d' }}
        </div>
      </div>

    </div>
    <div class="sch-tl-scroll">
      <app-day-timeline
        [events]="events()"
        [date]="date()"
        [loading]="loading()"
        (slotSelected)="slotSelected.emit($event)"
        (reschedule)="reschedule.emit($event)"
        (dropBlocked)="dropBlocked.emit($event)"
        (editRequested)="editRequested.emit($event)" />
    </div>
  `,
})
export class ScheduleTimelineCardComponent {
  readonly doc = input.required<DoctorOption>();
  readonly isDefaultView = input.required<boolean>();
  readonly summary = input.required<DaySummary>();
  readonly date = input.required<Date>();
  readonly events = input<ScheduleView[]>([]);
  readonly loading = input<boolean>(false);

  readonly slotSelected = output<string | null>();
  readonly reschedule = output<{ id: number; start: string }>();
  readonly dropBlocked = output<string>();
  readonly editRequested = output<ScheduleView>();
}
