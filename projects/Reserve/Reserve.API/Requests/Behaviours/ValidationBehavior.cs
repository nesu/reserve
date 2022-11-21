using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Reserve.API.Requests.Behaviours
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IValidatorFactory _factory;

        public ValidationBehavior(IValidatorFactory factory)
        {
            _factory = factory;
        }
        
        public async Task<TResponse> Handle(TRequest request, CancellationToken ct, RequestHandlerDelegate<TResponse> next)
        {
            var validator = _factory.GetValidator(request.GetType());
            var result = validator?.Validate(request);

            if (result != null && !result.IsValid)
                throw new ValidationException(result.Errors.First().ErrorMessage);

            return await next();
        }
    }
}