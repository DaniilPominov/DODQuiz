using DODQuiz.Core.Entyties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DODQuiz.Infrastructure.Data.Config
{
    internal class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(e => e.Id);

            builder.HasMany(r => r.Users).WithMany(p => p.Roles);
        }
    }
}
