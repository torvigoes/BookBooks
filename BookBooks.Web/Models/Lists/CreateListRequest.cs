namespace BookBooks.Web.Models.Lists;

public sealed record CreateListRequest(
    string Name,
    string? Description,
    ListVisibility Visibility
);
