namespace Application.Interfaces
{
    public interface IFraudDetectionService
    {
        /// Returns a flag reason if the transaction looks suspicious, otherwise null.
        Task<string?> EvaluateAsync(string userId, decimal amount, CancellationToken cancellationToken = default);
    }
}
