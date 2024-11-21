using CommunityToolkit.Maui.Views;
using MauiPopup;
using ProTCE.Controllers;
using ProTCE.Models;

namespace TNAPP.Views;

public partial class AsignarNotasPage : ContentPage
{
    private readonly string codigoClase;
    private string codigoEvaluacion;
    private readonly int calificacionMaxima;
    private readonly EstudianteController estudianteController;
    public AsignarNotasPage(string codigoClase, string codigoEvaluacion, int calificacionMaxima)
    {
        InitializeComponent();
        this.codigoClase = codigoClase;
        this.codigoEvaluacion = codigoEvaluacion;
        this.calificacionMaxima = calificacionMaxima;
        estudianteController = new EstudianteController();
        LoadEstudiantes();
    }
    private async void LoadEstudiantes()
    {
        var estudiantes = await estudianteController.GetEstudiantesPorClase(codigoClase);

        // Asignar la imagen en el momento de la carga
        foreach (var estudiante in estudiantes)
        {
            //// Asignar la imagen directamente en el código
            //if (estudiante.Genero == "M")
            //{
            //    imagenestudiante.Source = "boy.png";*/ // Asegúrate de que esta propiedad exista
            //}
            //else if (estudiante.Genero == "F")
            //{
            //    imagenestudiante.Source= "girl.png"; // Asegúrate de que esta propiedad exista
            //}
        }
        estudiantesListView.ItemsSource = estudiantes;
    }

    private async void OnEstudianteSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Estudiante estudiante)
        {
            // Mostrar opciones para el estudiante seleccionado
            var action = await DisplayActionSheet("Selecciona una opción", "Cancelar", null, "Asignar Nota", "Ver Nota");
            if (action == "Asignar Nota")
            {
                // Crear la instancia de la modal con los parámetros necesarios
                var popup = new IngresarNotaModal(estudiante.Id, codigoEvaluacion, calificacionMaxima, codigoClase);
                PopupAction.DisplayPopup(popup); // Abre la modal usando BasePopupPage
            }
            else if (action == "Ver Nota")
            {
                // Lógica para ver la nota
                var nota = await estudianteController.GetNotaPorEvaluacion(estudiante.Id, codigoClase, codigoEvaluacion);
                if (nota != null)
                {
                    await DisplayAlert("Nota", $"La nota de {estudiante.Nombres} {estudiante.Apellidos} es: {nota.Valor}", "OK");
                }
                else
                {
                    await DisplayAlert("Info", "No hay nota registrada para este estudiante.", "OK");
                }
            }

        // Deseleccionar el ítem después de la acción
        ((ListView)sender).SelectedItem = null;
        }
    }
}