namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyAsync(string userId, string title, string message, CancellationToken cancellationToken = default);
    }
}
