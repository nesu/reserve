using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Reserve.Data.Traits;

namespace Reserve.Data.Extensions
{
    public static class ModelBinderExtensions
    {
        public static void ConfigureTimestampedEntities(this ModelBuilder builder)
        {
            foreach (var type in builder.Model.GetEntityTypes())
            {
                if (typeof(ITimestamped).IsAssignableFrom(type.ClrType))
                {
                    builder.Entity(type.ClrType)
                        .Property<DateTime>(nameof(ITimestamped.CreatedAt))
                        .HasDefaultValueSql("NOW()");
                    
                    
                    builder.Entity(type.ClrType)
                        .Property<DateTime>(nameof(ITimestamped.UpdatedAt))
                        .HasDefaultValueSql("NOW()");
                }
            }
        }
        
        public static void UseUnderscoreNaming(this ModelBuilder builder)
        {
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName().ToUnderscoreCase());
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.Name.ToUnderscoreCase());
                }
            }
        }

        private static string ToUnderscoreCase(this string str) {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
        }
    }
}