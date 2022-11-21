using System;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.FileProviders.Composite;
using RazorLight;

namespace Reserve.API.Requests.Common
{
    public static class SendMail
    {
        [PublicAPI]
        public class NotificationEnvelope : INotification
        {
            
        }

        public class Handler : INotificationHandler<NotificationEnvelope>
        {
            private readonly IFluentEmail _mail;

            public Handler(IFluentEmail mail)
            {
                _mail = mail;
            }
            
            public async Task Handle(NotificationEnvelope notification, CancellationToken ct)
            {
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var engine = new RazorLightEngineBuilder()
                    .UseFileSystemProject(Path.Combine(location, "Assets"))
                    .Build();

                dynamic model = new ExpandoObject();
                model.AccountEmail = "AAA";
                model.Password = "VVV";
                
                var contents = await engine.CompileRenderAsync("AccountCreated", model);
                
                await _mail
                    .To("salted.mail@gmail.com")
                    .Subject("This is test email")
                    .Body(contents, true)
                    .SendAsync(ct);
            }
        }
    }
}