import { ScheduleView } from './schedule.model';

/** A 30-min slot in the day grid. */
export interface DaySlot {
  label: string;   // 'HH:mm'
  minutes: number; // minutes from the day start
}

/** State of a slot in the timeline. */
export type SlotState = 'free' | 'busy' | 'past';

/** Minimal start/end range used for overlap checks. */
export interface TimeRange {
  startTime: string;
  endTime: string;
}

/** A schedule event positioned (in px) on the proportional timeline. */
export interface PositionedEvent {
  ref: ScheduleView;
  top: number;
  height: number;
  isRest: boolean;
  status: string;
  timeLabel: string;
}
