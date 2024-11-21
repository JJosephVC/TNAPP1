using MauiPopup;
using MauiPopup.Views;
using ProTCE.Controllers;
using ProTCE.Models;
using System.Formats.Tar;

namespace TNAPP.Views;

public partial class CrearClaseModal : BasePopupPage
{
    private readonly ClaseController claseController;
    private string userId;//Id del Maestro
    private Action OnClaseCreated;//Call Back para notificar que la clase fue creada

    private readonly string[] imagenes = { "image1.jpg", "image2.jpg", "image3.jpg", "image4.jpg", "image5.jpg" };

    public CrearClaseModal(string userId, Action onClaseCreated)
	{
		InitializeComponent();
        claseController = new ClaseController();
        this.userId = userId;
        this.OnClaseCreated = onClaseCreated;
    }

    private void OnCancel(object sender, EventArgs e)
    {
        PopupAction.ClosePopup();
    }

    private async void OnSaveClassClicked(object sender, EventArgs e)
    {
        // Asegúrate de que se ingresen todos los campos necesarios
        if (string.IsNullOrWhiteSpace(classNameEntry.Text) || string.IsNullOrWhiteSpace(yearEntry.Text))
        {
            await DisplayAlert("Error", "Por favor, completa todos los campos.", "OK");
            return;
        }

        // Obtener el grado seleccionado y materia seleccionada
        var gradoSeleccionado = gradePicker.SelectedItem.ToString();
        var materiaSeleccionada = MateriaPicker.SelectedItem.ToString();
        // Generar el código de clase
        var codigoClase = await claseController.GenerarCodigoClase(classNameEntry.Text, gradoSeleccionado, int.Parse(yearEntry.Text));

        // Seleccionar una imagen aleatoria
        var random = new Random();
        var imagenAleatoria = imagenes[random.Next(imagenes.Length)];

        var nuevaClase = new Clase
        {
            NombreClase = classNameEntry.Text,
            CodigoClase = codigoClase, // Usar el código generado
            Grado = gradoSeleccionado,
            Materia = materiaSeleccionada,
            IdMaestro = userId,
            Anio = int.Parse(yearEntry.Text),
            BackgroundImage = imagenAleatoria // Asignar la imagen aleatoria
        };

        // Guardar la clase
        bool result = await claseController.CreateClass(nuevaClase);
        if (result)
        {
            await DisplayAlert("Éxito", "Clase creada correctamente.", "OK");
            OnClaseCreated?.Invoke(); // Llamar al callback aquí
            PopupAction.ClosePopup(); // Regresar a la página anterior
        }
        else
        {
            await DisplayAlert("Error", "No se pudo crear la clase.", "OK");
        }
    }
}