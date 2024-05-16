using System;
using System.Collections.Generic;
using System.Linq;

namespace Baetoti.API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }

        public int TotalPages { get; private set; }

        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }

        public bool HasPrevious => CurrentPage > 1;

        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public static PagedList<T> ToPagedList(IList<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        public object Metadata(object obj)
        {
            return new
            {
                TotalCount = obj.GetType().GetProperty("TotalCount").GetValue(obj, null),
                PageSize = obj.GetType().GetProperty("PageSize").GetValue(obj, null),
                CurrentPage = obj.GetType().GetProperty("CurrentPage").GetValue(obj, null),
                TotalPages = obj.GetType().GetProperty("TotalPages").GetValue(obj, null),
                HasNext = obj.GetType().GetProperty("HasNext").GetValue(obj, null),
                HasPrevious = obj.GetType().GetProperty("HasPrevious").GetValue(obj, null)
            };
        }

    }
}
