using DemoJWT.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoJWT.DAL.EntitiesConfiguration
{
    public class RolUsuarioConfiguration : IEntityTypeConfiguration<RolUsuario>
    {
        public void Configure(EntityTypeBuilder<RolUsuario> builder)
        {
            builder.ToTable("RolesUsuario");
            builder.HasKey(c => new { c.UserId, c.RoleId });

            builder.HasOne(ur => ur.Rol)
               .WithMany(r => r.RolUsuarios)
               .HasForeignKey(ur => ur.RoleId)
               .IsRequired();

            builder.HasOne(ur => ur.Usuario)
           .WithMany(r => r.RolUsuarios)
           .HasForeignKey(ur => ur.UserId)
           .IsRequired();
        }
    }
}
