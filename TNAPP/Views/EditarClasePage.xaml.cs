using ProTCE.Controllers;
using ProTCE.Models;
using System.Formats.Tar;

namespace TNAPP.Views;

public partial class EditarClasePage : ContentPage
{
    private readonly ClaseController claseController;
    private Clase clase;
    public EditarClasePage(Clase clase)
	{
		InitializeComponent();
        this.clase = clase;
        claseController = new ClaseController();
        LoadClassData();
    }
    private void LoadClassData()
    {
        // Cargar los datos de la clase en los campos de entrada
        classNameEntry.Text = clase.NombreClase;
        gradoPicker.SelectedItem = clase.Grado; // Asumiendo que el grado se corresponde con los elementos del Picker
        yearEntry.Text = clase.Anio.ToString();
        MateriaPicker.SelectedItem = clase.Materia; // Asumiendo que la materia se corresponde con los elementos del Picker
    }

    private async void OnActualizarClicked(object sender, EventArgs e)
    {
        // Validar campos
        if (string.IsNullOrWhiteSpace(classNameEntry.Text) || string.IsNullOrWhiteSpace(yearEntry.Text))
        {
            await DisplayAlert("Error", "Por favor, completa todos los campos.", "OK");
            return;
        }

        // Actualizar los campos de la clase
        clase.NombreClase = classNameEntry.Text;
        clase.Grado = gradoPicker.SelectedItem.ToString();
        clase.Anio = int.Parse(yearEntry.Text);
        clase.Materia = MateriaPicker.SelectedItem.ToString();

        // Llamar al método de actualización
        bool result = await claseController.UpdateClass(clase);
        if (result)
        {
            await DisplayAlert("Éxito", "Clase actualizada correctamente.", "OK");
            await Navigation.PopAsync(); // Regresar a la página anterior
        }
        else
        {
            await DisplayAlert("Error", "No se pudo actualizar la clase.", "OK");
        }
    }

    private async void OnCancelar(object sender, EventArgs e)
    {
        await Navigation.PopAsync();    
    }
}