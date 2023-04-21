namespace cleaner.Domain.Helpers;

public static class CollectionHelpers
{
    public static bool IsNullOrEmpty<T>(IEnumerable<T>? collection)
    {
        if (collection == null)
            return true;

        return collection?.Any() == false;
    }
}