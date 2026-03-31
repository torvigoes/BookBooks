export interface Book {
  id: string;
  title: string;
  author: string;
  isbn: string;
  year: number;
  coverImageUrl: string | null;
  averageRating: number;
  reviewCount: number;
}
