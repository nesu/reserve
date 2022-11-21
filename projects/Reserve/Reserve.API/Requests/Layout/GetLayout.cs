using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Reserve.API.Requests.Handlers;

namespace Reserve.API.Requests.Layout
{
    public static class GetLayout
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ResponseEnvelope> { }

        [PublicAPI]
        public class ResponseEnvelope
        {
            
        }

        [UsedImplicitly]
        public class Handler : AuthenticatedRequestHandler<RequestEnvelope, ResponseEnvelope>
        {
            public Handler(IHttpContextAccessor accessor) : base(accessor)
            {
                
            }

            public override Task<ResponseEnvelope> Handle(RequestEnvelope request, CancellationToken ct)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}