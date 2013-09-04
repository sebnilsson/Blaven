using System;
using System.Collections;
using System.Web;

namespace Blaven
{
    public class RequestLazy<T>
    {
        private readonly Func<T> valueFactory;

        private readonly Func<IDictionary> storeFactory;

        internal RequestLazy(Func<T> valueFactory, string storeKey, Func<IDictionary> storeFactory)
        {
            this.storeFactory = storeFactory ?? (() => (HttpContext.Current != null) ? HttpContext.Current.Items : null);
            this.valueFactory = valueFactory ?? this.GetValue;
            this.StoreKey = string.Format("RequestLazy_{0}", storeKey ?? Guid.NewGuid().ToString());
        }

        public RequestLazy(string storeKey)
            : this(null, storeKey)
        {
        }

        public RequestLazy(Func<T> valueFactory, string storeKey = null)
            : this(valueFactory, storeKey, null)
        {
        }

        public string StoreKey { get; private set; }

        public T Value
        {
            get
            {
                if (this.ContextStore == null)
                {
                    return this.valueFactory();
                }

                if (!this.ContextStore.Contains(this.StoreKey))
                {
                    this.ContextStore[this.StoreKey] = this.valueFactory();
                }

                return this.GetValue();
            }
            set
            {
                if (this.ContextStore == null)
                {
                    return;
                }

                this.ContextStore[this.StoreKey] = value;
            }
        }

        private IDictionary ContextStore
        {
            get
            {
                return this.storeFactory();
            }
        }

        public bool IsValueCreated
        {
            get
            {
                return this.ContextStore != null && this.ContextStore.Contains(this.StoreKey);
            }
        }

        public override string ToString()
        {
            return this.IsValueCreated ? this.Value.ToString() : "(Value not created)";
        }

        private T GetValue()
        {
            var item = this.ContextStore[this.StoreKey];
            return item is T ? (T)item : default(T);
        }
    }
}