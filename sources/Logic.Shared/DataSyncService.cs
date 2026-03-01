using Data.Accessor.Interfaces;
using Data.Database.Entities.User;
using Data.Database.Entities.Vocabulary;
using Logic.Shared.Interfaces;
using Newtonsoft.Json;
using Shared.Enums;
using Shared.Models.Sync;
using Shared.Models.User;

namespace Logic.Shared
{
    public class DataSyncService : IDataSyncService
    {
        private readonly IHttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DataSyncService> _logger;

        public DataSyncService(IHttpClient httpClient, IUnitOfWork unitOfWork, ILogger<DataSyncService> logger)
        {
            _httpClient = httpClient;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CurrentUser?> SyncUserData()
        {
            try
            {

                var userDataJson = await PullData("syncronization/pulluserdata");

                var userData = ParseResponse<UserDataSyncModel>(userDataJson);
                var userEntity = await _unitOfWork.UserRepository.FirstOrDefaultByIdExternalAsync(userData?.IdExternal ?? Guid.Empty, false);

                var isNewUser = userEntity == null;

                if (isNewUser && userData?.IdExternal != null)
                {
                    userEntity = new UserEntity
                    {
                        IdExternal = userData.IdExternal,
                        FirstName = userData.FirstName,
                        LastName = userData.LastName,
                        EmailAddress = userData.EmailAddress,
                        DateOfBirth = userData.DateOfBirth,
                        ProfileImage = userData.ProfileImage,
                        UserRole = userData.UserRole,
                        IsDirty = userData.IsDirty,
                        UserCredentials = new UserCredentialsEntity
                        {
                            IdExternal = userData.UserCredentials.IdExternal,
                            PasswordHash = userData.UserCredentials.PasswordHash,
                            ExpireDate = userData.UserCredentials.ExpireDate,
                            RefreshToken = userData.UserCredentials.RefreshToken,
                            IsDirty = userData.UserCredentials.IsDirty,
                            CreatedAt = userData.UserCredentials.CreatedAt,
                            CreatedBy = userData.UserCredentials.CreatedBy,
                            UpdatedAt = userData.UserCredentials.UpdatedAt,
                            UpdatedBy = userData.UserCredentials.UpdatedBy,
                        },
                        UserSettings = new UserSettingsEntity
                        {
                            IdExternal = userData.UserSettings.IdExternal,
                            IsAutoDataSyncEnabled = userData.UserSettings.IsAutoDataSyncEnabled,
                            Culture = userData.UserSettings.Culture,
                            UseLocalDataStore = userData.UserSettings.UseLocalDataStore,
                            IsDirty = userData.UserSettings.IsDirty,
                            CreatedAt = userData.UserSettings.CreatedAt,
                            CreatedBy = userData.UserSettings.CreatedBy,
                            UpdatedAt = userData.UserSettings.UpdatedAt,
                            UpdatedBy = userData.UserSettings.UpdatedBy
                        },
                        CreatedAt = userData.CreatedAt,
                        CreatedBy = userData.CreatedBy,
                        UpdatedAt = userData.UpdatedAt,
                        UpdatedBy = userData.UpdatedBy
                    };

                    await _unitOfWork.UserRepository.AddAsync(userEntity);

                    await _unitOfWork.SaveChangesAsync("System");
                }

                if (userEntity == null)
                {
                    throw new Exception("Could not sync user data.");
                }

                return new CurrentUser
                {
                    UserId = userEntity.Id,
                    Email = userEntity.EmailAddress,
                    ProfileImage = userEntity.ProfileImage
                };

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync("Sync user data failed.", LogMessageTypeEnum.Error, exception);

                return null;
            }
        }

        public async Task SyncVocabularyMetaData()
        {
            try
            {
                var languageSyncResult = await SyncVocabularyLanguages();
                var partOfSpeechSyncResult = await SyncPartOfSpeech();
                var vocabularyCategorySyncResult = await SyncVocabularyCategories();

                if (languageSyncResult || partOfSpeechSyncResult || vocabularyCategorySyncResult)
                {
                    await _unitOfWork.SaveChangesAsync("System");
                }

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync("Sync vocabulary meta data failed.", LogMessageTypeEnum.Error, exception);
            }
        }

        private async Task<bool> SyncVocabularyLanguages()
        {
            var vocabulyLanguageResponseContent = await PullData("syncronization/pullvocabularylanguagesyncmodels");
            var languageSyncModels = ParseResponse<List<VocabularyLanguageModel>>(vocabulyLanguageResponseContent);

            if (languageSyncModels != null && languageSyncModels.Any())
            {
                var existingLanguageEntities = await _unitOfWork.VocabularyUnitOfWork.LanguageRepository.GetAllAsync();

                if (!existingLanguageEntities.Any())
                {
                    var newLanguageEntities = languageSyncModels.Select(model => new LanguageEntity
                    {
                        IdExternal = model.IdExternal,
                        Name = model.Name,
                        ResourceKey = model.ResourceKey,
                        TranslationType = model.TranslationType,
                        IsDirty = model.IsDirty,
                        CreatedAt = model.CreatedAt,
                        CreatedBy = model.CreatedBy,
                        UpdatedBy = model.UpdatedBy,
                        UpdatedAt = model.UpdatedAt,
                    }).ToList();

                    await _unitOfWork.VocabularyUnitOfWork.LanguageRepository.AddRangeAsync(newLanguageEntities);

                    return true;
                }
                else
                {
                    var languageDictionary = existingLanguageEntities.ToDictionary(e => e.IdExternal, e => e);

                    foreach (var languageEntity in languageSyncModels)
                    {
                        var entityToUpdate = languageDictionary.GetValueOrDefault(languageEntity.IdExternal);

                        if (entityToUpdate == null)
                        {
                            var entityToAdd = new LanguageEntity
                            {
                                IdExternal = languageEntity.IdExternal,
                                Name = languageEntity.Name,
                                ResourceKey = languageEntity.ResourceKey,
                                TranslationType = languageEntity.TranslationType,
                                IsDirty = languageEntity.IsDirty,
                                CreatedAt = languageEntity.CreatedAt,
                                CreatedBy = languageEntity.CreatedBy,
                                UpdatedBy = languageEntity.UpdatedBy,
                                UpdatedAt = languageEntity.UpdatedAt,
                            };

                            await _unitOfWork.VocabularyUnitOfWork.LanguageRepository.AddAsync(entityToAdd);
                        }
                        else
                        {
                            entityToUpdate.Name = languageEntity.Name;
                            entityToUpdate.ResourceKey = languageEntity.ResourceKey;
                            entityToUpdate.TranslationType = languageEntity.TranslationType;
                            entityToUpdate.IsDirty = languageEntity.IsDirty;
                            entityToUpdate.UpdatedAt = languageEntity.UpdatedAt;
                            entityToUpdate.UpdatedBy = languageEntity.UpdatedBy;
                        }
                    }
                }
            }

            return false;
        }

        private async Task<bool> SyncPartOfSpeech()
        {
            var partOfSpeechResponseContent = await PullData("syncronization/pullpartofspeechsyncmodels");
            var partOfSpeechSyncModels = ParseResponse<List<PartOfSpeechSyncModel>>(partOfSpeechResponseContent);

            if (partOfSpeechSyncModels != null && partOfSpeechSyncModels.Any())
            {
                var existingEntities = await _unitOfWork.VocabularyUnitOfWork.PartOfSpeechRepository.GetAllAsync();

                if (!existingEntities.Any())
                {
                    var newPartOfSpeechEntities = partOfSpeechSyncModels.Select(model => new PartOfSpeechEntity
                    {
                        IdExternal = model.IdExternal,
                        Name = model.Name,
                        ResourceKey = model.ResourceKey,
                        IsDirty = model.IsDirty,
                        CreatedBy = model.CreatedBy,
                        CreatedAt = model.CreatedAt,
                        UpdatedBy = model.UpdatedBy,
                        UpdatedAt = model.UpdatedAt,
                    }).ToList();

                    await _unitOfWork.VocabularyUnitOfWork.PartOfSpeechRepository.AddRangeAsync(newPartOfSpeechEntities);

                    return true;
                }
                else
                {
                    var partOfSpeechDictionary = existingEntities.ToDictionary(e => e.IdExternal, e => e);

                    foreach (var partOfSpeech in partOfSpeechSyncModels)
                    {
                        var entityToUpdate = partOfSpeechDictionary.GetValueOrDefault(partOfSpeech.IdExternal);

                        if (entityToUpdate == null)
                        {
                            var entityToAdd = new PartOfSpeechEntity
                            {
                                IdExternal = partOfSpeech.IdExternal,
                                Name = partOfSpeech.Name,
                                ResourceKey = partOfSpeech.ResourceKey,
                                IsDirty = partOfSpeech.IsDirty,
                                CreatedBy = partOfSpeech.CreatedBy,
                                CreatedAt = partOfSpeech.CreatedAt,
                                UpdatedBy = partOfSpeech.UpdatedBy,
                                UpdatedAt = partOfSpeech.UpdatedAt,
                            };

                            await _unitOfWork.VocabularyUnitOfWork.PartOfSpeechRepository.AddAsync(entityToAdd);
                        }
                        else
                        {
                            entityToUpdate.Name = partOfSpeech.Name;
                            entityToUpdate.ResourceKey = partOfSpeech.ResourceKey;
                            entityToUpdate.IsDirty = partOfSpeech.IsDirty;
                            entityToUpdate.UpdatedAt = partOfSpeech.UpdatedAt;
                            entityToUpdate.UpdatedBy = partOfSpeech.UpdatedBy;
                        }
                    }
                }

                var partOfSpeechEntities = partOfSpeechSyncModels
                    .Where(model => !existingEntities.All(e => e.IdExternal == model.IdExternal))
                    .Select(model => new PartOfSpeechEntity
                    {
                        IdExternal = model.IdExternal,
                        Name = model.Name,
                        ResourceKey = model.ResourceKey,
                        IsDirty = model.IsDirty,
                        CreatedBy = model.CreatedBy,
                        CreatedAt = model.CreatedAt,
                        UpdatedBy = model.UpdatedBy,
                        UpdatedAt = model.UpdatedAt,
                    }).ToList();

                if (partOfSpeechEntities.Any())
                {
                    await _unitOfWork.VocabularyUnitOfWork.PartOfSpeechRepository.AddRangeAsync(partOfSpeechEntities);

                    return true;
                }
            }

            return false;
        }

        private async Task<bool> SyncVocabularyCategories()
        {
            var vocabularyCategoryResponseContent = await PullData("syncronization/pullvocabularycategorysyncmodels");
            var vocabularyCategorySyncModels = ParseResponse<List<VocabularyCategorySyncModel>>(vocabularyCategoryResponseContent);

            if (vocabularyCategorySyncModels != null && vocabularyCategorySyncModels.Any())
            {
                var existingEntities = await _unitOfWork.VocabularyUnitOfWork.VocabularyCategoryRepository.GetAllAsync();

                if (!existingEntities.Any())
                {
                    var newVocabularyCategoryEntities = vocabularyCategorySyncModels.Select(model => new VocabularyCategoryEntity
                    {
                        IdExternal = model.IdExternal,
                        Name = model.Name,
                        SourceLanguage = model.SourceLanguage,
                        IsDirty = model.IsDirty,
                        CreatedBy = model.CreatedBy,
                        CreatedAt = model.CreatedAt,
                        UpdatedBy = model.UpdatedBy,
                        UpdatedAt = model.UpdatedAt,
                    }).ToList();

                    await _unitOfWork.VocabularyUnitOfWork.VocabularyCategoryRepository.AddRangeAsync(newVocabularyCategoryEntities);

                    return true;
                }
                else
                {
                    var vocabularyCategoryDictionary = existingEntities.ToDictionary(e => e.IdExternal, e => e);

                    foreach (var vocabularyCategory in vocabularyCategorySyncModels)
                    {
                        var entityToUpdate = vocabularyCategoryDictionary.GetValueOrDefault(vocabularyCategory.IdExternal);

                        if (entityToUpdate == null)
                        {
                            var entityToAdd = new VocabularyCategoryEntity
                            {
                                IdExternal = vocabularyCategory.IdExternal,
                                Name = vocabularyCategory.Name,
                                SourceLanguage = vocabularyCategory.SourceLanguage,
                                IsDirty = vocabularyCategory.IsDirty,
                                CreatedBy = vocabularyCategory.CreatedBy,
                                CreatedAt = vocabularyCategory.CreatedAt,
                                UpdatedBy = vocabularyCategory.UpdatedBy,
                                UpdatedAt = vocabularyCategory.UpdatedAt,
                            };

                            await _unitOfWork.VocabularyUnitOfWork.VocabularyCategoryRepository.AddAsync(entityToAdd);
                        }
                        else
                        {
                            entityToUpdate.Name = vocabularyCategory.Name;
                            entityToUpdate.SourceLanguage = vocabularyCategory.SourceLanguage;
                            entityToUpdate.IsDirty = vocabularyCategory.IsDirty;
                            entityToUpdate.UpdatedAt = vocabularyCategory.UpdatedAt;
                            entityToUpdate.UpdatedBy = vocabularyCategory.UpdatedBy;
                        }
                    }
                }

            }

            return false;
        }

        private async Task<string> PullData(string url)
        {
            var response = await _httpClient.SendGetRequest(url);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        private T? ParseResponse<T>(string responseContentString)
        {
            if (responseContentString == null)
            {
                return default;
            }

            var responseModel = JsonConvert.DeserializeObject<T>(responseContentString);

            return responseModel;
        }
    }
}
