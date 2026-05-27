import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EncounterDetailComponent } from './encounter-detail.component';

describe('EncounterDetailComponent', () => {
  let component: EncounterDetailComponent;
  let fixture: ComponentFixture<EncounterDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EncounterDetailComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(EncounterDetailComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
