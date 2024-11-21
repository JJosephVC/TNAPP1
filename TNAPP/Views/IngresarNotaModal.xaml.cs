using MauiPopup.Views;
using ProTCE.Controllers;
using ProTCE.Models;

namespace TNAPP.Views;

public partial class IngresarNotaModal : BasePopupPage
{
    private readonly string estudianteId;
    private readonly string codigoEvaluacion;
    private readonly int calificacionMaxima;
    private readonly string codigoClase; // Agregado
    private readonly EvaluacionesController evaluacionesController;
    public IngresarNotaModal(string estudianteId, string codigoEvaluacion, int calificacionMaxima, string codigoClase)
	{
		InitializeComponent();
        this.estudianteId = estudianteId;
        this.codigoEvaluacion = codigoEvaluacion;
        this.calificacionMaxima = calificacionMaxima;
        this.codigoClase = codigoClase;
        evaluacionesController = new EvaluacionesController();
    }

    private async void OnGuardarNotaClicked(object sender, EventArgs e)
    {
        if (int.TryParse(notaEntry.Text, out int nota) && !string.IsNullOrEmpty(retroalimentacionEntry.Text))
        {
            // Validar que la nota esté dentro del rango permitido
            if (nota < 0 || nota > calificacionMaxima)
            {
                await DisplayAlert("Error", $"La nota debe estar entre 0 y {calificacionMaxima}.", "OK");
                return;
            }

            var evaluacionNota = new Nota
            {
                Valor = nota,
                Retroalimentacion = retroalimentacionEntry.Text
            };

            // Guardar nota en la base de datos a través del controlador
            var success = await evaluacionesController.AsignarNota(estudianteId, codigoEvaluacion, evaluacionNota, codigoClase);
            if (success)
            {
                await DisplayAlert("Éxito", "Nota guardada exitosamente.", "OK");
                await Navigation.PopModalAsync(); // Regresar a la lista de estudiantes
            }
            else
            {
                // Mostrar el mensaje de error si no se pudo guardar la nota
                await DisplayAlert("Error", "No se pudo guardar la nota. ¡Ya existe una nota registrada para este estudiante en esta evaluación!", "OK");
                await Navigation.PopModalAsync();
            }
        }
        else
        {
            await DisplayAlert("Error", "Por favor, ingresa una nota válida y retroalimentación.", "OK");
        }
    }

    private async void OnCancel(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}