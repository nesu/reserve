using MediatR;

namespace Reserve.API.Requests.Abstractions
{
    public abstract class PageRequest<TResponse> : IRequest<TResponse>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}