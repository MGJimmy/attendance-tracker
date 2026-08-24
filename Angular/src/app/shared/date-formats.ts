export const EGYPT_TIMEZONE = 'Africa/Cairo';
export const DATE_TIME_FORMAT = 'dd/MM/yyyy HH:mm';
export const DATE_FORMAT = 'dd/MM/yyyy';

export function toEgyptDateTimeInput(value?: string | Date | null): string {
  const date = value ? new Date(value) : new Date();
  const parts = new Intl.DateTimeFormat('en-GB', {
    timeZone: EGYPT_TIMEZONE,
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false
  }).formatToParts(date);

  const read = (type: string) => parts.find(part => part.type === type)?.value ?? '00';
  return `${read('year')}-${read('month')}-${read('day')}T${read('hour')}:${read('minute')}`;
}

/** Formats a .NET TimeSpan string as hours:minutes, with no seconds. */
export function formatHoursMinutes(value: string | null | undefined): string {
  if (value == null || value === '') {
    return '-';
  }

  const match = /^\s*(-)?(?:(\d+)\.)?(\d+):(\d+)(?::(\d+)(?:\.\d+)?)?\s*$/.exec(value);
  if (!match) {
    return value;
  }

  const negative = match[1] === '-';
  let hours = Number(match[2] || 0) * 24 + Number(match[3]);
  let minutes = Number(match[4]);
  const seconds = Number(match[5] || 0);

  if (seconds >= 30) {
    minutes += 1;
  }
  if (minutes >= 60) {
    hours += Math.floor(minutes / 60);
    minutes = minutes % 60;
  }

  const sign = negative ? '-' : '';
  return `${sign}${hours}:${String(minutes).padStart(2, '0')}`;
}
