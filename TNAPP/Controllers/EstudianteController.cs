using ProTCE.Database;
using ProTCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProTCE.Controllers
{
    public class EstudianteController
    {
        private readonly FirebaseConnection firebaseConnection;

        public EstudianteController()
        {
            firebaseConnection = new FirebaseConnection();
        }
        public async Task<Estudiante> GetEstudiantePorId(string estudianteId)
        {
            var estudianteData = await firebaseConnection.GetEstudiante(estudianteId);

            // Verificar que se obtuvieron los datos del estudiante
            if (estudianteData != null)
            {
                // Crear un objeto Estudiante con la información recuperada
                return new Estudiante
                {
                    Nombres = estudianteData.Nombres,  // Asumiendo que la propiedad 'Nombre' existe en 'estudianteData'
                    Genero = estudianteData.Genero   // Asumiendo que la propiedad 'Genero' existe en 'estudianteData'
                };
            }
            return null;
        }

        public async Task<List<Clase>> GetClasesPorEstudiante(string estudianteId)
        {
            var estudianteData = await firebaseConnection.GetEstudiante(estudianteId);
            var clasesInscritas = new List<Clase>();

            if (estudianteData ?.Clases != null)
            {
                foreach (var claseKey in estudianteData.Clases.Keys)
                {
                    var claseDetalles = await firebaseConnection.GetClaseByCodigo(claseKey);
                    if (claseDetalles != null)
                    {
                        clasesInscritas.Add(claseDetalles);
                    }
                }
            }

            return clasesInscritas;
        }

        public async Task<bool> RegisterStudentInClass(string estudianteId, string codigoClase)
        {
            bool yaInscrito = await IsEstudianteInscrito(estudianteId, codigoClase);
            if (yaInscrito)
            {
                return false;
            }
            return await firebaseConnection.RegisterEstudianteEnClase(estudianteId, codigoClase);
        }

        public async Task<bool> IsEstudianteInscrito(string estudianteId, string codigoClase)
        {
            return await firebaseConnection.IsEstudianteInscrito(estudianteId, codigoClase);
        }

        public async Task<List<Estudiante>> GetEstudiantesPorClase(string codigoClase)
        {
            return await firebaseConnection.GetEstudiantesPorClase(codigoClase);
        }

        public async Task<Nota> GetNotaPorEvaluacion(string estudianteId, string codigoClase, string codigoEvaluacion)
        {
            return await firebaseConnection.GetNotaPorEvaluacion(estudianteId, codigoClase, codigoEvaluacion);
        }
    }
}
