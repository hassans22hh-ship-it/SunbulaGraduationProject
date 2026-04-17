using System;
using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Threading.Tasks;
using Application.Services.Abstraction;
using Domain.Entities.ValueObjects;

namespace UserIdentityInfrastructure.Services
{
    public class CoinStreamManager : ICoinStreamManager
    {
        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Channel<CoinBalanceChangedEvent>, byte>> _userChannels = new();

        public Channel<CoinBalanceChangedEvent> Subscribe(Guid userId)
        {
            var channel = Channel.CreateUnbounded<CoinBalanceChangedEvent>();
            
            var channelsDict = _userChannels.GetOrAdd(userId, _ => new ConcurrentDictionary<Channel<CoinBalanceChangedEvent>, byte>());
            channelsDict.TryAdd(channel, 1);
            
            return channel;
        }

        public void Unsubscribe(Guid userId, Channel<CoinBalanceChangedEvent> channel)
        {
            if (_userChannels.TryGetValue(userId, out var channelsDict))
            {
                channelsDict.TryRemove(channel, out _);
                if (channelsDict.IsEmpty)
                {
                    _userChannels.TryRemove(userId, out _);
                }
            }
        }

        public async ValueTask NotifyCoinChangeAsync(CoinBalanceChangedEvent coinEvent)
        {
            if (_userChannels.TryGetValue(coinEvent.UserId, out var channelsDict))
            {
                foreach (var channel in channelsDict.Keys)
                {
                    try
                    {
                        await channel.Writer.WriteAsync(coinEvent);
                    }
                    catch (ChannelClosedException)
                    {
                        // Ignore if channel is closed
                    }
                }
            }
        }
    }
}
