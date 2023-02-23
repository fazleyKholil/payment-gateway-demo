using Microsoft.AspNetCore.Mvc;

namespace Bank.Processor.Controllers;

[ApiController]
[Route("[controller]")]
public class AdministrationController
{
    private readonly ILogger<AdministrationController> _logger;

    public AdministrationController(ILogger<AdministrationController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "Administration")]
    public string Administration()
    {
        var iteration = 1;  
        _logger.LogDebug($"Debug {iteration}");
        _logger.LogInformation($"Information {iteration}");
        _logger.LogWarning($"Warning {iteration}");
        _logger.LogError($"Error {iteration}");
        _logger.LogCritical($"Critical {iteration}");
        
        return "OK";
    }
}