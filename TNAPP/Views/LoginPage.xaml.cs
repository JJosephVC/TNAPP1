using ProTCE.Controllers;

namespace TNAPP.Views;

public partial class LoginPage : ContentPage
{
    private AuthController authController;
    public LoginPage()
	{
		InitializeComponent();
        authController = new AuthController();
	}
    private void LimpiarEntrys()
    {
        emailEntry.Text = "";
        passwordEntry.Text = "";
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        //Verificar conexi�n a internet
        var current = Connectivity.Current.NetworkAccess;
        if (current != NetworkAccess.Internet)
        {
            await DisplayAlert("Error", "No hay conexi�n a Internet. Por favor, verifica tu conexi�n y vuelve a intentarlo.", "OK"); // <-- Agregar esta l�nea
            return; // Detener el proceso si no hay conexi�n // <-- Agregar esta l�nea
        }

        var email = emailEntry.Text;
        var password = passwordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            errorLabel.Text = "Por favor, ingrese email y contrase�a.";
            errorLabel.IsVisible = true;
            return;
        }

        var userId = await authController.Login(email, password);
        if (!string.IsNullOrEmpty(userId))
        {// Determinar el rol del usuario
            var role = await authController.GetUserRole(userId);
            if (role == "maestro")
            {
                // Navegar a la p�gina del maestro
                //await Navigation.PushAsync(new MaestroPage(userId));
                LimpiarEntrys();
            }
            else if (role == "estudiante")
            {
                // Navegar a la p�gina del estudiante
                await Navigation.PushAsync(new EstudiantePage(userId));
                LimpiarEntrys();
            }
            else
            {
                errorLabel.Text = "Rol de usuario no encontrado.";
                errorLabel.IsVisible = true;
            }
        }
        else
        {
            errorLabel.Text = "Credenciales inv�lidas.";
            errorLabel.IsVisible = true;
        }
    }

    private async void TapCreate(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}