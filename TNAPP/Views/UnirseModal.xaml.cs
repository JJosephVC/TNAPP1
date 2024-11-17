using MauiPopup;
using MauiPopup.Views;
using ProTCE.Controllers;

namespace TNAPP.Views;

public partial class UnirseModal : BasePopupPage
{
    private readonly string userId;
    private readonly EstudianteController estudianteController;
    public UnirseModal(string userId)
	{
		InitializeComponent();
        this.userId = userId;
        estudianteController = new EstudianteController();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        PopupAction.ClosePopup();
    }

    private async void OnJoinClicked(object sender, EventArgs e)
    {
        string? codigoClase = classCodeEntry.Text?.Trim();
        if (string.IsNullOrEmpty(codigoClase))
        {
            await DisplayAlert("Error", "Por favor, ingresa un código de clase válido.", "OK");
            return;
        }

        bool success = await estudianteController.RegisterStudentInClass(userId, codigoClase);
        if (success)
        {
            await DisplayAlert("Éxito", "Inscripción exitosa.", "OK");
            MessagingCenter.Send(this, "ClaseInscrita"); // Enviar mensaje para actualizar lista
            await Navigation.PopModalAsync();
        }
        else
        {
            // Si no fue exitoso, verifica si es porque ya está inscrito
            bool yaInscrito = await estudianteController.IsEstudianteInscrito(userId, codigoClase);
            if (yaInscrito)
            {
                await DisplayAlert("Error", "Ya estás inscrito en esta clase.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "No se pudo inscribir en la clase. Verifica el código.", "OK");
            }
            //await DisplayAlert("Error", "No se pudo inscribir en la clase. Verifica el código.", "OK");
        }
    }
}