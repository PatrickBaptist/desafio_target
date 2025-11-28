set -e
BASE_DIR=$(dirname "$(realpath "$0")")


echo "Executando DesafioComissao..."
cd "$BASE_DIR/src/comissao"
dotnet run


echo "\nExecutando DesafioEstoque..."
cd "$BASE_DIR/src/estoque"
dotnet run


echo "\nExecutando DesafioJuros..."
cd "$BASE_DIR/src/juros"
dotnet run