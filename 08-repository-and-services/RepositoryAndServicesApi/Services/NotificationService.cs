// Services/NotificationService.cs
using Microsoft.Extensions.Logging;
using RepositoryAndServicesApi.Models.DTOs;

namespace RepositoryAndServicesApi.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendLowStockAlertAsync(IEnumerable<ProductResponseDto> products)
    {
        _logger.LogWarning(" Notification: Sending low stock alert for {Count} products", products.Count());
        
        // Simulate email/SMS sending
        await Task.Delay(100);
        
        foreach (var product in products)
        {
            _logger.LogWarning(" LOW STOCK ALERT: {ProductName} - Stock: {Stock}", 
                product.Name, product.StockQuantity);
        }
    }

    public async Task SendProductCreatedNotificationAsync(ProductResponseDto product)
    {
        _logger.LogInformation(" Notification: Product created - {ProductName}", product.Name);
        
        await Task.Delay(50);
        
        _logger.LogInformation(" NEW PRODUCT: {ProductName} - Price: ${Price}", 
            product.Name, product.Price);
    }

    public async Task SendProductDeletedNotificationAsync(string productName)
    {
        _logger.LogInformation(" Notification: Product deleted - {ProductName}", productName);
        
        await Task.Delay(50);
        
        _logger.LogWarning(" PRODUCT DELETED: {ProductName}", productName);
    }

    public async Task SendStockChangeNotificationAsync(string productName, int oldStock, int newStock)
    {
        _logger.LogInformation(" Notification: Stock change for {ProductName}: {OldStock} → {NewStock}", 
            productName, oldStock, newStock);
        
        await Task.Delay(50);
        
        _logger.LogInformation(" STOCK UPDATE: {ProductName} - {OldStock} → {NewStock}", 
            productName, oldStock, newStock);
    }
}
