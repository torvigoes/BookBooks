namespace BookBooks.Domain.Enums;

/// <summary>
/// Represents the user's reading status for a specific book.
/// </summary>
public enum ReadingStatusType
{
    WantToRead,
    CurrentlyReading,
    Read,
    Abandoned
}

/// <summary>
/// Defines the visibility level of a user's curated book list.
/// </summary>
public enum ListVisibility
{
    Public,
    Private,
    FriendsOnly
}
