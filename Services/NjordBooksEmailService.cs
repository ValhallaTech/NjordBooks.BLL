using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using NjordBooks.BLL.Models;

namespace NjordBooks.BLL.Services
{
    public class NjordBooksEmailService : IEmailSender
    {
        private readonly MailSettings mailSettings;
        public NjordBooksEmailService( IOptions<MailSettings> mailSettings ) => this.mailSettings = mailSettings.Value;

        public async Task SendEmailAsync( string email, string subject, string htmlMessage )
        {
            MimeMessage msg = new MimeMessage { Sender = MailboxAddress.Parse( this.mailSettings.Mail ) };
            msg.To.Add( MailboxAddress.Parse( email ) );
            msg.Subject = subject;
            BodyBuilder builder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };
            msg.Body = builder.ToMessageBody( );
            SmtpClient smtp = new SmtpClient( );
            await smtp.ConnectAsync( this.mailSettings.Host, this.mailSettings.Port, SecureSocketOptions.StartTls )
                      .ConfigureAwait( false );
            await smtp.AuthenticateAsync( this.mailSettings.Mail, this.mailSettings.Password )
                      .ConfigureAwait( false );
            await smtp.SendAsync( msg ).ConfigureAwait( false );
            await smtp.DisconnectAsync( true ).ConfigureAwait( false );
        }
    }
}
