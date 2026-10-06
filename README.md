# Desafio técnico Target Sistemas

Solução em C# para os três exercícios propostos pela Target Sistemas: cálculo de comissão, movimentação de estoque e cálculo de juros por atraso.

## Decisões da solução

- Valores monetários usam `decimal`, tipo apropriado para dinheiro porque evita erros comuns de arredondamento do `double`.
- Cada regra de negócio fica separada da interação pelo console, facilitando leitura e testes.
- As comissões são calculadas e arredondadas por venda, com duas casas decimais.
- A multa de 2,5% ao dia foi interpretada como juros simples: `valor x 2,5% x dias em atraso`.
- Vencimento no dia atual ou no futuro não gera juros.
- O estoque fica em memória durante a execução. O enunciado pede o saldo final, mas não pede gravação das mudanças no JSON.

## Pré-requisito

- .NET SDK 10

Confira a instalação com:

```bash
dotnet --version
```

## 1 Comissão por vendedor

O programa lê `src/comissao/vendas.json`, calcula a comissão de cada venda e exibe o total por vendedor.

Regras aplicadas:

- venda abaixo de R$ 100,00: sem comissão;
- venda de R$ 100,00 até R$ 499,99: 1%;
- venda a partir de R$ 500,00: 5%.

Execução:

```bash
dotnet run --project src/comissao/comissao.csproj
```

Também é possível indicar outro arquivo JSON:

```bash
dotnet run --project src/comissao/comissao.csproj -- caminho/para/vendas.json
```

## 2 Movimentação de estoque

O programa lê `src/estoque/estoque.json` e abre um menu para lançar entradas e saídas. Cada movimentação aceita produto, tipo, quantidade e descrição; quando é concluída, recebe um número sequencial único e mostra o estoque final.

Validações incluídas:

- produto existente;
- quantidade maior que zero;
- descrição obrigatória;
- bloqueio de saída sem saldo suficiente.

Execução interativa:

```bash
dotnet run --project src/estoque/estoque.csproj
```

Demonstração sem digitação:

```bash
dotnet run --project src/estoque/estoque.csproj -- --demo
```

## 3 Juros por atraso

O programa recebe um valor e uma data de vencimento. A data aceita os formatos `dd/MM/aaaa` e `aaaa-MM-dd`.

Execução interativa:

```bash
dotnet run --project src/juros/juros.csproj
```

Execução por argumentos:

```bash
dotnet run --project src/juros/juros.csproj -- 1000,00 25/09/2026
```

## Testes

Os testes não usam pacotes externos. Eles verificam os limites das faixas de comissão, as entradas e saídas de estoque, a proteção contra saldo insuficiente, a geração de identificadores únicos e o cálculo de juros.

```bash
dotnet run --project tests/DesafioTarget.Testes/DesafioTarget.Testes.csproj
```

## Executar tudo

No Windows PowerShell:

```powershell
.\run_all.ps1
```

No Linux ou macOS:

```bash
./run_all.sh
```

Os scripts executam a comissão, demonstrações não interativas dos outros dois exercícios e os testes.

## Estrutura

```text
src/
  comissao/  leitura do JSON e cálculo das comissões
  estoque/   menu e regras de movimentação
  juros/     entrada de dados e cálculo por atraso
tests/
  DesafioTarget.Testes/  testes automatizados das regras
```
