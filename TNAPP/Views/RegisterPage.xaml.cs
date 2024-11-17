using Microsoft.Maui.Storage;
using ProTCE.Controllers;

namespace TNAPP.Views;

public partial class RegisterPage : ContentPage
{
	private AuthController authController;
	public RegisterPage()
	{
		InitializeComponent();
        rolePicker.SelectedItem = null; // Rol no seleccionado al inicio
        authController = new AuthController();
	}
    private async void OnRoleSelected(object sender, EventArgs e)
    {
        // Verifica si el usuario seleccionó "Maestro"
        if (rolePicker.SelectedItem?.ToString() == "maestro")
        {
            var mensajePopup = new MensajeConf();
            await this.Navigation.PushModalAsync(mensajePopup);

            // Espera el resultado del popup
            var resultado = await mensajePopup.TaskCompletionSource.Task;

            if (!resultado)
            {
                // Si el usuario canceló, restablece el picker a "null" o "Estudiante" como prefieras
                rolePicker.SelectedItem = null; // O usa: rolePicker.SelectedItem = "Estudiante";
            }
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        // Verificar conexión a Internet
        var current = Connectivity.Current.NetworkAccess;
        if (current != NetworkAccess.Internet)
        {
            await DisplayAlert("Error", "No hay conexión a Internet. Por favor, verifica tu conexión y vuelve a intentarlo.", "OK"); // <-- Agregar esta línea
            return; // Detener el proceso si no hay conexión // <-- Agregar esta línea
        }

        var email = emailEntry.Text;
        var password = passwordEntry.Text;
        var confirmPassword = confirmPasswordEntry.Text;
        var nombres = nombresEntry.Text;
        var apellidos = apellidosEntry.Text;
        var role = rolePicker.SelectedItem?.ToString();
        var genero = GenderPicker.SelectedItem?.ToString(); // Solo si se selecciona estudiante

        // Validar campos vacíos
        if (string.IsNullOrWhiteSpace(email))
        {
            errorLabel.Text = "Por favor, ingrese un correo electrónico.";
            errorLabel.IsVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            errorLabel.Text = "Por favor, ingrese una contraseña.";
            errorLabel.IsVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(nombres))
        {
            errorLabel.Text = "Por favor, ingrese sus nombres.";
            errorLabel.IsVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(apellidos))
        {
            errorLabel.Text = "Por favor, ingrese sus apellidos.";
            errorLabel.IsVisible = true;
            return;
        }

        // Validar coincidencia de contraseñas
        if (password != confirmPassword)
        {
            errorLabel.Text = "Las contraseñas no coinciden.";
            errorLabel.IsVisible = true;
            return;
        }

        // Validar formato de correo electrónico
        if (!IsValidEmail(email))
        {
            errorLabel.Text = "Por favor, ingrese un correo electrónico válido.";
            errorLabel.IsVisible = true;
            return;
        }
        try
        {
            // Registrar usuario
            var userId = await authController.Register(email, password, role, nombres, apellidos, genero);

            if (!string.IsNullOrEmpty(userId))
            {
                await DisplayAlert("Éxito", "Cuenta creada exitosamente.", "OK");
                await Navigation.PopAsync(); // Regresar a la página de login
            }
            else
            {
                errorLabel.Text = "Error al crear la cuenta.";
                errorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            // Verificar si el mensaje de excepción indica que el correo ya existe
            if (ex.Message.Contains("EMAIL_EXISTS"))
            {
                await DisplayAlert("Error", "El correo Electrónico/Email ya existe.", "OK");
            }
            else
            {
                // Mostrar el mensaje de error original en caso de otros tipos de excepciones
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
    // Método para validar formato de correo electrónico
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    protected override bool OnBackButtonPressed()
    {
        // Modifica el botón de retroceso y regresa a la página anterior
        Navigation.PopAsync();

        // Retorna true para indicar que hemos manejado el evento y evitar que se salga de la aplicación
        return true;
    }

    private async void TapAlready(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
}