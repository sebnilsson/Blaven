using System;

namespace Blaven.Tests
{
    public static class ActionExtensions
    {
        public static Action<TKey> WithTracking<TKey>(this Action<TKey> action, DelegateTracker<TKey> tracking)
        {
            if (tracking == null)
            {
                throw new ArgumentNullException(nameof(tracking));
            }

            action = action ?? (key => { });

            Action<TKey> tracked = key =>
                {
                    var @event = tracking.AddEvent(key);

                    action(key);

                    @event.EndedAt = DateTime.Now;
                };

            return tracked;
        }

        public static Action<TKey, T2> WithTracking<TKey, T2>(
            this Action<TKey, T2> action,
            DelegateTracker<TKey> tracking)
        {
            if (tracking == null)
            {
                throw new ArgumentNullException(nameof(tracking));
            }

            action = action ?? ((_, __) => { });

            Action<TKey, T2> tracked = (key, arg2) =>
                {
                    var @event = tracking.AddEvent(key);

                    action(key, arg2);

                    @event.EndedAt = DateTime.Now;
                };
            return tracked;
        }

        public static Action<TKey, T2, T3> WithTracking<TKey, T2, T3>(
            this Action<TKey, T2, T3> action,
            DelegateTracker<TKey> tracking)
        {
            if (tracking == null)
            {
                throw new ArgumentNullException(nameof(tracking));
            }

            action = action ?? ((_, __, ___) => { });

            Action<TKey, T2, T3> tracked = (key, arg2, arg3) =>
                {
                    var @event = tracking.AddEvent(key);

                    action(key, arg2, arg3);

                    @event.EndedAt = DateTime.Now;
                };
            return tracked;
        }

        public static Action<TKey, T2, T3, T4> WithTracking<TKey, T2, T3, T4>(
            this Action<TKey, T2, T3, T4> action,
            DelegateTracker<TKey> tracking)
        {
            if (tracking == null)
            {
                throw new ArgumentNullException(nameof(tracking));
            }

            action = action ?? ((_, __, ___, ____) => { });

            Action<TKey, T2, T3, T4> tracked = (key, arg2, arg3, arg4) =>
                {
                    var @event = tracking.AddEvent(key);

                    action(key, arg2, arg3, arg4);

                    @event.EndedAt = DateTime.Now;
                };
            return tracked;
        }

        public static Action<TKey, T2, T3, T4, T5> WithTracking<TKey, T2, T3, T4, T5>(
            this Action<TKey, T2, T3, T4, T5> action,
            DelegateTracker<TKey> tracking)
        {
            if (tracking == null)
            {
                throw new ArgumentNullException(nameof(tracking));
            }

            action = action ?? ((_, __, ___, ____, _____) => { });

            Action<TKey, T2, T3, T4, T5> tracked = (key, arg2, arg3, arg4, arg5) =>
                {
                    var @event = tracking.AddEvent(key);

                    action(key, arg2, arg3, arg4, arg5);

                    @event.EndedAt = DateTime.Now;
                };
            return tracked;
        }
    }
}