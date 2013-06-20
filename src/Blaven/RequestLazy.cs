using System;
using System.Collections;
using System.Web;

namespace Blaven
{
    public class RequestLazy<T>
    {
        private readonly string storeKey;

        private readonly Func<T> valueFactory;

        private readonly IDictionary customContextStore;

        internal RequestLazy(IDictionary customContextStore, Func<T> valueFactory, string storeKey = null)
            : this(valueFactory, storeKey)
        {
            this.customContextStore = customContextStore;
        }

        public RequestLazy(Func<T> valueFactory, string storeKey = null)
        {
            this.valueFactory = valueFactory;
            this.storeKey = storeKey ?? string.Format("ContextLazy_{0}", Guid.NewGuid());
        }

        public T Value
        {
            get
            {
                var contextStore = this.GetContextStore();
                if (contextStore == null)
                {
                    return this.valueFactory();
                }

                if (!contextStore.Contains(this.storeKey))
                {
                    contextStore[this.storeKey] = this.valueFactory();
                }

                var item = contextStore[this.storeKey];
                return item is T ? (T)item : default(T);
            }
        }

        public bool IsValueCreated
        {
            get
            {
                var contextStore = this.GetContextStore();
                return contextStore != null && contextStore.Contains(this.storeKey);
            }
        }

        public override string ToString()
        {
            return this.IsValueCreated ? this.Value.ToString() : string.Empty;
        }

        private IDictionary GetContextStore()
        {
            return this.customContextStore ?? (HttpContext.Current != null ? HttpContext.Current.Items : null);
        }
    }
}