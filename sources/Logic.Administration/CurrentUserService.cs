using Logic.Administration.Interfaces;
using Shared.Enums;
using Shared.Models.Authentication;
using System.ComponentModel;

namespace Logic.Administration
{
    public class CurrentUserService : ICurrentUserService, INotifyPropertyChanged
    { 
        private AuthenticationResult _authenticationResult;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public AuthenticationResult AuthenticationResult
        {
            get => _authenticationResult;
            private set
            {
                if (_authenticationResult != value)
                {
                    _authenticationResult = value;
                    OnPropertyChanged(nameof(AuthenticationResult));
                }
            }
        }

        public CurrentUserService()
        {
            _authenticationResult = new AuthenticationResult
            {
                IsAuthenticated = false
            };
        }


        public void SetCurrentUser(AuthenticationResult authenticationResult)
        {
            AuthenticationResult = authenticationResult;
        }
    }
}
