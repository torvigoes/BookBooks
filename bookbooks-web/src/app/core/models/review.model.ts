export interface Review {
  id: string;
  bookId: string;
  userId: string;
  userDisplayName: string;
  rating: number;
  content: string;
  containsSpoiler: boolean;
  createdAt: string;
}
