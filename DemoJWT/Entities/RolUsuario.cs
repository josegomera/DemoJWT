using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoJWT.Entities
{
    public class RolUsuario : IdentityUserRole<int>
    {
        public virtual Usuario Usuario { get; set; }
        public virtual Rol Rol { get; set; }

    }
}
