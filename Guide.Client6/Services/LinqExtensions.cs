namespace Guide.Client6.Services
{
    public static class LinqExtensions
    {
        public static IEnumerable<TSource> ConditionalWhere<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }
    }
}