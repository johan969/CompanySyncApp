using CompanySyncApp.Context;
using CompanySyncApp.Models;
using CompanySyncApp.Services;
using CsvHelper;
using Microsoft.EntityFrameworkCore;

namespace CompanySyncApp;

internal static class Program
{
    private static async Task Main()
    {
        var service = new ImportSyncService();
        await service.ImportAsync();
    }
    }

