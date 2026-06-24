import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoctorScheduleCardComponent } from './doctor-schedule-card.component';

describe('DoctorScheduleCardComponent', () => {
  let component: DoctorScheduleCardComponent;
  let fixture: ComponentFixture<DoctorScheduleCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DoctorScheduleCardComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(DoctorScheduleCardComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
