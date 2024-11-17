using MauiPopup;
using ProTCE.Controllers;

namespace TNAPP.Views;

public partial class EstudiantePage : ContentPage
{
    private readonly EstudianteController estudianteController;
    private readonly string userId;
    public EstudiantePage(string userId)
	{
		InitializeComponent();
        estudianteController = new EstudianteController();
        this.userId = userId;
        LoadEstudianteData();

        // Suscribirse al mensaje para recargar los datos cuando se inscribe una clase
        MessagingCenter.Subscribe<UnirseModal>(this, "ClaseInscrita", (sender) =>
        {
            LoadEstudianteData();
        });
    }

    private async void LoadEstudianteData()
    {
        var clasesInscritas = await estudianteController.GetClasesPorEstudiante(userId);
        if (clasesInscritas != null && clasesInscritas.Count > 0)
        {
            enrolledClassesCollectionView.ItemsSource = clasesInscritas;
            noClasesimg.IsVisible = false;
        }
        else
        {
            noClasesimg.IsVisible = true;
            enrolledClassesCollectionView.ItemsSource = null;
            // await DisplayAlert("Información", "No tienes clases inscritas.", "OK");
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Desuscribirse del mensaje
        MessagingCenter.Unsubscribe<UnirseModal>(this, "ClaseInscrita");
    }

    private void OnNavInicio(object sender, EventArgs e)
    {
        ResetTabStyles();
        SetActiveTab(navInicioIcon, navInicioLabel);

        // Crea una nueva instancia de EstudiantePage y carga su contenido
        var estudiantePage = new EstudiantePage(userId);
        estudiantePage.LoadEstudianteData(); // Llama explícitamente al método de carga de datos
        ContentContainer.Content = estudiantePage.Content; // Asigna el contenido de la página
    }

    private void OnNavPerfil(object sender, EventArgs e)
    {
        ResetTabStyles();
        SetActiveTab(navPerfilIcon, navPerfilLabel);
        ContentContainer.Content = new PerfilEstudiante();
    }

    private void OnNavAjustes(object sender, EventArgs e)
    {
        ResetTabStyles();
        SetActiveTab(navAjustesIcon, navAjustesLabel);
    }

    private void ResetTabStyles()
    {
        // Restaurar todos los íconos y colores predeterminados
        navInicioIcon.Source = "home.png";
        navInicioLabel.TextColor = Colors.Black;

        navPerfilIcon.Source = "user.png";
        navPerfilLabel.TextColor = Colors.Black;

        navAjustesIcon.Source = "setting_s.png";
        navAjustesLabel.TextColor = Colors.Black;
    }

    private void SetActiveTab(Image icon, Label label)
    {
        // Cambiar la imagen del icono activo
        if (icon == navInicioIcon)
            icon.Source = "home_s.png"; // Cambia al ícono seleccionado de inicio
        else if (icon == navPerfilIcon)
            icon.Source = "user_s.png"; // Cambia al ícono seleccionado de perfil
        else if (icon == navAjustesIcon)
            icon.Source = "setting.png"; // Cambia al ícono seleccionado de ajustes

        // Cambiar el color del texto de la etiqueta activa
        label.TextColor = Colors.Blue;
    }

    private void OnFloatingButtonTapped(object sender, TappedEventArgs e)
    {
        PopupAction.DisplayPopup(new UnirseModal(userId)); // Abre la modal o ventana emergente
    }
}