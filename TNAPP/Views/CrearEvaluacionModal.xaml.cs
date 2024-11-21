using MauiPopup.Views;
using ProTCE.Controllers;
using ProTCE.Models;

namespace TNAPP.Views;

public partial class CrearEvaluacionModal : BasePopupPage
{
    private readonly EvaluacionesController evaluacionesController;
    private readonly string claseId;
    public CrearEvaluacionModal(string claseId)
	{
		InitializeComponent();
        this.claseId = claseId;
        evaluacionesController = new EvaluacionesController();
    }

    private async void OnSaveEvaluacionClicked(object sender, EventArgs e)
    {
        // Validaciones
        if (string.IsNullOrWhiteSpace(nombreEntry.Text) ||
            string.IsNullOrWhiteSpace(tipoEntry.Text) ||
            !int.TryParse(calificacionMaximaEntry.Text, out int calificacionMaxima) || calificacionMaxima <= 0)
        {
            await DisplayAlert("Error", "Por favor, complete todos los campos correctamente.", "OK");
            return;
        }

        // Obtener valores de los Pickers
        string corteEvaluativoSeleccionado = corteEvaluativoPicker.SelectedItem.ToString();
        string semestreSeleccionado = semestrePicker.SelectedItem.ToString();

        // Crear la nueva evaluación
        var nuevaEvaluacion = new Evaluacion
        {
            Nombre = nombreEntry.Text,
            TipoEvaluacion = tipoEntry.Text,
            Fecha = fechaPicker.Date,
            CalificacionMaxima = calificacionMaxima,
            CorteEvaluativo = corteEvaluativoSeleccionado, // Almacena el valor completo
            Semestre = semestreSeleccionado // Almacena el valor completo
        };

        // Guardar la evaluación con el ID generado
        var exito = await evaluacionesController.CrearEvaluacion(claseId, nuevaEvaluacion);
        if (exito)
        {
            await DisplayAlert("Éxito", "Evaluación creada correctamente.", "OK");
            await Navigation.PopModalAsync(); // Regresar a la página anterior
        }
        else
        {
            await DisplayAlert("Error", "No se pudo crear la evaluación.", "OK");
        }
    }
}