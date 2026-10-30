using System.Globalization;

namespace notas.Views;

public partial class GradesPage : ContentPage
{
    private readonly CultureInfo _culture = CultureInfo.GetCultureInfo("es-EC");
    private readonly string _userName;
    private bool _welcomeShown;

    public GradesPage(string userName)
    {
        InitializeComponent();
        _userName = userName;
        ConnectedUserLabel.Text = $"Sesión iniciada como {userName}";
        StudentPicker.ItemsSource = new[]
        {
            "Ana López", "Diego Martínez", "Camila Torres", "Mateo Sánchez", "Valentina Ruiz"
        };
        StudentPicker.SelectedIndex = 0;
        GradeDatePicker.Date = DateTime.Today;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (width > 0)
            GradesContent.WidthRequest = Math.Min(width, 1240);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.WhenAll(
            AnimateCardAsync(PeriodCard, 0),
            AnimateCardAsync(StudentCard, 55),
            AnimateCardAsync(Partial1Card, 105),
            AnimateCardAsync(Partial2Card, 155),
            AnimateCardAsync(FinalCard, 205));

        if (!_welcomeShown)
        {
            _welcomeShown = true;
            await DisplayAlertAsync("Bienvenido", $"¡Bienvenido, {_userName}! Ya puedes registrar las calificaciones.", "Continuar");
        }
    }

    private void OnInputChanged(object? sender, EventArgs e)
    {
        var p1 = ReadScore(Follow1Entry.Text) * 0.30 + ReadScore(Exam1Entry.Text) * 0.20;
        var p2 = ReadScore(Follow2Entry.Text) * 0.30 + ReadScore(Exam2Entry.Text) * 0.20;
        Partial1Label.Text = $"{p1.ToString("0.00", _culture)} / 5,00";
        Partial2Label.Text = $"{p2.ToString("0.00", _culture)} / 5,00";

        var complete = HasValidScore(Follow1Entry.Text) && HasValidScore(Exam1Entry.Text)
            && HasValidScore(Follow2Entry.Text) && HasValidScore(Exam2Entry.Text);
        if (!complete)
        {
            FinalScoreLabel.Text = "—";
            StatusLabel.Text = "Pendiente";
            StatusLabel.TextColor = Color.FromArgb("#DDD5F6");
            return;
        }

        var total = p1 + p2;
        FinalScoreLabel.Text = total.ToString("0.00", _culture);
        StatusLabel.Text = GetStatus(total);
        StatusLabel.TextColor = total >= 7 ? Color.FromArgb("#B8F1D7")
            : total >= 5 ? Color.FromArgb("#FFE3A3") : Color.FromArgb("#FFB7C4");
    }

    private async void OnCalculateClicked(object? sender, EventArgs e)
    {
        await AnimateButtonPressAsync(sender);

        if (StudentPicker.SelectedIndex < 0)
        {
            await DisplayAlertAsync("Falta el estudiante", "Selecciona un estudiante para continuar.", "Entendido");
            return;
        }

        if (GradeDatePicker.Date is null)
        {
            await DisplayAlertAsync("Falta la fecha", "Selecciona la fecha del registro para continuar.", "Entendido");
            return;
        }

        var entries = new[] { Follow1Entry, Exam1Entry, Follow2Entry, Exam2Entry };
        var names = new[] { "Seguimiento 1", "Examen 1", "Seguimiento 2", "Examen 2" };
        for (var i = 0; i < entries.Length; i++)
        {
            if (!HasValidScore(entries[i].Text))
            {
                await DisplayAlertAsync("Revisa las calificaciones",
                    $"{names[i]} debe ser un número entre 0 y 10. Puedes usar punto o coma decimal.", "Corregir");
                entries[i].Focus();
                return;
            }
        }

        OnInputChanged(this, EventArgs.Empty);
        var p1 = ReadScore(Follow1Entry.Text) * 0.30 + ReadScore(Exam1Entry.Text) * 0.20;
        var p2 = ReadScore(Follow2Entry.Text) * 0.30 + ReadScore(Exam2Entry.Text) * 0.20;
        var total = p1 + p2;
        await FinalScoreLabel.ScaleToAsync(1.08, 120, Easing.CubicOut);
        await FinalScoreLabel.ScaleToAsync(1, 160, Easing.SpringOut);

        var message = $"Nombre: {StudentPicker.SelectedItem}\n" +
                      $"Fecha: {GradeDatePicker.Date?.ToString("dd/MM/yyyy", _culture) ?? "Sin fecha"}\n" +
                      $"Nota Parcial 1: {p1.ToString("0.00", _culture)} / 5,00\n" +
                      $"Nota Parcial 2: {p2.ToString("0.00", _culture)} / 5,00\n" +
                      $"Nota Final: {total.ToString("0.00", _culture)} / 10\n" +
                      $"Estado: {GetStatus(total)}";
        await DisplayAlertAsync("Resultado académico", message, "Listo");
    }

    private static double ReadScore(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;
        var normalized = text.Trim().Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value)
            ? value : 0;
    }

    private static bool HasValidScore(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        var normalized = text.Trim().Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value)
            && value is >= 0 and <= 10;
    }

    private static string GetStatus(double total) => total >= 7 ? "Aprobado"
        : total >= 5 ? "Complementario" : "Reprobado";

    private static async Task AnimateButtonPressAsync(object? sender)
    {
        if (sender is not VisualElement button) return;
        await button.ScaleToAsync(0.95, 75, Easing.CubicOut);
        await button.ScaleToAsync(1, 110, Easing.SpringOut);
    }

    private static async Task AnimateCardAsync(VisualElement card, int delayMilliseconds)
    {
        card.Opacity = 0;
        card.TranslationY = 22;
        if (delayMilliseconds > 0)
            await Task.Delay(delayMilliseconds);

        await Task.WhenAll(
            card.FadeToAsync(1, 340, Easing.CubicOut),
            card.TranslateToAsync(0, 0, 340, Easing.CubicOut));
    }
}
