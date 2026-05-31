# task-management-api

API RESTful para gestão de tarefas desenvolvida seguindo os princípios da Clean Architecture e Domain-Driven Design.

## Arquitetura

O sistema é estruturado em quatro camadas principais seguindo os princípios da Clean Architecture para garantir a independência de frameworks e a testabilidade. A camada de Domínio contém o núcleo das regras de negócio e não possui dependências externas o que assegura que as políticas da aplicação sejam preservadas. A camada Application orquestra os fluxos de dados enquanto a Infrastructure lida com a persistência e a API expõe os endpoints para consumo externo. Essa separação clara de responsabilidades permite que cada componente seja modificado ou substituído sem afetar as demais partes do software de forma imprevisível.

## Tecnologias

- .NET 8
- ASP.NET Core
- EF Core InMemory
- Swashbuckle
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

O Swagger estará disponível em http://localhost:5000/swagger

## Testes

```bash
dotnet test
```

## Endpoints

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| GET | /api/tasks | Lista tarefas com filtros opcionais de status e data |
| GET | /api/tasks/{id} | Obtém os detalhes de uma tarefa específica por ID |
| POST | /api/tasks | Cria uma nova tarefa |
| PUT | /api/tasks/{id} | Atualiza os dados de uma tarefa existente |
| DELETE | /api/tasks/{id} | Remove uma tarefa do sistema |

## Exemplos

Criar uma tarefa:
```bash
curl -X POST http://localhost:5000/api/tasks \
-H "Content-Type: application/json" \
-d '{"title": "Implementar testes", "status": "Pending"}'
```

Listar tarefas pendentes:
```bash
curl "http://localhost:5000/api/tasks?status=Pending"
```

## Decisões técnicas

- Utilização do EF Core InMemory para simplificar a execução do protótipo sem necessidade de banco de dados externo.
- Implementação de um middleware de exceções global para padronizar respostas de erro e evitar repetição de blocos try/catch.
- Uso de DTOs para separar o modelo de persistência das interfaces de entrada e saída da API.
- Configuração de enums como string no JSON para melhorar a legibilidade das respostas e facilitar o consumo.
