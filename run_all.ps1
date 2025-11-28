$base = Split-Path -Parent $MyInvocation.MyCommand.Definition
Write-Host "Executando DesafioComissao..."
Push-Location (Join-Path $base "src/DesafioComissao")
dotnet run
Pop-Location


Write-Host "\nExecutando DesafioEstoque..."
Push-Location (Join-Path $base "src/DesafioEstoque")
dotnet run
Pop-Location


Write-Host "\nExecutando DesafioJuros..."
Push-Location (Join-Path $base "src/DesafioJuros")
dotnet run
Pop-Location