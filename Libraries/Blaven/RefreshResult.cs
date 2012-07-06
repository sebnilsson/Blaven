namespace Blaven {
    public enum RefreshResult {
        UpdateFailed = -1,
        CancelledIsRefreshed = 0,
        CancelledIsRefreshing = 1,
        UpdateSync = 2,
        UpdateAsync = 3,
    }
}
