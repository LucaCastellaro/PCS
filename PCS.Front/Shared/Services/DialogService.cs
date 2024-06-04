namespace PCS.Front.Shared.Services;

internal interface IDialogService
{
    Task Alert(string title, string message, string cancel);
}

internal class DialogService : IDialogService
{
    public Task Alert(string title, string message, string cancel)
    {
        if (Application.Current?.MainPage is null)
            return Task.CompletedTask;
        
        return Application.Current.MainPage.DisplayAlert(title, message, cancel);
    }
}