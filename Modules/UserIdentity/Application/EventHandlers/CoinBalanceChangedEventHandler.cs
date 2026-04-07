using System.Threading;
using System.Threading.Tasks;
using Application.Services.Abstraction;
using Domain.Entities.ValueOpjects;
using MediatR;

namespace Application.EventHandlers
{
    public class CoinBalanceChangedEventHandler : INotificationHandler<CoinBalanceChangedEvent>
    {
        private readonly ICoinStreamManager _coinStreamManager;

        public CoinBalanceChangedEventHandler(ICoinStreamManager coinStreamManager)
        {
            _coinStreamManager = coinStreamManager;
        }

        public async Task Handle(CoinBalanceChangedEvent notification, CancellationToken cancellationToken)
        {
            await _coinStreamManager.NotifyCoinChangeAsync(notification);
        }
    }
}
