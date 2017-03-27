using Edubase.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Data.Entity;

namespace Edubase.Services
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Throws an EntityNotFoundException if the entity isn't found.
        /// Single() would throw the 'wrong' exception type; i.e., we want a domain exception that's recognised as expected.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TSource SingleOrThrow<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var retVal = source.SingleOrDefault(predicate);
            if (retVal == null) throw new EntityNotFoundException();
            return retVal;
        }

        public static async Task<TSource> SingleOrThrowAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var retVal = await source.SingleOrDefaultAsync(predicate);
            if (retVal == null) throw new EntityNotFoundException();
            return retVal;
        }

        public static TSource SingleOrThrow<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<Exception> exceptionFactory)
        {
            var retVal = source.SingleOrDefault(predicate);
            if (retVal == null) throw exceptionFactory();
            return retVal;
        }
        

    }
}
