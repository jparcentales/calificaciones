namespace notas.Views;

public partial class LoginPage : ContentPage
{
    private static readonly string[] Users = ["Carlos", "Ana", "Jose"];
    private static readonly string[] Passwords = ["carlos123", "ana123", "jose123"];

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
        var userIndex = Array.FindIndex(Users,
            allowedUser => string.Equals(allowedUser, user, StringComparison.OrdinalIgnoreCase));

        if (userIndex >= 0 && string.Equals(Passwords[userIndex], PasswordEntry.Text, StringComparison.Ordinal))
        {
            ErrorLabel.IsVisible = false;
            await Navigation.PushAsync(new GradesPage(Users[userIndex]));
            return;
        }

        ErrorLabel.Text = "Usuario o contraseña incorrectos. Verifica tus credenciales asignadas.";
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
