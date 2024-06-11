namespace PCS.Front.Shared.Services;

public interface IDialogService
{
    Task Alert(string title, string message, string? cancel = null);
}

public class DialogService : IDialogService
{
    public Task Alert(string title, string message, string? cancel = null)
    {
        if (Application.Current?.MainPage is null)
            return Task.CompletedTask;

        return Application.Current.MainPage.DisplayAlert(title, message, cancel);
    }
}