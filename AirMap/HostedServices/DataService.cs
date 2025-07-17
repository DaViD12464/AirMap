using AirMap.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using Newtonsoft.Json;
using System.Collections.Generic;
using AirMap.DTOs;

[ApiController]
[Route("api/[controller]")]
public class DataService : ControllerBase
{
    private readonly ILogger<DataService> _logger;

    public DataService(ILogger<DataService> logger)
    {
        _logger = logger;
    }
} 