# GameResults

### 🎲  Pré-requisitos - Rodando o BackEnd (servidor)

```
Antes de começar você vai precisar criar um banco de dados chamado Desafio e rodar o comando ADD-MIGRATION
Logo após executar esse comando para criar a tabela de TestCache
dotnet sql-cache create "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Desafio;Integrated Security=True;" dbo TestCache
```

### 🎲  Configurações (servidor)

```
No Appsettings.json:

# CacheTimeout
Configurações de tempo do cache. (Segundos)

# CheckUpdateTime
Configuração de tempo do Worker Service. (Milissegundos)
```
