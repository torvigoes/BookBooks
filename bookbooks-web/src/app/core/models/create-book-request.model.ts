export interface CreateBookRequest {
  title: string;
  author: string;
  isbn: string;
  year: number;
  coverImageUrl: string | null;
}
