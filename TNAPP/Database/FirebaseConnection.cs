using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using ProTCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProTCE.Database
{
    public class FirebaseConnection
    {
        //private readonly FirebaseClient firebaseClient;

        //public FirebaseConnection()
        //{
        //    firebaseClient = new FirebaseClient("miConexion");
        //}

        //public FirebaseClient GetFirebaseClient()
        //{
        //    return firebaseClient;
        //}

        private readonly FirebaseClient firebaseClient;
        private readonly FirebaseAuthProvider authProvider;

        public FirebaseConnection()
        {
            // Inicializa la conexión al Firebase
            firebaseClient = new FirebaseClient("https://trackingofclassevaluations-default-rtdb.firebaseio.com/");
            authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyDM4oxKMx9K1Veef-SXCG1dMtIDQSzUmSQ"));
        }

        public FirebaseClient GetFirebaseClient()
        {
            return firebaseClient;
        }
        //Metodos para autenticacion con Firebase Authentication

        public async Task<string> SignIn(string email, string password)
        {
            try
            {
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);
                return auth?.User?.LocalId; // Devuelve el ID del usuario
            }
            catch (FirebaseAuthException ex)
            {
                // Maneja el error de autenticación
                Console.WriteLine($"Error de autenticación: {ex.Message}");
                return null; // Retorna null si hay un error
            }
        }
        public async Task<string> CreateUser(string email, string password)
        {
            var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);
            return auth?.User?.LocalId; // Devuelve el ID del usuario
        }

        // Fin Metodos para autenticacion con Firebase Authentication

        //Metodos para crear Maestros y Estudiantes
        public async Task CreateMaestro(Maestro maestro)
        {
            await firebaseClient.Child("maestros").Child(maestro.Id).PutAsync(maestro);
        }

        public async Task CreateEstudiante(Estudiante estudiante)
        {
            await firebaseClient.Child("estudiantes").Child(estudiante.Id).PutAsync(estudiante);
        }
        //Fin Metodos para crear Maestros y Estudiantes


        // Método para obtener un Maestro por ID
        public async Task<Maestro> GetMaestro(string userId)
        {
            return await firebaseClient.Child("maestros").Child(userId).OnceSingleAsync<Maestro>();
        }

        // Método para obtener un Estudiante por ID
        public async Task<Estudiante> GetEstudiante(string userId)
        {
            return await firebaseClient.Child("estudiantes").Child(userId).OnceSingleAsync<Estudiante>();
        }

        //Metodo para crear una clase
        public async Task CreateClase(Clase clase)
        {
            await firebaseClient.Child("clases").Child(clase.CodigoClase).PutAsync(clase);

            // Agregar la clase al maestro correspondiente
            await firebaseClient.Child("maestros").Child(clase.IdMaestro).Child("clases")
                .Child(clase.CodigoClase).PutAsync(true);
        }

        public async Task<bool> UpdateClase(Clase clase)
        {
            try
            {
                // Actualiza la clase en la ruta correspondiente
                await firebaseClient.Child("clases").Child(clase.CodigoClase).PutAsync(clase);
                return true; // Retorna verdadero si se actualizó correctamente
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la clase: {ex.Message}");
                return false; // Retorna falso si hubo un error
            }
        }

        public async Task<Clase> GetClaseByCodigo(string codigoClase)
        {
            var clase = await firebaseClient.Child("clases").Child(codigoClase).OnceSingleAsync<Clase>();
            return clase; // Devuelve el objeto Clase si existe, o null si no
        }

        //Metodo para inscribir un estudiante a una clase
        public async Task<bool> RegisterEstudianteEnClase(string estudianteId, string codigoClase)
        {
            // Buscar la clase por el código
            var clase = await GetClaseByCodigo(codigoClase);
            if (clase == null)
            {
                return false; // La clase no existe
            }

            // Agregar el ID del estudiante a la lista de inscritos
            await firebaseClient.Child("clases").Child(codigoClase).Child("estudiantes")
                .Child(estudianteId).PutAsync(true);

            // También agregar la clase a las clases inscritas del estudiante
            var estudiante = await GetEstudiante(estudianteId);
            if (estudiante != null)
            {
                await firebaseClient.Child("estudiantes").Child(estudianteId).Child("clases")
                    .Child(codigoClase).PutAsync(true);
            }

            return true; // Inscripción exitosa
        }

        public async Task<bool> IsEstudianteInscrito(string estudianteId, string codigoClase)
        {
            var estudiante = await GetEstudiante(estudianteId);
            if (estudiante?.Clases != null)
            {
                return estudiante.Clases.ContainsKey(codigoClase); // Verifica si la clase está en las clases inscritas
            }
            return false; // Si no hay clases inscritas, retorna falso
        }
        public async Task<bool> CrearEvaluacion(string claseId, Evaluacion evaluacion, string idEvaluacion)
        {
            try
            {
                // Guarda la evaluación en la ruta correspondiente usando el ID generado
                await firebaseClient.Child("clases").Child(claseId).Child("evaluaciones")
                    .Child(idEvaluacion) // Usar el ID generado
                    .PutAsync(evaluacion); // Guarda la evaluación

                return true; // Retorna verdadero si se guardó correctamente
            }
            catch
            {
                return false; // Retorna falso si hubo un error
            }
        }

        public async Task<Evaluacion> GetEvaluacionById(string idEvaluacion)
        {
            // Este método buscará la evaluación en la ruta correspondiente
            var evaluacion = await firebaseClient.Child("evaluaciones").Child(idEvaluacion).OnceSingleAsync<Evaluacion>();
            return evaluacion; // Devuelve el objeto Evaluacion si existe, o null si no
        }

        public async Task<List<Estudiante>> GetEstudiantesPorClase(string codigoClase)
        {
            var clase = await firebaseClient.Child("clases").Child(codigoClase).OnceSingleAsync<Clase>();
            var estudiantes = new List<Estudiante>();

            if (clase?.estudiantes != null)
            {
                foreach (var estudianteEntry in clase.estudiantes)
                {
                    // Obtener los detalles del estudiante usando su ID
                    var estudiante = await firebaseClient.Child("estudiantes").Child(estudianteEntry.Key).OnceSingleAsync<Estudiante>();
                    if (estudiante != null)
                    {
                        estudiantes.Add(estudiante); // Agregar el estudiante a la lista
                    }
                }
            }

            return estudiantes; // Retornar la lista de estudiantes
        }
        public async Task<bool> AsignarNota(string codigoClase, string codigoEvaluacion, string estudianteId, Nota nota)
        {
            try
            {
                // Guarda la nota en la ruta correspondiente
                await firebaseClient.Child("clases")
                    .Child(codigoClase)
                    .Child("evaluaciones")
                    .Child(codigoEvaluacion)
                    .Child("notas")
                    .Child(estudianteId)
                    .PutAsync(nota);

                return true; // Retorna verdadero si se guardó correctamente
            }
            catch
            {
                return false; // Retorna falso si hubo un error
            }
        }

        public async Task<Nota> GetNotaPorEvaluacion(string estudianteId, string codigoClase, string evaluacionId)
        {
            var evaluacion = await firebaseClient.Child("clases")
                .Child(codigoClase)
                .Child("evaluaciones")
                .Child(evaluacionId)
                .OnceSingleAsync<Evaluacion>();

            if (evaluacion?.Notas != null && evaluacion.Notas.ContainsKey(estudianteId))
            {
                return evaluacion.Notas[estudianteId]; // Devuelve la nota si existe
            }

            return null; // Si no se encuentra la nota
        }
    }
}