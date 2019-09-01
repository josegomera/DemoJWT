using DemoJWT.DAL.EntitiesConfiguration;
using DemoJWT.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoJWT.DAL.SQL
{
    public class DemoJWTDbContext : IdentityDbContext<Usuario, Rol, int, IdentityUserClaim<int>, RolUsuario, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DemoJWTDbContext(DbContextOptions<DemoJWTDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UsuarioConfiguration());
            builder.ApplyConfiguration(new RolConfiguration());
            builder.ApplyConfiguration(new RolUsuarioConfiguration());
        }

        public DbSet<Usuario> Usuarios { get; set; }
        // Estamos utilizando la propiedad en singular para evitar conflicto con Identity.
        public DbSet<Rol> Rol { get; set; }
        public DbSet<RolUsuario> RolesUsuarios { get; set; }
    }
}
