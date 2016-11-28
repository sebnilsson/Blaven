using System.Threading.Tasks;

namespace Blaven
{
    internal static class TaskHelper
    {
        public static Task CompletedTask { get; } = Task.FromResult(false);
    }
}