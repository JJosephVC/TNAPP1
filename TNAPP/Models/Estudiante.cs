using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProTCE.Models
{
    public class Estudiante
    {
        public string Id { get; set; } // ID del usuario
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Genero { get; set; }
        public Dictionary<string, bool> Clases { get; set; } = new Dictionary<string, bool>();
        public object Nombre { get; internal set; }
    }
}
