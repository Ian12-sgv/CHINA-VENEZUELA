$ErrorActionPreference = 'Stop'

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue

if ($null -eq $dotnet) {
    Write-Host 'No se encontro dotnet en este servidor.' -ForegroundColor Red
    exit 1
}

Write-Host "dotnet: $($dotnet.Source)" -ForegroundColor Cyan
Write-Host ''
Write-Host 'Runtimes instalados:' -ForegroundColor Cyan

$runtimes = & dotnet --list-runtimes
$runtimes

$hasCore10 = $runtimes | Where-Object { $_ -match '^Microsoft\.NETCore\.App 10\.' }
$hasAspNetCore10 = $runtimes | Where-Object { $_ -match '^Microsoft\.AspNetCore\.App 10\.' }

Write-Host ''

if ($hasCore10 -and $hasAspNetCore10) {
    Write-Host 'Correcto: el servidor puede ejecutar la API ASP.NET Core 10.' -ForegroundColor Green
    exit 0
}

Write-Host 'Falta ASP.NET Core Runtime 10 x64 para ejecutar la API China-Venezuela.' -ForegroundColor Yellow
Write-Host 'Las versiones existentes de .NET no se eliminan ni se reemplazan al instalar .NET 10.' -ForegroundColor Yellow
exit 2
