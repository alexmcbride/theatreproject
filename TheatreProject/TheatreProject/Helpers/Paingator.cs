using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheatreProject.Helpers
{
    public class Paginator<T>
    {
        private int currentPage;
        private IQueryable<T> items;
        private int postsPerPage;
        private int itemsToSkip;
        private int totalPages;

        public IQueryable<T> Items
        {
            get { return items.Skip(itemsToSkip).Take(postsPerPage); }
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
            this.items = items;
            this.currentPage = currentPage;
            this.postsPerPage = postsPerPage;
            this.itemsToSkip = this.currentPage * postsPerPage;
            this.totalPages = (int)Math.Ceiling((double)(items.Count() / postsPerPage));
        }
    }
}