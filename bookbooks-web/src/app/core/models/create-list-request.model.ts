import { ListVisibility } from './list-visibility.enum';

export interface CreateListRequest {
  name: string;
  description: string | null;
  visibility: ListVisibility;
}
