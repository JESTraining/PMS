import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BillingReviewComponent } from './billing-review.component';

describe('BillingReviewComponent', () => {
  let component: BillingReviewComponent;
  let fixture: ComponentFixture<BillingReviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BillingReviewComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(BillingReviewComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
