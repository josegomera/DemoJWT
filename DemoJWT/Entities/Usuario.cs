using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoJWT.Entities
{
    public class Usuario : IdentityUser<int>
    {
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Estado { get; set; }
        public ICollection<RolUsuario> RolUsuarios { get; set; }
    }
}
