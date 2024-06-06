namespace PCS.Front.Shared.Services;

public interface IDialogService
{
    Task Alert(string title, string message, string cancel);
}

public class DialogService : IDialogService
{
    public Task Alert(string title, string message, string cancel)
    {
        if (Application.Current?.MainPage is null)
            return Task.CompletedTask;

        return Application.Current.MainPage.DisplayAlert(title, message, cancel);
    }
}