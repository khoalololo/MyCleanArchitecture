# MyCleanArchitecture

A production-ready **Clean Architecture** template for .NET 10, utilizing **CQRS**, **MediatR**, **FluentValidation**, and **Entity Framework Core**.

---

## Architecture Overview

This project is built using the principles of Clean Architecture, ensuring the core business logic (Domain) is independent of external frameworks (Infrastructure/UI).

```mermaid
graph TD
    subgraph WebApi_Layer [WebApi / Presentation]
        TC[TransactionController]
        EH[Global Exception Handler]
    end

    subgraph Application_Layer [Application]
        direction TB
        M[IMediator]
        CTC[CreateTransactionCommand]
        GTQ[GetTransactionsQuery]
        VAL[Validation Pipeline Behavior]
        IDB[IApplicationDbContext Interface]
    end

    subgraph Domain_Layer [Domain]
        TE[Transaction Entity]
        CE[Category Entity]
    end

    subgraph Infrastructure_Layer [Infrastructure]
        ADB[AppDbContext Implementation]
        EF[Entity Framework Core]
    end

    %% Dependencies
    TC -- Sends Command/Query --> M
    M -- Dispatches to --> CTC
    M -- Dispatches to --> GTQ
    
    CTC -- Uses --> IDB
    GTQ -- Uses --> IDB
    CTC -- Uses --> TE
    
    VAL -- Intercepts & Validates --> CTC
    
    ADB -- Implements --> IDB
    ADB -- Uses --> EF

    %% Styling
    style Domain_Layer fill:#f9f,stroke:#333,stroke-width:2px
    style Application_Layer fill:#bbf,stroke:#333,stroke-width:2px
    style Infrastructure_Layer fill:#dfd,stroke:#333,stroke-width:2px
    style WebApi_Layer fill:#fdd,stroke:#333,stroke-width:2px
```

## Request Flow (POST /api/Transaction)

This project uses **MediatR** to decouple the Controllers from the business logic. Instead of calling a service directly, the Controller sends a "Command" or "Query" into a pipeline.

```mermaid
sequenceDiagram
    participant User
    participant TC as TransactionController
    participant Med as MediatR (Pipeline)
    participant Val as CreateTransactionCommandValidator
    participant Handler as CreateTransactionCommandHandler
    participant Domain as Transaction (Domain Entity)
    participant DB as IApplicationDbContext (Infrastructure)
    participant SQL as Database (PostgreSQL/SQL Server)

    User->>TC: POST /api/transaction { Name, Amount, CatId }
    
    TC->>Med: Send(CreateTransactionCommand)
    
    Note over Med,Val: MediatR Pipeline Starts
    
    Med->>Val: Validate(Command)
    alt is Invalid
        Val-->>TC: Validation Errors
        TC-->>User: 400 Bad Request
    else is Valid
        Val-->>Med: OK
        
        Med->>Handler: Handle(Command)
        
        Note over Handler,Domain: Domain Logic
        Handler->>Domain: Transaction.Create(name, amount, catId)
        Domain-->>Handler: New Transaction Object
        
        Note over Handler,DB: Persistence
        Handler->>DB: Add(transaction)
        Handler->>DB: SaveChangesAsync()
        DB->>SQL: INSERT INTO Transactions...
        SQL-->>DB: Success (Return ID)
        DB-->>Handler: OK (transaction.Id)
        
        Handler-->>Med: Return transaction.Id
        Med-->>TC: Return result
        
        TC-->>User: 201 Created (with ID)
    end

```
---

## Getting Started

### Prerequisites
-   **.NET 10 SDK**
-   **Docker Desktop** (for PostgreSQL)
-   **Git**

### Quick Start (with Docker)
1.  **Clone the Repo**:
    ```bash
    git clone https://github.com/<your-username>/MyCleanArchitecture.git
    cd MyCleanArchitecture
    ```
2.  **Spin up the Infrastructure**:
    ```bash
    docker compose up -d
    ```
3.  **Run the App**:
    ```bash
    dotnet run --project src/WebApi
    ```
4.  **Swagger UI**: Open `http://localhost:5130/swagger`

---

## How to Add a New Feature (e.g., "Category")

Clean Architecture uses **Vertical Slices**. To add a new feature:

1.  **Domain**: Add the entity in `src/Domain/Entities/Category.cs`.
2.  **Infrastructure**: Add `DbSet<Category>` to `AppDbContext.cs` and create a Migration.
3.  **Application**:
    -   Create a folder `src/Application/Categories/Commands`.
    -   Create `CreateCategoryCommand` and `CreateCategoryCommandHandler`.
    -   Create `CreateCategoryCommandValidator`.
4.  **Presentation**: Add a `CategoryController.cs` and use `IMediator` to send the command.

---

## Core Features implemented

-   **CQRS with MediatR**: Separation of Reads and Writes.
-   **Validation Pipeline**: Automated request validation before hitting the database.
-   **Global Exception Handling**: Converts exceptions into clean API responses (Problem Details).
-   **Rich Domain Model**: Logic stays inside Entities, not just "Anemic" DTOs.
-   **Infrastructure Agnostic**: The business layer doesn't know EF Core exists.
-   **Docker Healthchecks**: WebApi waits for the database to be fully healthy before starting.
-   **Auto-Migrations**: Database schema is updated automatically on application startup.

---

## Common Problems & Solutions

| Issue | Solution |
| :--- | :--- |
| **Database Connection Failed** | Ensure Docker is running and the connection string in `appsettings.json` or `compose.yaml` matches your local DB. |
| **Migrations not applying** | Run `dotnet ef database update --project src/Infrastructure --startup-project src/WebApi`. |
| **400 Bad Request on Post** | This is the **Validation Shield** at work. Check the response body for specific field errors. |
| **Changes not reflecting** | Run `dotnet build` to ensure all layers are recompiled. |

---

## License
This project is licensed under the MIT License. Feel free to use it for your own learning and projects!
