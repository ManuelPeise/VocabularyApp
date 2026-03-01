using Shared.Models.User;

namespace Logic.Shared.Interfaces
{
    public interface IDataSyncService
    {
        Task<CurrentUser?> SyncUserData();
        Task SyncVocabularyMetaData();
    }
}
