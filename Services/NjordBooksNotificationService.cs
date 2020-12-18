using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NjordBooks.BLL.Data;
using NjordBooks.BLL.Models;

namespace NjordBooks.BLL.Services
{
    public class NjordBooksNotificationService : INjordBooksNotificationService
    {
        private readonly NjordBooksContext context;
        private readonly IEmailSender emailService;

        public NjordBooksNotificationService( NjordBooksContext context, IEmailSender emailService )
        {
            this.context = context;
            this.emailService = emailService;
        }

        public async Task NotifyOverdraft( string userId, BankAccount bankAccount )
        {
            NjordBooksUser user = await this.context.Users.FirstOrDefaultAsync( u => u.Id == userId )
                                            .ConfigureAwait( false );
            Household houseHold = await this.context.Household.FirstOrDefaultAsync( hh => hh.Id == user.HouseholdId )
                                            .ConfigureAwait( false );
            Transaction transaction = bankAccount.Transactions.Last( );

            const string subject = "Overdraft Alert";
            string body =
                $"Your <b>{bankAccount.Name}</b> account has an overdraft. Your current balance is <b>{bankAccount.CurrentBalance}</b>. Offending transaction: <b>{transaction.Amount}</b> for <b>{transaction.CategoryItem.Name}</b> on <b>{transaction.Created}</b>.";
            await this.emailService.SendEmailAsync( user.Email, subject, body ).ConfigureAwait( false );

            Notification notification = new Notification
            {
                HouseholdId = houseHold.Id,
                Created = DateTimeOffset.Now,
                Subject = subject,
                Body = body,
                IsRead = false
            };
            await this.context.AddAsync( notification ).ConfigureAwait( false );
            await this.context.SaveChangesAsync( ).ConfigureAwait( false );
        }

        public async Task<List<Notification>> GetNotificationsAsync( string userId )
        {
            NjordBooksUser user = await this.context.Users.FirstOrDefaultAsync( u => u.Id == userId )
                                            .ConfigureAwait( false );

            return await this.context.Notification.Where( n => n.HouseholdId == user.HouseholdId && !n.IsRead )
                             .ToListAsync( )
                             .ConfigureAwait( false );
        }
    }
}
