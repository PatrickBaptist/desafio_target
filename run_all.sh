#!/usr/bin/env bash
set -euo pipefail
BASE_DIR=$(dirname "$(realpath "$0")")

echo "Executando desafio de comissão..."
dotnet run --project "$BASE_DIR/src/comissao/comissao.csproj"

printf '\nExecutando demonstração do desafio de estoque...\n'
dotnet run --project "$BASE_DIR/src/estoque/estoque.csproj" -- --demo

printf '\nExecutando demonstração do desafio de juros...\n'
dotnet run --project "$BASE_DIR/src/juros/juros.csproj" -- --demo

printf '\nExecutando testes...\n'
dotnet run --project "$BASE_DIR/tests/DesafioTarget.Testes/DesafioTarget.Testes.csproj"
