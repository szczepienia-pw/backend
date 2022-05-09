using System;
using System.Diagnostics.CodeAnalysis;
using backend.Database;
using backend.Exceptions;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Helpers
{
	public static class EnumberableExtensions
	{
        [ExcludeFromCodeCoverage]
        public static void CheckDuplicate<TSource>(this DbSet<TSource> source, Func<TSource, bool> predicate, BasicException exception, string[]? relatedFields = null)
            where TSource : BaseModel
        {
            if (source != null)
            {
                var loadedSource = source.LoadFields(relatedFields);
                var elements = loadedSource.Where(predicate);

                if (elements != null && elements.Any())
                    throw exception;
            }
        }

        [ExcludeFromCodeCoverage]
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

