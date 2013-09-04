namespace Blaven.DataSources
{
    public interface IDataSource
    {
        DataSourceRefreshResult Refresh(DataSourceRefreshContext refreshInfo);
    }
}