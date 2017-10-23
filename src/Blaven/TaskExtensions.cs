using System;
using System.Threading.Tasks;

namespace Blaven
{
    internal static class TaskExtensions
    {
        public static void Await(this Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            task.GetAwaiter().GetResult();
        }

        public static T Await<T>(this Task<T> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var result = task.GetAwaiter().GetResult();
            return result;
        }
    }
}