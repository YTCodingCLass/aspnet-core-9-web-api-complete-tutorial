using Microsoft.AspNetCore.Mvc;

namespace ExceptionHandlingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceLifetimeController : ControllerBase
{
    private readonly ISingletonService singletonService1;
    private readonly ISingletonService singletonService2;

    private readonly IScopedService scopedService1;
    private readonly IScopedService scopedService2;
    
    private readonly ITransientService transientService1;
    private readonly ITransientService transientService2;
    
    public ServiceLifetimeController(
        ISingletonService singleton1,
        ISingletonService singleton2,
        IScopedService scoped1,
        IScopedService scoped2,
        ITransientService transient1,
        ITransientService transient2)
    {
        singletonService1 = singleton1;
        singletonService2 = singleton2;
        scopedService1 = scoped1;
        scopedService2 = scoped2;
        transientService1 = transient1;
        transientService2 = transient2;
        
        Console.WriteLine("🏗️ Controller created with all services");
    }
    
    [HttpGet("demo")]
    public ActionResult<object> GetServiceLifetimeDemo()
    {
        return Ok(new
        {
            Explanation = new
            {
                Singleton = "Same instance across entire application lifetime",
                Scoped = "Same instance within a single HTTP request", 
                Transient = "New instance every time service is requested"
            },
            Results = new
            {
                Singleton = new
                {
                    Instance1_Id = singletonService1.InstanceId,
                    Instance2_Id = singletonService2.InstanceId,
                    Instance1_Time = singletonService1.CreatedAt,
                    Instance2_Time = singletonService2.CreatedAt,
                    AreSame = singletonService1.InstanceId == singletonService2.InstanceId
                },
                Scoped = new
                {
                    Instance1_Id = scopedService1.InstanceId,
                    Instance2_Id = scopedService2.InstanceId,
                    Instance1_Time = scopedService1.CreatedAt,
                    Instance2_Time = scopedService2.CreatedAt,
                    AreSame = scopedService1.InstanceId == scopedService2.InstanceId
                },
                Transient = new
                {
                    Instance1_Id = transientService1.InstanceId,
                    Instance2_Id = transientService2.InstanceId,
                    Instance1_Time = transientService1.CreatedAt,
                    Instance2_Time = transientService2.CreatedAt,
                    AreSame = transientService1.InstanceId == transientService2.InstanceId
                }
            },
            RequestTime = DateTime.UtcNow
        });
    }
}