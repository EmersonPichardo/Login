using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace login_data_access.Contexts.SecurityContext.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }

        public class Configuration : IEntityTypeConfiguration<User>
        {
            public void Configure(EntityTypeBuilder<User> builder)
            {
                builder.ToTable("users");
                builder.HasKey(user => user.Id).HasName("pk_users");

                builder.HasIndex(user => user.Email).IsUnique().HasDatabaseName("unq_users_name");

                builder.Property(user => user.Id).HasColumnType("int").UseMySqlIdentityColumn().IsRequired();
                builder.Property(user => user.Email).HasColumnType("varchar(75)").IsRequired();
                builder.Property(user => user.Name).HasColumnType("varchar(20)").IsRequired();
                builder.Property(user => user.Password).HasColumnType("varbinary(128)").IsRequired();
                builder.Property(user => user.Salt).HasColumnType("varbinary(32)").IsRequired();
            }
        }
    }
}
