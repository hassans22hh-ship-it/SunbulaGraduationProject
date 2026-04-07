using Application.Services.Abstraction;
using Domain.Contracts;
using MediatR;
using SharedKernel;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.EventHandlers
{
    /// <summary>
    /// Handles the CoinsEarnedEvent by awarding coins to the user's balance.
    /// This is an integration handler that reacts to events in the TimeTracking module.
    /// </summary>
    public class CoinsEarnedEventHandler : INotificationHandler<CoinsEarnedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoinsEarnedEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CoinsEarnedEvent notification, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(notification.UserId, cancellationToken);
            if (user == null) return;

            // Business Rule: Convert decimal coins from TimeTracking to integer for UserIdentity balance
            int amount = (int)Math.Round(notification.CoinsAmount, MidpointRounding.AwayFromZero);

            if (amount > 0)
            {
                user.AddCoins(amount, "Time Tracking Session Reward Update");
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else if (amount < 0)
            {
                // If amount is negative, it means the session was shortened or behavior changed.
                // We deduct the difference.
                user.SpendCoins(Math.Abs(amount), "Time Tracking Session Adjustment");
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
