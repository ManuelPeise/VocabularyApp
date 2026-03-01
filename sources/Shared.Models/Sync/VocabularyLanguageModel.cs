using Shared.Enums;

namespace Shared.Models.Sync
{
    public class VocabularyLanguageModel: SyncModelBase
    {
        public string Name { get; set; } = string.Empty;
        public string ResourceKey { get; set; } = string.Empty;
        public TranslationEnum TranslationType { get; set; }
    }
}
