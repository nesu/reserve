using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Reserve.Data;
using Reserve.Data.Types;

namespace Reserve.API.Requests.Account
{
    public static class Authenticate
    {
        [PublicAPI]
        public class RequestEnvelope : IRequest<ResponseEnvelope>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [PublicAPI]
        public class ResponseEnvelope
        {
            public string SecurityToken { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<RequestEnvelope, ResponseEnvelope>
        {
            private readonly DatabaseContext _context;
            private readonly IConfiguration _configurations;

            public Handler(DatabaseContext context, IConfiguration configurations)
            {
                _context = context;
                _configurations = configurations;
            }
            
            public async Task<ResponseEnvelope> Handle(RequestEnvelope request, CancellationToken ct)
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(it => it.Email == request.Email, ct);
                
                if (account == null)
                    throw new UnauthorizedAccessException();
                
                if (!BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash))
                    throw new UnauthorizedAccessException();
                
                var secret = _configurations
                    .GetSection("Authentication")
                    .GetSection("JWTSecret")
                    .Value;
                
                var key = Encoding.UTF8.GetBytes(secret);
                
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                
                var descriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Email, account.Email),
                        new Claim(ClaimTypes.Role, Enum.GetName(typeof(RoleType), account.Role)),
                    }),
                    Expires = DateTime.Now.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),
                };

                var st = handler.CreateToken(descriptor);
                
                return new ResponseEnvelope
                {
                    SecurityToken = handler.WriteToken(st)
                };
            }
        }
    }
}