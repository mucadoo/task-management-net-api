# task-management-api

API RESTful para gestão de tarefas desenvolvida seguindo os princípios de Clean Architecture e Domain-Driven Design.

## Arquitetura

O sistema é estruturado em quatro camadas (Domain, Application, Infrastructure, API) que garantem a separação de responsabilidades e alta testabilidade. A camada de Domínio contém as entidades e regras de negócio sem dependências externas, garantindo que o núcleo da aplicação seja independente. A camada Application orquestra os casos de uso, enquanto a Infrastructure gerencia a persistência de dados. A camada API expõe os recursos através de endpoints RESTful. Essa arquitetura facilita a manutenção e a evolução do projeto, isolando mudanças tecnológicas das regras de negócio.

## Tecnologias

- .NET 8
- ASP.NET Core
- Entity Framework Core InMemory
- FluentValidation
- Swashbuckle (Swagger)
- xUnit
- Moq

## Pré-requisitos

Apenas: .NET 8 SDK instalado

## Como rodar

```bash
git clone https://github.com/usuario/task-management-api.git
cd task-management-api
dotnet restore
dotnet run --project TaskManagement.API
```

O Swagger estará disponível em http://localhost:5055/swagger

## Testes

```bash
dotnet test
```

## Endpoints

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| GET | /api/tasks | Lista tarefas com filtros e paginação opcionais |
| GET | /api/tasks/{id} | Retorna uma tarefa pelo Id |
| POST | /api/tasks | Cria uma nova tarefa |
| PUT | /api/tasks/{id} | Atualiza uma tarefa existente |
| DELETE | /api/tasks/{id} | Remove uma tarefa |

## Parâmetros de listagem

| Parâmetro  | Tipo     | Descrição                                      |
|------------|----------|------------------------------------------------|
| status     | string?  | Filtra por status: Pending, InProgress, Completed |
| dueDate    | DateTime?| Filtra por data de vencimento (yyyy-MM-dd)     |
| pageNumber | int      | Número da página (default: 1, mínimo: 1)       |
| pageSize   | int      | Itens por página (default: 10, máximo: 100)    |

## Exemplos

Exemplo 1 — curl para POST criar uma tarefa:
```bash
curl -X POST http://localhost:5055/api/tasks \
-H "Content-Type: application/json" \
-d '{
  "title": "Revisar documentação",
  "description": "Verificar endpoints no Swagger",
  "dueDate": "2025-12-31",
  "status": "Pending"
}'
```

Exemplo 2 — curl GET listando tarefas pendentes com paginação:
```bash
curl "http://localhost:5055/api/tasks?status=Pending&pageNumber=1&pageSize=5"
```

Exemplo 3 — Formato de resposta paginada:
```json
{
  "items": [...],
  "totalCount": 42,
  "pageNumber": 1,
  "pageSize": 5,
  "totalPages": 9,
  "hasNextPage": true
}
```

## Validações

| Campo       | Regra                                              |
|-------------|-----------------------------------------------------|
| Title       | Obrigatório, máximo 200 caracteres, sem espaços brancos |
| Description | Máximo 2000 caracteres (quando informado)           |
| DueDate     | Não pode ser uma data no passado (quando informada) |
| Status      | Deve ser um valor válido do enum                    |

## Respostas de erro

A API retorna erros no formato ProblemDetails (RFC 7807) contendo os campos: type, title, status, detail, traceId.

Exemplo de erro 400:
```json
{
  "type": "",
  "title": "Validation Error",
  "status": 400,
  "detail": "Due date cannot be in the past.",
  "traceId": "00-abc123..."
}
```

## Decisões técnicas

1. EF Core InMemory — sem necessidade de banco externo para rodar ou testar.
2. FluentValidation — regras de negócio separadas do DTO, testáveis isoladamente.
3. Middleware de exceções — tratamento centralizado retornando ProblemDetails padronizado.
4. DTOs — desacoplam o contrato da API da entidade de domínio.
5. Paginação — evita retornar listas sem limite, padrão esperado em APIs de produção.
