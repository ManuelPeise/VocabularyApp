using Shared.Enums;

namespace Shared.Models.Sync
{
    public class UserSettingsSyncModel : SyncModelBase
    {
        public CultureEnum Culture { get; set; }
        public bool IsAutoDataSyncEnabled { get; set; }
        public bool UseLocalDataStore { get; set; }
    }
}
