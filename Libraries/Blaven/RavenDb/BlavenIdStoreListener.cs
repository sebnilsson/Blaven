using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Listeners;
using Raven.Json.Linq;

namespace Blaven.RavenDb {
    public class BlavenIdStoreListener : IDocumentStoreListener {
        HiLoKeyGenerator _generator;
        IDocumentStore _store;
        public BlavenIdStoreListener(IDocumentStore store) {
            this._store = store;
            _generator = new HiLoKeyGenerator(store.DatabaseCommands, "BlogPosts", 1);
        }
        
        public void AfterStore(string key, object entityInstance, RavenJObject metadata) {
            
        }

        public bool BeforeStore(string key, object entityInstance, RavenJObject metadata, RavenJObject original) {
            var blogPost = entityInstance as BlogPost;
            if(blogPost != null && blogPost.BlavenId == 0) {
                string documentKey = _generator.GenerateDocumentKey(_store.Conventions, entityInstance);
                blogPost.BlavenId = int.Parse(documentKey.Substring(documentKey.IndexOf("/") + 1));
                return true;
            }
            return false;
        }
    }
}
