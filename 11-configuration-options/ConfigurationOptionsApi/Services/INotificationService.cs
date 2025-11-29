// Services/INotificationService.cs
using ConfigurationOptionsApi.Models.DTOs;

namespace ConfigurationOptionsApi.Services;

public interface INotificationService
{
    Task SendLowStockAlertAsync(IEnumerable<ProductResponseDto> products);
    Task SendProductCreatedNotificationAsync(ProductResponseDto product);
    Task SendProductDeletedNotificationAsync(string productName);
    Task SendStockChangeNotificationAsync(string productName, int oldStock, int newStock);
}
