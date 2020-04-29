using System;

namespace Blaven.Storage
{
    public interface IStorageUpdater
    {
        StorageUpdateResult Update(
            DateTimeOffset? updatedSince = null,
            params BlogKey[] blogKeys);
    }
}
