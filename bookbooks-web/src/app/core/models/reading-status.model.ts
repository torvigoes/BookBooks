import { ReadingStatusType } from './reading-status-type.enum';

export interface ReadingStatus {
  bookId: string;
  userId: string;
  status: ReadingStatusType;
  updatedAt: string;
}
