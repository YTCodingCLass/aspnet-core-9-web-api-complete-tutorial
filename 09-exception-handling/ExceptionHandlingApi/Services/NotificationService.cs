// Services/NotificationService.cs
using Microsoft.Extensions.Logging;
using ExceptionHandlingApi.Models.DTOs;

namespace ExceptionHandlingApi.Services;

public class NotificationService(ILogger<NotificationService> logger) : INotificationService
{
    public async Task SendLowStockAlertAsync(IEnumerable<ProductResponseDto> products)
    {
        logger.LogWarning(" Notification: Sending low stock alert for {Count} products", products.Count());
        
        // Simulate email/SMS sending
        await Task.Delay(100);
        
        foreach (var product in products)
        {
            logger.LogWarning(" LOW STOCK ALERT: {ProductName} - Stock: {Stock}", 
                product.Name, product.StockQuantity);
        }
    }

    public async Task SendProductCreatedNotificationAsync(ProductResponseDto product)
    {
        logger.LogInformation(" Notification: Product created - {ProductName}", product.Name);
        
        await Task.Delay(50);
        
        logger.LogInformation(" NEW PRODUCT: {ProductName} - Price: ${Price}", 
            product.Name, product.Price);
    }

    public async Task SendProductDeletedNotificationAsync(string productName)
    {
        logger.LogInformation(" Notification: Product deleted - {ProductName}", productName);
        
        await Task.Delay(50);
        
        logger.LogWarning(" PRODUCT DELETED: {ProductName}", productName);
    }

    public async Task SendStockChangeNotificationAsync(string productName, int oldStock, int newStock)
    {
        logger.LogInformation(" Notification: Stock change for {ProductName}: {OldStock} → {NewStock}", 
            productName, oldStock, newStock);
        
        await Task.Delay(50);
        
        logger.LogInformation(" STOCK UPDATE: {ProductName} - {OldStock} → {NewStock}", 
            productName, oldStock, newStock);
    }
}
