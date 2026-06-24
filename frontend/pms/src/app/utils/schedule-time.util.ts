/** Shared time helpers for the daily schedule (08:00–18:00, fixed 30-min slots). */

import { DaySlot, SlotState, TimeRange } from '../models/timeline.model';

export const DAY_START_HOUR = 8;
export const DAY_END_HOUR = 18;
export const SLOT_MINUTES = 30;
export const DAY_TOTAL_MINUTES = (DAY_END_HOUR - DAY_START_HOUR) * 60;

export function generateDaySlots(): DaySlot[] {
  const slots: DaySlot[] = [];
  for (let m = 0; m < DAY_TOTAL_MINUTES; m += SLOT_MINUTES) {
    slots.push({ label: minutesToLabel(m), minutes: m });
  }
  return slots;
}

export function minutesToLabel(minutes: number): string {
  const h = DAY_START_HOUR + Math.floor(minutes / 60);
  return `${pad(h)}:${pad(minutes % 60)}`;
}

/** 'HH:mm' from a 'YYYY-MM-DDTHH:mm:ss' string. */
export function timeFromIso(iso: string): string {
  return (iso.split('T')[1] ?? '00:00').slice(0, 5);
}

/** Minutes from DAY_START_HOUR for a 'YYYY-MM-DDTHH:mm:ss' string. */
export function minutesFromIso(iso: string): number {
  const t = iso.split('T')[1] ?? '00:00';
  const [h, m] = t.split(':').map(Number);
  return (h - DAY_START_HOUR) * 60 + m;
}

/** Adds minutes to an 'HH:mm' label and returns 'HH:mm'. */
export function addMinutesToLabel(label: string, minutes: number): string {
  const [h, m] = label.split(':').map(Number);
  const total = h * 60 + m + minutes;
  return `${pad(Math.floor(total / 60))}:${pad(total % 60)}`;
}

export function isSameDay(a: Date, b: Date): boolean {
  return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate();
}

export function isBeforeDay(a: Date, b: Date): boolean {
  const da = new Date(a.getFullYear(), a.getMonth(), a.getDate()).getTime();
  const db = new Date(b.getFullYear(), b.getMonth(), b.getDate()).getTime();
  return da < db;
}

/** Minutes-from-start of `now` if `date` is today, else null. */
export function nowMinutes(date: Date, now: Date): number | null {
  if (!isSameDay(date, now)) return null;
  return (now.getHours() - DAY_START_HOUR) * 60 + now.getMinutes();
}

/** Whether a slot is free, occupied by an event, or in the past. */
export function slotState(events: TimeRange[], slotMinutes: number, date: Date, now: Date): SlotState {
  const slotEnd = slotMinutes + SLOT_MINUTES;
  const busy = events.some((e) => {
    const s = minutesFromIso(e.startTime);
    const en = minutesFromIso(e.endTime);
    return s < slotEnd && slotMinutes < en;
  });
  if (busy) return 'busy';
  if (isMinutesPast(slotMinutes, date, now)) return 'past';
  return 'free';
}

/** Whether a minutes-from-start value is in the past for the given day. */
export function isMinutesPast(minutes: number, date: Date, now: Date): boolean {
  if (isBeforeDay(date, now)) return true;
  const nm = nowMinutes(date, now);
  return nm !== null && minutes < nm;
}

function pad(n: number): string {
  return String(n).padStart(2, '0');
}
