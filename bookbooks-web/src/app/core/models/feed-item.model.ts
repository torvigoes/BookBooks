export interface FeedItem {
  reviewId: string;
  bookId: string;
  bookTitle: string;
  bookAuthor: string;
  bookCoverImageUrl: string | null;
  userId: string;
  userDisplayName: string;
  rating: number;
  content: string;
  containsSpoiler: boolean;
  createdAt: string;
}
