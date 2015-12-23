using System;

namespace Blaven.Tests
{
    public static class FuncExtensions
    {
        public static Func<TKey, TResult> WithTracking<TKey, TResult>(
            this Func<TKey, TResult> func,
            DelegateTracker<TKey> tracking)
        {
            if (tracking == null)
            {
                throw new ArgumentNullException(nameof(tracking));
            }

            func = func ?? (_ => default(TResult));

            Func<TKey, TResult> tracked = key =>
                {
                    var @event = tracking.AddEvent(key);

                    var result = func(key);
                    
                    @event.EndedAt = DateTime.Now;

                    return result;
                };
            return tracked;
        }

        public static Func<TKey, T2, TResult> WithTracking<TKey, T2, TResult>(
            this Func<TKey, T2, TResult> func,
            DelegateTracker<TKey> tracking)
        {
            if (tracking == null)
            {
                throw new ArgumentNullException(nameof(tracking));
            }

            func = func ?? ((_, __) => default(TResult));

            Func<TKey, T2, TResult> tracked = (key, arg2) =>
                {
                    var @event = tracking.AddEvent(key);

                    var result = func(key, arg2);

                    @event.EndedAt = DateTime.Now;

                    return result;
                };
            return tracked;
        }

        public static Func<TKey, T2, T3, TResult> WithTracking<TKey, T2, T3, TResult>(
            this Func<TKey, T2, T3, TResult> func,
            DelegateTracker<TKey> tracking)
        {
            if (tracking == null)
            {
                throw new ArgumentNullException(nameof(tracking));
            }

            func = func ?? ((_, __, ___) => default(TResult));

            Func<TKey, T2, T3, TResult> tracked = (key, arg2, arg3) =>
                {
                    var @event = tracking.AddEvent(key);

                    var result = func(key, arg2, arg3);

                    @event.EndedAt = DateTime.Now;

                    return result;
                };
            return tracked;
        }

        public static Func<TKey, T2, T3, T4, TResult> WithTracking<TKey, T2, T3, T4, TResult>(
            this Func<TKey, T2, T3, T4, TResult> func,
            DelegateTracker<TKey> tracking)
        {
            if (tracking == null)
            {
                throw new ArgumentNullException(nameof(tracking));
            }

            func = func ?? ((_, __, ___, ____) => default(TResult));

            Func<TKey, T2, T3, T4, TResult> tracked = (key, arg2, arg3, arg4) =>
                {
                    var @event = tracking.AddEvent(key);

                    var result = func(key, arg2, arg3, arg4);

                    @event.EndedAt = DateTime.Now;

                    return result;
                };
            return tracked;
        }

        public static Func<TKey, T2, T3, T4, T5, TResult> WithTracking<TKey, T2, T3, T4, T5, TResult>(
            this Func<TKey, T2, T3, T4, T5, TResult> func,
            DelegateTracker<TKey> tracking)
        {
            if (tracking == null)
            {
                throw new ArgumentNullException(nameof(tracking));
            }

            func = func ?? ((_, __, ___, ____, ______) => default(TResult));

            Func<TKey, T2, T3, T4, T5, TResult> tracked = (key, arg2, arg3, arg4, arg5) =>
                {
                    var @event = tracking.AddEvent(key);

                    var result = func(key, arg2, arg3, arg4, arg5);

                    @event.EndedAt = DateTime.Now;

                    return result;
                };
            return tracked;
        }
    }
}