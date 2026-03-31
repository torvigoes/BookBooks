using BookBooks.Application.Features.Lists.DTOs;
using BookBooks.Domain.Entities;

namespace BookBooks.Application.Features.Lists;

internal static class ListMapping
{
    public static BookListDto ToDto(BookList list)
    {
        var items = list.Items
            .OrderBy(i => i.Order)
            .Select(i => new BookListItemDto(
                i.BookId,
                i.Book?.Title ?? string.Empty,
                i.Book?.Author ?? string.Empty,
                i.Order,
                i.Notes))
            .ToList()
            .AsReadOnly();

        return new BookListDto(
            list.Id,
            list.UserId,
            list.Name,
            list.Description,
            list.Visibility,
            list.CreatedAt,
            items.Count,
            items);
    }
}
