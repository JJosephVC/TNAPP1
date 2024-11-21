using MauiPopup;
using ProTCE.Controllers;
using ProTCE.Models;

namespace TNAPP.Views;

public partial class EstudiantePage : TabbedPage
{
    private readonly EstudianteController estudianteController;
    private readonly string userId;
    public EstudiantePage(string userId)
	{
		InitializeComponent();
        estudianteController = new EstudianteController();
        this.userId = userId;
        LoadEstudianteData();
        LoadEstudiante();

        // Suscribirse al mensaje para recargar los datos cuando se inscribe una clase
        MessagingCenter.Subscribe<UnirseModal>(this, "ClaseInscrita", (sender) =>
        {
            LoadEstudianteData();
        });
    }

    private async void LoadEstudiante()
    {
        // Obtener los datos del estudiante (nombre y género, por ejemplo)
        var estudiante = await estudianteController.GetEstudiantePorId(userId);

        if (estudiante != null)
        {
            // Asignar los valores de los datos del estudiante a los controles
            nombreLabel.Text = estudiante.Nombres;  // Asume que el estudiante tiene una propiedad Nombre
            generoLabel.Text = estudiante.Genero;  // Asume que el estudiante tiene una propiedad Genero
        }
        else
        {
            // Si no se encuentra el estudiante, puedes manejarlo, por ejemplo, mostrando un mensaje
            nombreLabel.Text = "Nombre no disponible";
            generoLabel.Text = "Género no disponible";
        }
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
    private void OnFloatingButtonTapped(object sender, TappedEventArgs e)
    {
        PopupAction.DisplayPopup(new UnirseModal(userId)); // Abre la modal o ventana emergente
    }

    private async void OnClaseSelected(object sender, SelectionChangedEventArgs e)
    {
        // Verifica si hay un elemento seleccionado
        if (e.CurrentSelection.FirstOrDefault() is Clase clase)
        {
            // Navegar a la página de evaluaciones, pasando los parámetros necesarios
            await Navigation.PushAsync(new EvaluacionesEstPage(clase.CodigoClase, userId));

            // Limpia la selección para permitir futuras selecciones
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async void OnImg(object sender, TappedEventArgs e)
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Selecciona una imagen"
            });

            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                fotocambio.Source = ImageSource.FromStream(() => stream);
            }
        }
        catch (Exception ex)
        {
            // Manejar excepciones (por ejemplo, si el usuario cancela la selección)
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo seleccionar la imagen: {ex.Message}", "OK");
        }

    }
}