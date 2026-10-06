$ErrorActionPreference = "Stop"
$base = Split-Path -Parent $MyInvocation.MyCommand.Definition

Write-Host "Executando desafio de comissão..."
dotnet run --project (Join-Path $base "src/comissao/comissao.csproj")

Write-Host "`nExecutando demonstração do desafio de estoque..."
dotnet run --project (Join-Path $base "src/estoque/estoque.csproj") -- --demo

Write-Host "`nExecutando demonstração do desafio de juros..."
dotnet run --project (Join-Path $base "src/juros/juros.csproj") -- --demo

Write-Host "`nExecutando testes..."
dotnet run --project (Join-Path $base "tests/DesafioTarget.Testes/DesafioTarget.Testes.csproj")
