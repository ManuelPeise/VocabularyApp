using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.App.ViewModels;
using Services.Shared.Interfaces;
using Services.Shared.UiModels;
using System.ComponentModel;

namespace Core.App.Views.Private.User.Profile
{
    public partial class UserProfilePageViewModel : PrivateViewModelBase
    {
        private readonly IUserProfileService _profileService;
        private readonly ChangePasswordPopup _popup;

        private UserProfileModel? _originalUserProfileModel;


        [ObservableProperty]
        private UserProfileModel? _userProfileModel = new UserProfileModel();
        [ObservableProperty]
        private string _userName = string.Empty;
        [ObservableProperty]
        private bool _isModified = true;

        public UserProfilePageViewModel(
            ChangePasswordPopup popup,
            ICurrentUserService currentUserService,
            ISecureStorageHandler secureStorageHandler,
            IUserProfileService profileService) : base(currentUserService, secureStorageHandler)
        {
            _profileService = profileService;
            _popup = popup;

            Task.Run(async () => await InitializeAsync());
        }

        [RelayCommand]
        private async Task HandleUpdateProfileImage()
        {
            if (UserProfileModel?.UserId == null)
            {
                return;
            }

            var media = await MediaPicker.Default.CapturePhotoAsync();

            if (media != null)
            {
                using (var stream = await media.OpenReadAsync())
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();

                    UserProfileModel.ProfileImage = imageBytes;
                }
            }
        }

        [RelayCommand]
        private void RevertChanges()
        {
            if (_originalUserProfileModel != null)
            {
                UserProfileModel = GetUserProfileCopy(_originalUserProfileModel);
            }
        }

        [RelayCommand]

        private async Task SaveChanges()
        {
            if (UserProfileModel == null) { return; }

            var updatedProfile = await _profileService.UpdateProfile(UserProfileModel);

            if (updatedProfile == null)
            {
                RevertChanges();
                return;
            }

            _originalUserProfileModel = GetUserProfileCopy(updatedProfile);
            UserProfileModel = GetUserProfileCopy(updatedProfile);
        }

        [RelayCommand]
        private async Task ShowChangePasswordPopup()
        {
            var popupResult = await Shell.Current.CurrentPage.ShowPopupAsync(_popup);
        }

        partial void OnUserProfileModelChanged(UserProfileModel? oldValue, UserProfileModel? newValue)
        {
            if (oldValue != null)
                oldValue.PropertyChanged -= UserProfileModel_PropertyChanged;

            if (newValue != null)
                newValue.PropertyChanged += UserProfileModel_PropertyChanged;

            CheckIfModified();
        }

        private void UserProfileModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            CheckIfModified();
        }

        private async Task InitializeAsync()
        {
            var userProfile = await _profileService.GetProfile(CurrentUserService.UserData.UserId);

            if (userProfile == null) { return; }

            UserProfileModel = userProfile;
            _originalUserProfileModel = GetUserProfileCopy(userProfile);
            UserName = $"{UserProfileModel?.FirstName} {UserProfileModel?.LastName}";
        }

        private UserProfileModel GetUserProfileCopy(UserProfileModel userProfile)
        {
            return new UserProfileModel
            {
                UserId = userProfile.UserId,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                UserName = userProfile.UserName,
                DateOfBirth = userProfile.DateOfBirth,
                ProfileImage = userProfile.ProfileImage,
                UserRole = userProfile.UserRole,
                LastUpdateAt = userProfile.LastUpdateAt,
                LastUpdateBy = userProfile.LastUpdateBy
            };
        }

        private void UpdateUserProfileImage(UserProfileModel existing, byte[] image)
        {
            UserProfileModel = new UserProfileModel
            {
                UserId = existing.UserId,
                FirstName = existing.FirstName,
                LastName = existing.LastName,
                UserName = existing.UserName,
                DateOfBirth = existing.DateOfBirth,
                ProfileImage = image,
                UserRole = existing.UserRole,
                LastUpdateAt = existing.LastUpdateAt,
                LastUpdateBy = existing.LastUpdateBy
            };
        }

        private void CheckIfModified()
        {
            if (_originalUserProfileModel == null || UserProfileModel == null)
            {
                IsModified = false;
                return;
            }

            IsModified =
                _originalUserProfileModel.FirstName != UserProfileModel.FirstName ||
                _originalUserProfileModel.LastName != UserProfileModel.LastName ||
                _originalUserProfileModel.UserName != UserProfileModel.UserName ||
                _originalUserProfileModel.DateOfBirth != UserProfileModel.DateOfBirth ||
                !(_originalUserProfileModel.ProfileImage?.SequenceEqual(UserProfileModel.ProfileImage ?? Array.Empty<byte>()) ?? UserProfileModel.ProfileImage == null);
        }
    }
}
