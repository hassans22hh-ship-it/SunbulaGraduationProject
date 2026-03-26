using MediatR;

namespace SharedKernel
{
    /// <summary>
    /// Integration event published when a user deletes their account.
    /// Other modules subscribe to this to perform hard cleanup of user data.
    /// </summary>
    public record UserAccountDeletedEvent(Guid UserId) : INotification;
}
