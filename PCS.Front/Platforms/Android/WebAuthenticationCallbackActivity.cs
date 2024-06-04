using Android.App;
using Android.Content;
using Android.Content.PM;

namespace PCS.Front.Platforms.Android;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter([Intent.ActionView],
              Categories = [
                Intent.CategoryDefault,
                Intent.CategoryBrowsable
              ],
              DataScheme = CALLBACK_SCHEME)]
public class WebAuthenticationCallbackActivity : Microsoft.Maui.Authentication.WebAuthenticatorCallbackActivity
{
    private const string CALLBACK_SCHEME = "myapp";
}