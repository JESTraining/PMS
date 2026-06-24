import { Component, ChangeDetectionStrategy, input } from '@angular/core';
import { DaySummary } from '../../models/schedule.model';

@Component({
  selector: 'app-schedule-day-overview',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrl: './schedule-day-overview.component.scss',
  template: `
    @let s = summary();
    <h3 class="ui-card-title">Day overview</h3>
    <div class="sch-summary-stats">
      <div class="sch-summary-stat">
        <span class="sch-summary-num">{{ s.total }}</span>
        <span class="sch-summary-cap">Booked</span>
      </div>
      <div class="sch-summary-stat">
        <span class="sch-summary-num">{{ s.free }}</span>
        <span class="sch-summary-cap">Free slots</span>
      </div>
    </div>
    <ul class="sch-legend">
      <li><span class="sch-legend-dot sch-legend-dot--scheduled"></span> Scheduled <span class="sch-legend-n">{{ s.scheduled }}</span></li>
      <li><span class="sch-legend-dot sch-legend-dot--inprogress"></span> In progress <span class="sch-legend-n">{{ s.inProgress }}</span></li>
      <li><span class="sch-legend-dot sch-legend-dot--completed"></span> Completed <span class="sch-legend-n">{{ s.completed }}</span></li>
      <li>
        <span class="sch-legend-dot sch-legend-dot--rest"></span>
        Rest period
        <span class="sch-legend-n">{{ s.restRange ?? '—' }}</span>
      </li>
    </ul>
    <p class="sch-side-tip">Click a free slot, or use "New appointment", to book.</p>
  `,
})
export class ScheduleDayOverviewComponent {
  readonly summary = input.required<DaySummary>();
}
