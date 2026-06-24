import { Component, ChangeDetectionStrategy, input, output } from '@angular/core';
import { NextSlot } from '../../models/schedule.model';

@Component({
  selector: 'app-schedule-next-available',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrl: './schedule-next-available.component.scss',
  template: `
    <div class="sch-na-head">
      <h3 class="ui-card-title">Next available</h3>
      <span class="ui-pill">{{ doctorFirstName() }}</span>
    </div>
    <ul class="sch-na-list">
      @for (slot of slots(); track slot.key) {
        <li class="sch-na-item" (click)="slotJumped.emit(slot)">
          <span class="sch-na-date">{{ slot.dateLabel }}</span>
          <span class="sch-na-time">
            <span class="material-icons" style="font-size:0.75rem;line-height:1;">schedule</span>
            {{ slot.time }}
          </span>
        </li>
      }
    </ul>
  `,
})
export class ScheduleNextAvailableComponent {
  readonly slots = input<NextSlot[]>([]);
  readonly doctorFirstName = input.required<string>();

  readonly slotJumped = output<NextSlot>();
}
