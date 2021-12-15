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

                builder.HasIndex(sesion => sesion.User_Id).HasDatabaseName("ind_sesions_user_id");

                builder.Property(sesion => sesion.Token).HasColumnType("varbinary(16)").UseMySqlComputedColumn().IsRequired();
                builder.HasKey(sesion => sesion.Token).HasName("pk_sesions");
                builder.Property(sesion => sesion.User_Id).HasColumnType("int").IsRequired();
                builder.Property(sesion => sesion.ValidUntil).HasColumnType("datetime").IsRequired();

                builder.HasOne<User>().WithMany().HasPrincipalKey(sesion => sesion.Id).HasForeignKey(sesion => sesion.User_Id).HasConstraintName("fk_sesions_users");
            }
        }
    }
}
