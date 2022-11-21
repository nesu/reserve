using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Reserve.Data
{
    public class JsonObjectConverter<T> : ValueConverter<T, string> where T : class
    {
        private static T Deserialize(string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }

        private static string Serialize(T input)
        {
            return JsonConvert.SerializeObject(input);
        }
        
        public JsonObjectConverter([CanBeNull] ConverterMappingHints hints = default) 
            : base(v => Serialize(v), v => Deserialize(v), hints)
        { }
    }
}