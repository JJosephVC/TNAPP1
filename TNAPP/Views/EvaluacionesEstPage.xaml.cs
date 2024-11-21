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

        BindingContext = this;
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
            evaluacionesCollectionView.ItemsSource = evaluaciones; // Asigna la lista de evaluaciones al ListView
        }
        else
        {
            await DisplayAlert("Sin Evaluaciones", "No hay evaluaciones disponibles para esta clase.", "OK");
        }
    }

    private async void OnEvaluacionSelected(object sender, SelectionChangedEventArgs e)
    {
        // Verifica si hay una selecci�n v�lida
        if (e.CurrentSelection.FirstOrDefault() is Evaluacion evaluacion)
        {
            try
            {
                string codigoEvaluacion = await evaluacionesController.ObtenerIdEvaluacionPorNombre(claseId, evaluacion.Nombre);
                var nota = await evaluacionesController.GetNotaPorEvaluacion(estudianteId, claseId, codigoEvaluacion);

                if (nota != null)
                {
                    await DisplayAlert("Nota", $"Valor: {nota.Valor}\nRetroalimentaci�n: {nota.Retroalimentacion}", "OK");
                }
                else
                {
                    await DisplayAlert("Sin Nota", "No se encontr� una nota para esta evaluaci�n.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Ocurri� un error al cargar los datos: " + ex.Message, "OK");
            }
        }

    // Deselecciona el elemento despu�s de procesarlo
    ((CollectionView)sender).SelectedItem = null;
    }
}