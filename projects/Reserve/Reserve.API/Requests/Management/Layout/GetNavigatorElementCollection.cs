using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Reserve.API.Configurations;
using Reserve.API.Resources;
using Reserve.Data;
using Reserve.Data.Types;

namespace Reserve.API.Requests.Management.Layout
{
    public static class GetNavigatorElementCollection
    {
        private static JsonSerializerSettings _serializer = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };
        
        [PublicAPI]
        public class RequestEnvelope : IRequest<NavigatorElementResource[]> {}

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, NavigatorElementResource[]>
        {
            private readonly DatabaseContext _context;

            public Handler(DatabaseContext context)
            {
                _context = context;
            }
            
            public async Task<NavigatorElementResource[]> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var elements = await _context.VisualElements
                    .Where(it => it.Type == ElementType.NavigatorElement)
                    .ToArrayAsync(ct);

                var result = new List<NavigatorElementResource>();

                foreach (var element in elements)
                {
                    var deserialized = JsonConvert
                        .DeserializeObject<NavigatorElementResource>(element.Properties, SerializerConfigurations.DefaultSerializerSettings);
                    
                    deserialized.Id = element.Id;
                    result.Add(deserialized);
                }

                return result.ToArray();
            }
        }
    }
}