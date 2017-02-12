using System;
using System.Collections.Generic;
using System.Linq;

namespace TheatreProject.Helpers
{
    public class Paginator<T>
    {
        private int currentPage;
        private IList<T> items;
        private int totalPages;

        public IList<T> Items
        {
            get { return items; }
        }

        public bool HasNextPage
        {
            get { return currentPage < totalPages; }
        }

        public bool HasPreviousPage
        {
            get { return currentPage > 0; }
        }

        public int NextPageNumber
        {
            get { return currentPage + 1; }
        }

        public int PreviousPageNumber
        {
            get { return currentPage - 1; }
        }

        public Paginator(IQueryable<T> items, int currentPage, int postsPerPage)
        {
            this.currentPage = currentPage;
            this.totalPages = (int)Math.Ceiling((double)(items.Count() / postsPerPage));
            int itemsToSkip = this.currentPage * postsPerPage;
            this.items = items.Skip(itemsToSkip).Take(postsPerPage).ToList();
        }
    }
}