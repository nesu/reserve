using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reserve.Data.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> builder) where T : class
        {
            builder
                .HasConversion(new JsonObjectConverter<T>())
                .Metadata.SetValueComparer(new JsonObjectComparer<T>());
            
            return builder;
        }
    }
}