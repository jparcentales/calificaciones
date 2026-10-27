namespace notas.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (width > 0)
            LoginContent.WidthRequest = Math.Min(width, 620);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LoginCard.Opacity = 0;
        LoginCard.TranslationY = 30;
        await Task.WhenAll(
            LoginCard.FadeToAsync(1, 420, Easing.CubicOut),
            LoginCard.TranslateToAsync(0, 0, 420, Easing.CubicOut));
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        await AnimateButtonPressAsync(sender);
        var user = UserEntry.Text?.Trim();
        if (string.Equals(user, "estudiante", StringComparison.OrdinalIgnoreCase) && PasswordEntry.Text == "1234")
        {
            ErrorLabel.IsVisible = false;
            await Navigation.PushAsync(new GradesPage());
            return;
        }

        ErrorLabel.Text = "Usuario o contraseña incorrectos. Prueba con estudiante / 1234.";
        ErrorLabel.IsVisible = true;
        await ErrorLabel.FadeToAsync(1, 180);
    }

    private static async Task AnimateButtonPressAsync(object? sender)
    {
        if (sender is not VisualElement button) return;
        await button.ScaleToAsync(0.95, 75, Easing.CubicOut);
        await button.ScaleToAsync(1, 110, Easing.SpringOut);
    }
}
