using MauiPopup;
using ProTCE.Controllers;
using ProTCE.Models;

namespace TNAPP.Views;

public partial class MaestroPage : TabbedPage
{
    private readonly AuthController authController;
    private readonly string userId;
    public MaestroPage(string userId)
	{
		InitializeComponent();
        this.userId = userId;
        authController = new AuthController();
        LoadMaestroData();
    }
    private async Task LoadMaestroData()
    {
        var maestro = await authController.GetMaestro(userId);
        if (maestro != null)
        {
            var clasesList = new List<Clase>();
            var claseController = new ClaseController(); // Instanciar ClaseController

            // Obtener los c�digos de clase del diccionario
            foreach (var claseEntry in maestro.Clases)
            {
                // claseEntry.Key es el c�digo de clase
                var codigoClase = claseEntry.Key;

                // Obtener la clase detallada usando ClaseController
                var claseDetalles = await claseController.GetClaseByCodigo(codigoClase);

                if (claseDetalles != null && claseDetalles.IdMaestro == userId) // Aseg�rate de que el ID del maestro coincida
                {
                    clasesList.Add(claseDetalles);
                }
            }

            // Asignar la lista de clases al ListView
            classesListView.ItemsSource = clasesList;
        }
        else
        {
            await DisplayAlert("Error", "No se pudo cargar la informaci�n del maestro.", "OK");
        }
    }
    private async void OnClaseSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Clase clase)
        {
           // Navegar a EvaluacionesPage pasando el c�digo de la clase
           await Navigation.PushAsync(new ClasePage(clase.CodigoClase));
        }
           // Deseleccionar el item despu�s de la navegaci�n
           ((ListView)sender).SelectedItem = null;
    }

    private void OnFloatingButtonTapped(object sender, TappedEventArgs e)
    {
        // Abrir la modal CrearClaseModal y pasar el userId y el callback
        var crearClaseModal = new CrearClaseModal(userId, async () =>
        {
            // Este callback se ejecutar� cuando la clase sea creada
            await LoadMaestroData(); // Recargar los datos de clases
        });

        PopupAction.DisplayPopup(crearClaseModal);
    }

    private async void OnEdit(object sender, EventArgs e)
    {
        // Obtener el par�metro asociado al bot�n presionado
        var button = sender as Button;
        if (button?.CommandParameter is Clase clase)
        {
            // Navegar a la p�gina de edici�n pasando los datos de la clase
            await Navigation.PushAsync(new EditarClasePage(clase));
        }
    }

    private void OnMenuClicked(object sender, EventArgs e)
    {
        // Obtener el bot�n que activ� el evento
        var button = sender as ImageButton;

        // Buscar el Frame relacionado (PopupMenu)
        var parent = button.Parent as Grid;
        var popupMenu = parent.FindByName<Frame>("PopupMenu");

        // Mostrar/Ocultar el men�
        if (popupMenu != null)
        {
            popupMenu.IsVisible = !popupMenu.IsVisible;
        }
    }
}