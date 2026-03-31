import { ListVisibility } from './list-visibility.enum';
import { BookListItem } from './book-list-item.model';

export interface BookList {
  id: string;
  userId: string;
  name: string;
  description: string | null;
  visibility: ListVisibility;
  createdAt: string;
  itemCount: number;
  items: BookListItem[];
}
