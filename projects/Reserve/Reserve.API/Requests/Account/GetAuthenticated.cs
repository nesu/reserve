using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Reserve.API.Extensions;
using Reserve.API.Resources;
using Reserve.Data;

namespace Reserve.API.Requests.Account
{
    public static class GetAuthenticated
    {
        /// <summary>
        /// Retrieve currently authenticated account, map its' model
        /// to <see cref="AccountResource"/>.
        /// </summary>
        [PublicAPI]
        public class RequestEnvelope : IRequest<AccountResource> { }
        
        public class Handler : IRequestHandler<RequestEnvelope, AccountResource>
        {
            private readonly DatabaseContext _context;
            private readonly IHttpContextAccessor _accessor;
            private readonly IMapper _mapper;

            public Handler(
                DatabaseContext context, 
                IHttpContextAccessor accessor,
                IMapper mapper)
            {
                _context = context;
                _accessor = accessor;
                _mapper = mapper;
            }
            
            public Task<AccountResource> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var email = _accessor.HttpContext.User.Identity.GetIdentifyingValue();

                var account = _context.Accounts
                    .ProjectTo<AccountResource>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(it => it.Email == email, ct);

                if (account == null)
                    throw new UnauthorizedAccessException();

                return account;
            }
        }
    }
}