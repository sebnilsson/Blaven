using System;
using System.Collections.Generic;

namespace Blaven.Synchronization
{
    public class SynchronizationResult
    {
        public SynchronizationResult()
        {
            this.DeletedPosts = new List<BlogPost>();
            this.DeletedPostTags = new Dictionary<string, ICollection<string>>();
            this.InsertedPosts = new List<BlogPost>();
            this.InsertedPostTags = new Dictionary<string, ICollection<string>>();
        }

        public List<BlogPost> DeletedPosts { get; private set; }

        public Dictionary<string, ICollection<string>> DeletedPostTags { get; private set; }

        public List<BlogPost> InsertedPosts { get; private set; }

        public Dictionary<string, ICollection<string>> InsertedPostTags { get; private set; }

        public void AddDeletedPostTags(string sourceId, params string[] tags)
        {
            AddToDictionaryCollection(this.DeletedPostTags, sourceId, tags);
        }

        public void AddInsertedPostTags(string sourceId, params string[] tags)
        {
            AddToDictionaryCollection(this.DeletedPostTags, sourceId, tags);
        }

        private static void AddToDictionaryCollection(
            Dictionary<string, ICollection<string>> dictionary,
            string sourceId,
            params string[] tags)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            if (sourceId == null)
            {
                throw new ArgumentNullException(nameof(sourceId));
            }

            var collection = dictionary.ContainsKey(sourceId)
                                 ? dictionary[sourceId]
                                 : (dictionary[sourceId] = new HashSet<string>());

            foreach (var tag in tags)
            {
                collection.Add(tag);
            }
        }
    }
}