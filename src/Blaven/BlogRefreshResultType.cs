namespace Blaven
{
    public enum BlogRefreshResultType
    {
        UpdateFailed = -1,

        CancelledIsRefreshed = 0,

        CancelledIsRefreshing = 1,

        UpdateSync = 2,

        UpdateAsync = 3,
    }
}