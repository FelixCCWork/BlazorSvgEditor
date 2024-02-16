namespace BlazorSvgEditor.SvgEditor.Helper;
internal static class LinqExtensions
{
    // todo .NET 8 remove and replace with new build in method
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }
}
