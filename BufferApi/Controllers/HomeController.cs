using Microsoft.AspNetCore.Mvc;
using BufferApi.Buffer;
using Microsoft.Extensions.DependencyInjection;
using System;
using BufferApi.Jobs;

public class HomeController : Controller
{
    private readonly CalculatorLogRepository _calculatorLogRepository;

    public HomeController(CalculatorLogRepository calculatorLogRepository)
    {
        _calculatorLogRepository = calculatorLogRepository;
    }

    public IActionResult Index()
    {
        CalculatorLogRepository logRepository = _calculatorLogRepository;
        BufferApi.Models.Home.IndexModel calculatorLogRepository = new();
        calculatorLogRepository.CalculatorLogRepository = logRepository;

        return View(calculatorLogRepository);
    }
}