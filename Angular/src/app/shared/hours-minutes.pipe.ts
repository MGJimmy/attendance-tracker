import { Pipe, PipeTransform } from '@angular/core';
import { formatHoursMinutes } from './date-formats';

@Pipe({
  name: 'hoursMinutes',
  standalone: true
})
export class HoursMinutesPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    return formatHoursMinutes(value);
  }
}
