namespace Identity.Service;

public enum LoginStatus
{
    Succeeded = 1,
    WrongEmail = 2,
    WrongCredentials = 3,
    AccountLocked = 4,
    AccountNotActivated = 5,
    Blocked = 6,
    FirstLoginSucceeded = 7,
    IsDeactivated = 8,
}
