using ProTCE.Database;
using ProTCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProTCE.Controllers
{
    public class EvaluacionesController
    {
        private readonly FirebaseConnection firebaseConnection;

        public EvaluacionesController()
        {
            firebaseConnection = new FirebaseConnection();
        }

        public async Task<List<Evaluacion>> GetEvaluacionesPorClase(string claseId)
        {
            var claseData = await firebaseConnection.GetClaseByCodigo(claseId);
            var evaluaciones = new List<Evaluacion>();

            if (claseData?.evaluaciones != null)
            {
                foreach (var evaluacionEntry in claseData.evaluaciones)
                {
                    var evaluacion = evaluacionEntry.Value; // Accede a los detalles de la evaluación
                    evaluaciones.Add(new Evaluacion
                    {
                        Nombre = evaluacion.Nombre,
                        Fecha = evaluacion.Fecha,
                        TipoEvaluacion = evaluacion.TipoEvaluacion,
                        CalificacionMaxima = evaluacion.CalificacionMaxima
                        // Puedes mapear otros campos según sea necesario
                    });
                }
            }

            return evaluaciones;
        }

        public async Task<string> ObtenerIdEvaluacionPorNombre(string claseId, string nombreEvaluacion)
        {
            var claseData = await firebaseConnection.GetClaseByCodigo(claseId);

            if (claseData?.evaluaciones != null)
            {
                foreach (var evaluacionEntry in claseData.evaluaciones)
                {
                    var evaluacion = evaluacionEntry.Value; // Accede a los detalles de la evaluación
                    if (evaluacion.Nombre == nombreEvaluacion)
                    {
                        return evaluacionEntry.Key; // Retorna el ID de la evaluación
                    }
                }
            }

            return null; // Retorna null si no se encuentra la evaluación
        }


        public async Task<bool> CrearEvaluacion(string claseId, Evaluacion evaluacion)
        {
            // Generar un nombre único para la evaluación
            evaluacion.Nombre = await GenerarNombreUnicoEvaluacion(claseId, evaluacion.Nombre);
            // Generar el ID de la evaluación
            string idEvaluacion = await GenerarIdEvaluacion(evaluacion.Nombre, evaluacion.CorteEvaluativo, evaluacion.Semestre);

            // Llamar al método de FirebaseConnection para crear la evaluación
          return await firebaseConnection.CrearEvaluacion(claseId, evaluacion, idEvaluacion);
        }

        public async Task<string> GenerarIdEvaluacion(string nombre, string corteEvaluativo, string semestre)
        {
            // Extraer los números de corte y semestre directamente en este método
            string corteNum = ExtraerNumeroDeCorte(corteEvaluativo);
            string semestreNum = ExtraerNumeroDeSemestre(semestre);

            string nombreAvreviado = nombre.Length > 4 ? nombre.Substring(0, 4).ToUpper() : nombre.ToUpper();
            // Crear la base del ID
            string baseId = $"{nombreAvreviado}{corteNum}{semestreNum}";

            // Verificar si el ID existe
            string idEvaluacionFinal = baseId;
            int intentos = 0;
            while (await ExisteIdEvaluacion(idEvaluacionFinal) && intentos < 100)
            {
                var numeroAleatorio = new Random().Next(100, 999);
                idEvaluacionFinal = $"{baseId}-{numeroAleatorio}";
                intentos++;
            }

            return idEvaluacionFinal;
        }

        public async Task<string> GenerarNombreUnicoEvaluacion(string claseId, string nombreEvaluacion)
        {
            var claseData = await firebaseConnection.GetClaseByCodigo(claseId);
            int contador = 1;
            string nuevoNombre = nombreEvaluacion;

            while (claseData?.evaluaciones?.Any(e => e.Value.Nombre == nuevoNombre) == true)
            {
                nuevoNombre = $"{nombreEvaluacion}_{contador++}"; // Modificar el nombre
            }

            return nuevoNombre; // Retorna el nombre único
        }
        private string ExtraerNumeroDeCorte(string corteEvaluativo)
        {
            // Quitar 'Corte Evaluativo' y espacios, y poner en mayúsculas
            corteEvaluativo.Replace(" Corte Evaluativo", "").Trim().ToUpper();
            corteEvaluativo.Replace("er", "").Replace("do", "").Replace("to", "");

            return corteEvaluativo;
        }

        private string ExtraerNumeroDeSemestre(string semestre)
        {
            // Quitar 'Semestre' y espacios, y poner en mayúsculas
            semestre.Replace(" Semestre", "").Trim().ToUpper();
            semestre.Replace("er", "").Replace("do", "");

            return semestre;
        }

        private async Task<bool> ExisteIdEvaluacion(string idEvaluacion)
        {
            // Verificar si la evaluación con este ID ya existe en Firebase
            var evaluacionExistente = await firebaseConnection.GetEvaluacionById(idEvaluacion);
            return evaluacionExistente != null; // Retorna true si la evaluación existe
        }

        public async Task<bool> AsignarNota(string estudianteId, string codigoEvaluacion, Nota nota, string codigoClase)
        {
            if (string.IsNullOrEmpty(estudianteId) || string.IsNullOrEmpty(codigoEvaluacion) || nota == null)
            {
                return false; // Validar los datos de entrada
            }

            // Verificar si ya existe una nota registrada para ese Estudiante
            var notaExistente = await GetNotaPorEvaluacion(estudianteId, codigoClase, codigoEvaluacion);
            if (notaExistente != null)
            {
                // Si ya existe una nota, retornar false para indicar que no se puede guardar una nueva nota    
                return false;
            }

            // Guarda la nota si todo es correcto
            return await firebaseConnection.AsignarNota(codigoClase, codigoEvaluacion, estudianteId, nota);
        }

        public async Task<Nota> GetNotaPorEvaluacion(string estudianteId, string codigoClase, string evaluacionId)
        {
            var nota = await firebaseConnection.GetNotaPorEvaluacion(estudianteId, codigoClase, evaluacionId);
            return nota; // Retorna la nota si existe
        }
    }
}