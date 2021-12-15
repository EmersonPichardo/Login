using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace login_data_access.Contexts.SecurityContext.Models
{
    public class Application
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        public class Configuration : IEntityTypeConfiguration<Application>
        {
            public void Configure(EntityTypeBuilder<Application> builder)
            {
                builder.ToTable("applications");
                builder.HasKey(application => application.Id).HasName("pk_applications");

                builder.HasIndex(application => application.Name).HasDatabaseName("ind_applications_name");

                builder.Property(application => application.Id).HasColumnType("char(32)").IsRequired();
                builder.Property(application => application.Name).HasColumnType("varchar(64)").IsRequired();
                builder.Property(application => application.Name).HasColumnType("varchar(16)").IsRequired();
            }
        }
    }
}
