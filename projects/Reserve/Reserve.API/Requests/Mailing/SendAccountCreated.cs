using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using JetBrains.Annotations;
using MediatR;
using Reserve.API.Services;

namespace Reserve.API.Requests.Mailing
{
    public static class SendAccountCreated
    {
        [PublicAPI]
        public class NotificationEnvelope : INotification
        {
            public string AccountEmail { get; set; }
            
            public string GeneratedPassword { get; set; }
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
                model.AccountEmail = notification.AccountEmail;
                model.Password = notification.GeneratedPassword;
                
                var contents = await _razor.Engine.CompileRenderAsync("AccountCreated", model);
                
                await _mail
                    .To(notification.AccountEmail)
                    .Subject("Account created")
                    .Body(contents, true)
                    .SendAsync(ct);
            }
        }
    }
}