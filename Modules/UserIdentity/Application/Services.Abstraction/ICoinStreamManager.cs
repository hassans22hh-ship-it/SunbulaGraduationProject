using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Domain.Entities.ValueObjects;

namespace Application.Services.Abstraction
{
    public interface ICoinStreamManager
    {
        Channel<CoinBalanceChangedEvent> Subscribe(Guid userId);
        void Unsubscribe(Guid userId, Channel<CoinBalanceChangedEvent> channel);
        ValueTask NotifyCoinChangeAsync(CoinBalanceChangedEvent coinEvent);
    }
}
