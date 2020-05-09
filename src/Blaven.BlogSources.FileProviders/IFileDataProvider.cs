using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven.BlogSources.FileProviders
{
    public interface IFileDataProvider
    {
        Task<FileDataResult> GetFileData();
    }
}
