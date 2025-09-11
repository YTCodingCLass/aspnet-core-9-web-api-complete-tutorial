namespace RepositoryAndServicesApi;

public interface ISingletonService
{
    Guid InstanceId { get; }
    DateTime CreatedAt { get; }
}

public interface IScopedService  
{
    Guid InstanceId { get; }
    DateTime CreatedAt { get; }
}

public interface ITransientService
{
    Guid InstanceId { get; }
    DateTime CreatedAt { get; }
}

// Implementations
public class SingletonService : ISingletonService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public SingletonService()
    {
        Console.WriteLine($"🔴 Singleton created: {InstanceId}");
    }
}

public class ScopedService : IScopedService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public ScopedService()
    {
        Console.WriteLine($"🟡 Scoped created: {InstanceId}");
    }
}

public class TransientService : ITransientService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public TransientService()
    {
        Console.WriteLine($"🟢 Transient created: {InstanceId}");
    }
}
