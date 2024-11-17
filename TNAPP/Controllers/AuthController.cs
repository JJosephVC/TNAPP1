using Firebase.Auth;
using ProTCE.Database;
using ProTCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProTCE.Controllers
{
    public class AuthController
    {
        private readonly FirebaseConnection firebaseConnection;
        public AuthController()
        {
            firebaseConnection = new FirebaseConnection();
        }

        public async Task<string> Login(string email, string password)
        {
            return await firebaseConnection.SignIn(email, password);
        }

        //Modificacion del metodo register
        public async Task<string> Register(string email, string password, string role,
        string nombres, string apellidos, string genero = null)
        {
            try
            {
                //Crear usuario en Firebase Authentication
                var userId = await firebaseConnection.CreateUser(email, password);
                if (!string.IsNullOrEmpty(userId))
                {
                    // Guardar información del usuario en la base de datos
                    if (role == "maestro")
                    {
                        var maestro = new Maestro
                        {
                            Id = userId,
                            Nombres = nombres,
                            Apellidos = apellidos,
                            Email = email
                        };
                        await firebaseConnection.CreateMaestro(maestro);
                    }
                    else if (role == "estudiante")
                    {
                        var estudiante = new Estudiante
                        {
                            Id = userId,
                            Nombres = nombres,
                            Apellidos = apellidos,
                            Genero = genero // Solo para estudiantes
                        };
                        await firebaseConnection.CreateEstudiante(estudiante);
                    }
                }
                //return await firebaseConnection.CreateUser(email, password);
                return userId;
            }
            catch(FirebaseAuthException ex)
            {
                // Comprobar el mensaje de la excepción
                if (ex.Message.Contains("email address is already in use"))
                {
                    throw new Exception("El correo electrónico ya está en uso."); // Lanzar excepción para manejar en la UI
                }
                throw; // Lanzar otras excepciones sin cambios
            }
        }
        //metodo register anterior
        /*public async Task<string> Register(string email, string password)
        {
            return await firebaseConnection.CreateUser(email, password);
        }*/

        // Método para obtener el rol del usuario
        public async Task<string> GetUserRole(string userId)
        {
            var maestro = await firebaseConnection.GetMaestro(userId);
            if (maestro != null) return "maestro";

            var estudiante = await firebaseConnection.GetEstudiante(userId);
            if (estudiante != null) return "estudiante";

            return null; // Rol no encontrado
        }

        // Nuevos métodos para obtener datos de maestro y estudiante
        public async Task<Maestro> GetMaestro(string userId)
        {
            return await firebaseConnection.GetMaestro(userId);
        }

        public async Task<Estudiante> GetEstudiante(string userId)
        {
            return await firebaseConnection.GetEstudiante(userId);
        }
    }
}
