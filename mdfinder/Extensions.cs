using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mdfinder
{
    public static class Extensions
    {
        /// <summary> Enumerates <paramref name="items"/> into bins of size <paramref name="binSize"/>. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="items">   The items to act on. </param>
        /// <param name="binSize"> Size of the bin. </param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process bin in this collection.
        /// </returns>
        /// <remarks> Thanks to @juharr at Stack Overflow. https://stackoverflow.com/a/32970228/1210377 </remarks>
        public static IEnumerable<IEnumerable<T>> Bin<T>(this IEnumerable<T> items, int binSize)
        {
            if(binSize <= 0)
            {
                throw new ArgumentOutOfRangeException("binSize", Localization.Localization.BinSizeOutOfRangeExceptionMessage);
            }

            return items.Select((x, i) => new { x, i })
                        .GroupBy(a => a.i / binSize)
                        .Select(grp => grp.Select(a => a.x));
        }
    }
}
