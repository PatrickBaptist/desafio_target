set -e
BASE_DIR=$(dirname "$(realpath "$0")")


echo "Executando DesafioComissao..."
cd "$BASE_DIR/src/DesafioComissao"
dotnet run


echo "\nExecutando DesafioEstoque..."
cd "$BASE_DIR/src/DesafioEstoque"
dotnet run


echo "\nExecutando DesafioJuros..."
cd "$BASE_DIR/src/DesafioJuros"
dotnet run