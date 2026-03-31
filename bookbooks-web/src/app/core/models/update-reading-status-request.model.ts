import { ReadingStatusType } from './reading-status-type.enum';

export interface UpdateReadingStatusRequest {
  status: ReadingStatusType;
}
