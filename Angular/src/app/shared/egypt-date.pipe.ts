import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';
import { DATE_TIME_FORMAT, EGYPT_TIMEZONE } from './date-formats';

@Pipe({
  name: 'egyptDate',
  standalone: true
})
export class EgyptDatePipe implements PipeTransform {
  private readonly datePipe = new DatePipe('en-GB');

  transform(
    value: Date | string | number | null | undefined,
    format: string = DATE_TIME_FORMAT
  ): string {
    if (value == null || value === '') {
      return '-';
    }

    return this.datePipe.transform(value, format, EGYPT_TIMEZONE) ?? '-';
  }
}
