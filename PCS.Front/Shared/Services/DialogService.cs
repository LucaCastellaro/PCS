namespace PCS.Front.Shared.Services;

public interface IDialogService
{
    Task<bool> Alert(string title, string message, string accept, string? cancel = null);
}

public class DialogService : IDialogService
{
    public async Task<bool> Alert(string title, string message, string accept, string? cancel = null)
    {
        if (Application.Current?.MainPage is null)
            return false;

        return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
    }
}