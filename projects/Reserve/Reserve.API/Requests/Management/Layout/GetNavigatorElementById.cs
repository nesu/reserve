using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Reserve.API.Configurations;
using Reserve.API.Resources;
using Reserve.Data;

namespace Reserve.API.Requests.Management.Layout
{
    public static class GetNavigatorElementById
    {
        private static JsonSerializerSettings _serializer = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        [PublicAPI]
        public class RequestEnvelope : IRequest<NavigatorElementResource>
        {
            public long Id { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, NavigatorElementResource>
        {
            private readonly DatabaseContext _context;

            public Handler(DatabaseContext context)
            {
                _context = context;
            }
            
            public async Task<NavigatorElementResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var element = await _context.VisualElements
                    .FirstOrDefaultAsync(it => it.Id == request.Id, ct);
                
                if (element == null)
                    throw new ArgumentNullException(nameof(element));

                var deserialized = JsonConvert.DeserializeObject<NavigatorElementResource>(element.Properties, SerializerConfigurations.DefaultSerializerSettings);
                deserialized.Id = element.Id;

                return deserialized;
            }
        }
    }
}