namespace Blaven.DataSources
{
    public interface IBlogDataSource
    {
        DataSourceRefreshResult Refresh(DataSourceRefreshContext refreshInfo);
    }
}