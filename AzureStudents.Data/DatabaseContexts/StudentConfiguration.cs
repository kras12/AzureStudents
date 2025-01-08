using AzureStudents.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzureStudents.Data.DatabaseContexts;

/// <summary>
/// Configuration class for the student entity.
/// </summary>
public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    /// <inheritdoc/>    
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
