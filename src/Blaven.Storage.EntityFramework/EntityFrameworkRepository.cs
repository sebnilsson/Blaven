using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blaven.DataStorage.EntityFramework
{
    public class EntityFrameworkRepository : IRepository
    {
        private readonly BlavenDbContext dbContext;

        public EntityFrameworkRepository(BlavenDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            this.dbContext = dbContext;
        }

        public IQueryable<BlogMeta> GetBlogMetas()
        {
            var metas = this.dbContext.BlogMetas.OrderBy(x => x.Name);
            return metas;
        }

        public Task<BlogMeta> GetBlogMeta(string blogKey)
        {
            throw new NotImplementedException();
        }

        public Task<BlogPost> GetPost(string blogKey, string blavenId)
        {
            throw new NotImplementedException();
        }

        public Task<BlogPost> GetPostBySourceId(string blogKey, string sourceId)
        {
            throw new NotImplementedException();
        }

        public IQueryable<BlogArchiveItem> ListArchive(IEnumerable<string> blogKeys)
        {
            throw new NotImplementedException();
        }

        public IQueryable<BlogTagItem> ListTags(IEnumerable<string> blogKeys)
        {
            throw new NotImplementedException();
        }

        public IQueryable<BlogPostHead> ListPostHeads(IEnumerable<string> blogKeys)
        {
            throw new NotImplementedException();
        }

        public IQueryable<BlogPost> ListPosts(IEnumerable<string> blogKeys)
        {
            throw new NotImplementedException();
        }

        public IQueryable<BlogPost> ListPostsByArchive(IEnumerable<string> blogKeys, DateTime archiveDate)
        {
            throw new NotImplementedException();
        }

        public IQueryable<BlogPost> ListPostsByTag(IEnumerable<string> blogKeys, string tagName)
        {
            throw new NotImplementedException();
        }

        public IQueryable<BlogPost> SearchPosts(IEnumerable<string> blogKeys, string search)
        {
            throw new NotImplementedException();
        }
    }
}
