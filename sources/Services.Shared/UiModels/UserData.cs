using CommunityToolkit.Mvvm.ComponentModel;

namespace Services.Shared.UiModels
{
    public partial class UserData: ObservableObject
    {
        [ObservableProperty]
        private int _userId;
        [ObservableProperty]
        private string _email;
        [ObservableProperty]
        private byte[] _profileImage;
    }
}
