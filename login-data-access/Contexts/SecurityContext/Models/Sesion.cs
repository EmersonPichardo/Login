using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace login_data_access.Contexts.SecurityContext.Models
{
    public class Sesion
    {
        public byte[] Token { get; set; }
        public int User_Id { get; set; }
        public DateTime ValidUntil { get; set; }

        public class Configuration : IEntityTypeConfiguration<Sesion>
        {
            public void Configure(EntityTypeBuilder<Sesion> builder)
            {
                builder.ToTable("sesions");

                builder.Property(user => user.Token).HasColumnType("varbinary(64)").UseMySqlComputedColumn().IsRequired();
                builder.HasKey(user => user.Token).HasName("pk_sesions");
                builder.Property(user => user.User_Id).HasColumnType("int").IsRequired();
                builder.Property(user => user.ValidUntil).HasColumnType("datetime").IsRequired();

                builder.HasOne<User>().WithMany().HasPrincipalKey(user => user.Id).HasForeignKey(sesion => sesion.User_Id);
            }
        }
    }
}
