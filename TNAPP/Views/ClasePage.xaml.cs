using ProTCE.Controllers;
using ProTCE.Models;

namespace TNAPP.Views;

public partial class ClasePage : TabbedPage
{
    private readonly EvaluacionesController evaluacionesController;
    string claseId;
    private Clase clase;

    public ClasePage(string claseId)
	{
		InitializeComponent();
        this.claseId = claseId;
        evaluacionesController = new EvaluacionesController();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadEvaluaciones();
    }
    private async Task LoadEvaluaciones()
    {
        var evaluaciones = await evaluacionesController.GetEvaluacionesPorClase(claseId);
        if (evaluaciones != null && evaluaciones.Count > 0)
        {
            evaluacionesListView.ItemsSource = evaluaciones; // Asigna la lista de evaluaciones al ListView
        }
        else
        {
            await DisplayAlert("Sin Evaluaciones", "No hay evaluaciones disponibles para esta clase.", "OK");
        }
    }

    private async void OnEvaluacionSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Evaluacion evaluacion)
        {
            string codigoClase = claseId; // Usar el claseId que ya tienes
            string codigoEvaluacion = await evaluacionesController.ObtenerIdEvaluacionPorNombre(codigoClase, evaluacion.Nombre);

            if (codigoEvaluacion != null)
            {
                // Obtener la calificaci�n m�xima de la evaluaci�n seleccionada
                int calificacionMaxima = evaluacion.CalificacionMaxima;

                // Navegar a AsignarNotasPage, pasando todos los par�metros necesarios
                await Navigation.PushAsync(new AsignarNotasPage(codigoClase, codigoEvaluacion, calificacionMaxima));//Est� mal, jejeje
            }
            else
            {
                await DisplayAlert("Error", "No se pudo encontrar la evaluaci�n.", "OK");
            }
        }
            // Deseleccionar el item despu�s de la navegaci�n
            ((ListView)sender).SelectedItem = null;
    }

    private void OnAddEvaluacionClicked(object sender, EventArgs e)
    {

    }
}