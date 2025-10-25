// Services/INotificationService.cs
using CustomMiddlewareApi.Models.DTOs;

namespace CustomMiddlewareApi.Services;

public interface INotificationService
{
    Task SendLowStockAlertAsync(IEnumerable<ProductResponseDto> products);
    Task SendProductCreatedNotificationAsync(ProductResponseDto product);
    Task SendProductDeletedNotificationAsync(string productName);
    Task SendStockChangeNotificationAsync(string productName, int oldStock, int newStock);
}
