$base = Split-Path -Parent $MyInvocation.MyCommand.Definition
Write-Host "Executando DesafioComissao..."
Push-Location (Join-Path $base "src/comissao")
dotnet run
Pop-Location


Write-Host "\nExecutando DesafioEstoque..."
Push-Location (Join-Path $base "src/estoque")
dotnet run
Pop-Location


Write-Host "\nExecutando DesafioJuros..."
Push-Location (Join-Path $base "src/juros")
dotnet run
Pop-Location