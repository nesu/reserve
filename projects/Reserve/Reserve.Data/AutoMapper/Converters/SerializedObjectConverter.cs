using AutoMapper;
using Newtonsoft.Json;

namespace Reserve.Data.AutoMapper.Converters
{
    public class SerializedObjectConverter : IValueConverter<string, object>
    {
        public object Convert(string source, ResolutionContext context)
        {
            return JsonConvert.DeserializeObject(source);
        }
    }
}