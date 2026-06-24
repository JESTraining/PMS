import { Component, ChangeDetectionStrategy, input, output, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DoctorOption, SpecialtyGroup } from '../../models/doctor.model';

/**
 * Active doctor search: free-text by name/ID, with specialty quick-filter chips and
 * results grouped by specialty. Filters the preloaded doctor list client-side.
 */
@Component({
  selector: 'app-doctor-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrl: './doctor-search.component.scss',
  template: `
    @if (selected(); as doc) {
      <div class="sch-ds-selected">
        <div class="sch-ds-avatar">{{ initials(doc) }}</div>
        <div class="sch-ds-selected-info">
          <span class="sch-ds-selected-name">{{ doc.name }}</span>
          @if (doc.specialty) {
            <span class="sch-ds-selected-specialty">{{ doc.specialty }}</span>
          }
        </div>
        <button class="sch-ds-selected-change" (click)="cleared.emit()">Change</button>
      </div>
    } @else {
      <div class="sch-ds">
        <input type="text" class="ui-input" [(ngModel)]="term" (focus)="open.set(true)"
               placeholder="Search doctor by name or ID…" />

        @if (specialties().length > 1) {
          <div class="sch-ds-chips">
            @for (s of specialties(); track s) {
              <button class="ui-chip" [class.ui-chip--active]="specialtyFilter() === s"
                      (click)="toggleSpecialty(s)">{{ s }}</button>
            }
          </div>
        }

        @if (open() && grouped().length > 0) {
          <div class="sch-ds-dropdown">
            @for (group of grouped(); track group.specialty) {
              <div class="sch-ds-group-label">{{ group.specialty }}</div>
              @for (d of group.doctors; track d.id) {
                <button class="sch-ds-result" (click)="pick(d)">
                  <div class="sch-ds-result-avatar">{{ initials(d) }}</div>
                  <div class="sch-ds-result-body">
                    <strong>{{ d.name }}</strong>
                    <span class="sch-ds-result-id">ID {{ d.id }}</span>
                  </div>
                </button>
              }
            }
          </div>
        }
      </div>
    }
  `,
})
export class DoctorSearchComponent {
  readonly doctors = input<DoctorOption[]>([]);
  readonly selected = input<DoctorOption | null>(null);

  readonly doctorSelected = output<DoctorOption>();
  readonly cleared = output<void>();

  readonly term = signal('');
  readonly specialtyFilter = signal<string | null>(null);
  readonly open = signal(false);

  readonly specialties = computed(() => {
    const set = new Set<string>();
    for (const d of this.doctors()) if (d.specialty) set.add(d.specialty);
    return [...set].sort();
  });

  private readonly filtered = computed(() => {
    const t = this.term().trim().toLowerCase();
    const sp = this.specialtyFilter();
    return this.doctors().filter(
      (d) => (!sp || d.specialty === sp) && (!t || d.name.toLowerCase().includes(t) || String(d.id).includes(t)),
    );
  });

  readonly grouped = computed<SpecialtyGroup[]>(() => {
    const groups = new Map<string, DoctorOption[]>();
    for (const d of this.filtered()) {
      const key = d.specialty ?? 'Other';
      (groups.get(key) ?? groups.set(key, []).get(key)!).push(d);
    }
    return [...groups.entries()].sort((a, b) => a[0].localeCompare(b[0])).map(([specialty, doctors]) => ({ specialty, doctors }));
  });

  initials(doc: DoctorOption): string {
    const parts = doc.name.split(' ').filter(Boolean);
    if (parts.length === 0) return '';
    if (parts.length === 1) return parts[0][0].toUpperCase();
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
  }

  toggleSpecialty(specialty: string): void {
    this.specialtyFilter.set(this.specialtyFilter() === specialty ? null : specialty);
    this.open.set(true);
  }

  pick(doctor: DoctorOption): void {
    this.doctorSelected.emit(doctor);
    this.open.set(false);
    this.term.set('');
    this.specialtyFilter.set(null);
  }
}
