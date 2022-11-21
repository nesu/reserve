using System;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using JetBrains.Annotations;
using MediatR;
using Reserve.API.Services;

namespace Reserve.API.Requests.Mailing
{
    public static class SendEmployeeReservationNotification
    {
        [PublicAPI]
        public class NotificationEnvelope : INotification
        {
            public string TargetEmail { get; set; }
            
            public string DateTimeFormatted { get; set; }
            
            public double Price { get; set; }
            
            public int Duration { get; set; }
        }

        [UsedImplicitly]
        public class Handler : INotificationHandler<NotificationEnvelope>
        {
            private readonly IFluentEmail _mail;
            private readonly RazorMailing _razor;

            public Handler(IFluentEmail mail, RazorMailing razor)
            {
                _mail = mail;
                _razor = razor;
            }
            
            public async Task Handle(NotificationEnvelope notification, CancellationToken ct)
            {
                dynamic model = new ExpandoObject();
                model.DateTimeFormatted = notification.DateTimeFormatted;
                model.Price = notification.Price;
                model.Duration = notification.Duration;
                
                var contents = await _razor.Engine.CompileRenderAsync("EmployeeReservationNotification", model);
                
                await _mail
                    .To(notification.TargetEmail)
                    .Subject("New reservation assignment")
                    .Body(contents, true)
                    .SendAsync(ct);
            }
        }
    }
}