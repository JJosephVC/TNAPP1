using MauiPopup.Views;

namespace TNAPP.Views;

public partial class MensajeConf : BasePopupPage
{
    public TaskCompletionSource<bool> TaskCompletionSource { get; set; }
    public MensajeConf()
	{
		InitializeComponent();
        TaskCompletionSource = new TaskCompletionSource<bool>();
        CBEntendido.CheckedChanged += OnCheckboxCheckedChanged;
    }
    private void OnCheckboxCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        // Habilita el botón al seleccionar el CheckBox
        labelContinuar.IsEnabled = e.Value;
        // Cambiar el color del Label basado en el estado del CheckBox
        labelContinuar.TextColor = e.Value ? Colors.Blue : Colors.Gray;
    }

    private async void TapAcepted(object sender, TappedEventArgs e)
    {
        TaskCompletionSource.SetResult(true);
        await Navigation.PopModalAsync();
    }

    private async void TapCancel(object sender, TappedEventArgs e)
    {
        // Cancela y cierra el popup
        TaskCompletionSource.SetResult(false);
        await Navigation.PopModalAsync();
    }
}