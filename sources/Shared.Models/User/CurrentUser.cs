using CommunityToolkit.Mvvm.ComponentModel;

namespace Shared.Models.User
{
    public partial class CurrentUser : ObservableObject
    {
        [ObservableProperty]
        private int _userId;
        [ObservableProperty]
        private string _email;
        [ObservableProperty]
        private byte[] _profileImage;
    }
}
