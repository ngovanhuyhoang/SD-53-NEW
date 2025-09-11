using QuanApi.Data;

namespace QuanApi.Services
{
    public interface IOrderHistoryService
    {
        Task<bool> CanRollbackToStatusAsync(Guid orderId, string targetStatus);
        Task<List<string>> GetValidRollbackStatusesAsync(Guid orderId, string currentStatus);
        Task<bool> RollbackOrderStatusAsync(Guid orderId, string targetStatus, string reason, string updatedBy);
        Task SaveOrderHistoryAsync(Guid orderId, string oldStatus, string newStatus, string updatedBy, string reason = null);
        Task<List<LichSuHoaDon>> GetOrderHistoryAsync(Guid orderId);
    }
}
