using System;
using backend.Exceptions;

namespace backend.Helpers
{
	public static class EnumberableExtensions
	{
		public static void CheckDuplicate<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, BasicException exception)
        {
            if(source != null)
            {
                var elements = source.Where(predicate);

                if (elements != null && elements.Any())
                    throw exception;
            }
        }

        public static TSource FirstOrThrow<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, BasicException exception)
        {
            if (source == null)
                throw exception;

            var element = source.FirstOrDefault(predicate);

            if (element == null)
                throw exception;

            return element;
        }
    }
}

