using MauiPopup;
using ProTCE.Controllers;

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
    private void OnFloatingButtonTapped(object sender, TappedEventArgs e)
    {
        PopupAction.DisplayPopup(new UnirseModal(userId)); // Abre la modal o ventana emergente
    }
}