using System.Threading.Tasks;

namespace Blaven
{
    internal static class TaskCompleted
    {
        public static Task Task { get; } = Task.FromResult(0);
    }
}
