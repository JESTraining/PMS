import { Component, ChangeDetectionStrategy, input, output } from '@angular/core';
import { DoctorOption, GpOption } from '../../models/doctor.model';

@Component({
  selector: 'app-schedule-gm-shortcut',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrl: './schedule-gm-shortcut.component.scss',
  template: `
    <div class="sch-qp-head">
      <h3 class="sch-qp-ref-title">General Medicine</h3>
      <span class="sch-qp-ref-badge">Shortcut</span>
    </div>
    @if (doctors().length === 0) {
      <p class="sch-qp-empty">No General Medicine doctors available.</p>
    } @else {
      <ul class="sch-qp-list">
        @for (gp of doctors(); track gp.id) {
          <li class="sch-qp-item" [class.sch-qp-item--active]="activeDoctorId() === gp.id"
              (click)="doctorSelected.emit(gp)">
            <span class="sch-qp-name">{{ gp.name }}</span>
            <span class="sch-qp-slot">
              <span class="material-icons" style="font-size:0.875rem;line-height:1;">schedule</span>
              {{ gp.nextSlot }}
            </span>
          </li>
        }
      </ul>
    }
  `,
})
export class ScheduleGmShortcutComponent {
  readonly doctors = input<GpOption[]>([]);
  readonly activeDoctorId = input.required<number>();

  readonly doctorSelected = output<DoctorOption>();
}
