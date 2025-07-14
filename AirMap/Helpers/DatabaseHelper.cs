using AirMap.Models;
using Microsoft.EntityFrameworkCore;

namespace AirMap.Helpers;

/// <summary>
///     Static helper class for database operations using Entity Framework
/// </summary>
public static class DatabaseHelper
{
    /// <summary>
    ///     Gets all records from a specific table
    /// </summary>
    /// <typeparam name="TEntity">The entity type of the table</typeparam>
    /// <param name="context">The database context</param>
    /// <returns>A list of all records</returns>
    public static List<TEntity> GetAll<TEntity>(DbContext context) where TEntity : class
    {
        return [.. context.Set<TEntity>()];
    }

    /// <summary>
    ///     Gets all records from a specific table asynchronously
    /// </summary>
    /// <typeparam name="TEntity">The entity type of the table</typeparam>
    /// <param name="context">The database context</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all records</returns>
    public static async Task<List<TEntity>> GetAllAsync<TEntity>(DbContext context) where TEntity : class
    {
        return await context.Set<TEntity>().ToListAsync();
    }


    /// <summary>
    ///     Populates missing navigation properties in a list of entities based on their foreign key values.
    /// </summary>
    /// <typeparam name="TEntity">The type of the main entity.</typeparam>
    /// <typeparam name="SEntity">The type of the related entity.</typeparam>
    /// <param name="input">The list of main entities to process.</param>
    /// <param name="context">The database context used to retrieve related entities.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the updated list of entities with
    ///     populated navigation properties.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the related entity type does not have an 'id' property or if the main entity type does not have the
    ///     required foreign key or navigation property.
    /// </exception>
    public static async Task<List<TEntity>> GetMissingData<TEntity, SEntity>(List<TEntity?> input, DbContext context)
    where TEntity : class // T -> Type
    where SEntity : class
    {
        var sEntities = await context.Set<SEntity>().ToListAsync();

        // Location.Id =/= Value, == PropertyInfo
        var sEntityIdProperty = typeof(SEntity).GetProperty("Id") ??
                                throw new InvalidOperationException("SEntity must have an 'Id' property.");
        //var sEntityById = sEntities.ToDictionary(sEntityIdProperty.GetValue);
        var sEntityById = sEntities.ToDictionary(e => sEntityIdProperty.GetValue(e)!);

        var tEntityIdProp = typeof(TEntity)
                                .GetProperties()
                                .FirstOrDefault(p => p.Name.EndsWith(typeof(SEntity).Name + "Id")) ??
                            throw new InvalidOperationException(
                                $"TEntity must have a '{typeof(SEntity).Name}Id' property.");
        var tEntityNavProp = typeof(TEntity)
                                 .GetProperties()
                                 .FirstOrDefault(p => p.PropertyType == typeof(SEntity)) ??
                             throw new InvalidOperationException(
                                 $"TEntity must have a property of type '{typeof(SEntity).Name}'.");
        foreach (var item in input)
        {
            var foreignKeyValue = tEntityIdProp.GetValue(item);
            if (foreignKeyValue != null && sEntityById.TryGetValue(foreignKeyValue, out var sEntity))
                tEntityNavProp.SetValue(item, sEntity);
        }

        return input;
    }

    /// <summary>
    ///     Populates missing navigation properties in a list of entities based on their foreign key values and inserts them
    ///     into the correct place within TEntity as a list of SEntity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the main entity.</typeparam>
    /// <typeparam name="SEntity">The type of the related entity.</typeparam>
    /// <param name="input">The list of main entities to process.</param>
    /// <param name="context">The database context used to retrieve related entities.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the updated list of entities with
    ///     populated navigation properties.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the related entity type does not have an 'id' property or if the main entity type does not have the
    ///     required foreign key or navigation property.
    /// </exception>
    public static async Task<List<TEntity>> GetMissingDataFromList<TEntity, SEntity>(List<TEntity> input, DbContext context)
       where TEntity : class
       where SEntity : class
    {
        var sEntities = await context.Set<SEntity>().ToListAsync();

        var sEntityIdProperty = typeof(SEntity).GetProperty("SourceApiId") ??
                                throw new InvalidOperationException("SEntity must have an 'SourceApiId' property.");
        //var sEntityById = sEntities.ToDictionary(e => sEntityIdProperty.GetValue(e)!);

        var tEntityIdProp = typeof(TEntity)
                                .GetProperties()
                                .FirstOrDefault(p => p.Name.EndsWith(typeof(SEntity).Name + "Ids")) ??
                            throw new InvalidOperationException(
                                $"TEntity must have a '{typeof(SEntity).Name}Ids' property.");
        foreach (var entity in input)
        {
            var value = tEntityIdProp.GetValue(entity);

            if (value is System.Collections.IEnumerable enumerable)
            {
                var isEmpty = !enumerable.Cast<object>().Any();

                if (isEmpty)
                    continue;

                var idList = enumerable
                    .Cast<object>()
                    .Select(x => x == null ? (long?)null : Convert.ToInt64(x))
                    .ToList();

                var sEntityListProp = typeof(TEntity)
                    .GetProperties()
                    .FirstOrDefault(p => p.PropertyType.IsGenericType &&
                                         p.PropertyType.GetGenericTypeDefinition() == typeof(List<>) &&
                                         p.PropertyType.GenericTypeArguments[0] == typeof(SEntity));

                if (sEntityListProp == null)
                    throw new InvalidOperationException($"TEntity must have a List<{typeof(SEntity).Name}> property to hold the entities.");

                var currentList = (List<SEntity>?)sEntityListProp.GetValue(entity);
                if (currentList == null)
                {
                    currentList = new List<SEntity>();
                    sEntityListProp.SetValue(entity, currentList);
                }
                else
                {
                    currentList.Clear();
                }

                var matchingSEntities = sEntities
                    .Where(se => {
                        var idValue = sEntityIdProperty.GetValue(se);
                        if (idValue == null) return false;
                        return long.TryParse(idValue.ToString(), out var idLong) && idList.Contains(idLong);
                    })
                    .ToList();

                foreach (var sEntity in matchingSEntities.Where(sEntity => !currentList.Contains(sEntity)))
                {
                    currentList.Add(sEntity);
                }
            }
            else
            {
                throw new InvalidOperationException($"Property {tEntityIdProp.Name} is not a collection.");
            }
        }

        return input;
    }


}