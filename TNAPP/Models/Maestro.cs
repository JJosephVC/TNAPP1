using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProTCE.Models
{
    public class Maestro
    {
        public string Id { get; set; }// ID del usuario
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public Dictionary<string, bool> Clases { get; set; } = new Dictionary<string, bool>();
    }
}
