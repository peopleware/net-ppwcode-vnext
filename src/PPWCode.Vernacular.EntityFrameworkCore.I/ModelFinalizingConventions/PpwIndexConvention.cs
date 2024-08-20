using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace PPWCode.Vernacular.EntityFrameworkCore.I.ModelFinalizingConventions;

public class PpwIndexConvention : IModelFinalizingConvention
{
    /// <inheritdoc />
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        // Is not working correctly at the moment!
        // Indices that are explicit being set, will not be changed
        // The name of the index starts with
        // * IX_ => index that doesn't contain any navigational properties
        // * IX_FK => index that starts with at least one navigational property
        foreach (IConventionEntityType entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            // If we have a TCP strategy, an index on a abstract class will occur for each leaf type
            string? strategy = entityType.GetMappingStrategy();
            if ((strategy == RelationalAnnotationNames.TpcMappingStrategy) && entityType.IsAbstract())
            {
                continue;
            }

            foreach (IConventionIndex index in entityType.GetIndexes())
            {
                IConventionAnnotation? nameAnnotation =
                    index
                        .GetAnnotations()
                        .SingleOrDefault(a => a.Name == RelationalAnnotationNames.Name);
                if (nameAnnotation is null || (nameAnnotation.GetConfigurationSource() == ConfigurationSource.Convention))
                {
                    string tableName =
                        entityType.GetTableName()
                        ?? entityType.GetDefaultTableName()
                        ?? entityType.ClrType.Name;
                    string? schema =
                        entityType.GetSchema()
                        ?? entityType.GetDefaultSchema();
                    string name = schema is null ? tableName : $"{schema}_{tableName}";
                    string propertyNames = string.Join("_", index.Properties.Select(p => p.Name));
                    bool firstPropertyIsNavigational = index.Properties.FirstOrDefault(p => p.IsForeignKey()) is not null;
                    string indexName =
                        firstPropertyIsNavigational
                            ? $"IX_FK_{name}_{propertyNames}"
                            : $"IX_{name}_{propertyNames}";
                    index.SetDatabaseName(indexName);
                }
            }
        }
    }
}
