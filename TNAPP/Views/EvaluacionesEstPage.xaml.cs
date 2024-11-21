using ProTCE.Controllers;
using ProTCE.Models;

namespace TNAPP.Views;

public partial class EvaluacionesEstPage : ContentPage
{
    private readonly EvaluacionesController evaluacionesController;
    private readonly string claseId;
    private readonly string estudianteId;
    public EvaluacionesEstPage(string claseId, string estudianteId)
	{
		InitializeComponent();
        this.claseId = claseId;
        this.estudianteId = estudianteId;
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
            // Obtener el ID de la evaluación
            string codigoEvaluacion = await evaluacionesController.ObtenerIdEvaluacionPorNombre(claseId, evaluacion.Nombre);

            // Cargar la nota del estudiante
            var nota = await evaluacionesController.GetNotaPorEvaluacion(estudianteId, claseId, codigoEvaluacion);

            if (nota != null)
            {
                // Mostrar la nota en un DisplayAlert
                await DisplayAlert("Nota", $"Valor: {nota.Valor}\nRetroalimentación: {nota.Retroalimentacion}", "OK");
            }
            else
            {
                await DisplayAlert("Sin Nota", "No se encontró una nota para esta evaluación.", "OK");
            }
        }

    // Deseleccionar el item después de la navegación
    ((ListView)sender).SelectedItem = null;
    }
}