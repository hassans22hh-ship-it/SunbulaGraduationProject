<div align="center">

# 🌱 Sunbula (سُنبلة) — Modular Monolith Backend

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C# 13](https://img.shields.io/badge/C%23-13.0-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC292B?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server)
[![Testcontainers](https://img.shields.io/badge/Testcontainers-MsSql-008080?style=for-the-badge&logo=docker&logoColor=white)](https://dotnet.testcontainers.org/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-F5A623?style=for-the-badge)](LICENSE)

**A Modular Monolith backend for personal productivity, financial intelligence, and gamification built with .NET 10, exploring Domain-Driven Design (DDD), Clean Architecture, and isolated persistence boundaries.**

[Live Production Health Probe](https://sunbula.runasp.net/health) • [Frontend Client](https://github.com/hassans22hh-ship-it/SunbulaFrontEnd)

</div>

---

## 📑 Table of Contents

- [Project Overview](#-project-overview)
- [Architecture & Design Principles](#-architecture--design-principles)
- [Bounded Contexts (Modules)](#-bounded-contexts-modules)
- [Cross-Module Consistency & Event Choreography](#-cross-module-consistency--event-choreography)
- [Tech Stack](#-tech-stack)
- [Getting Started](#-getting-started)
  - [Option A: Docker & Docker Compose (Recommended)](#option-a-docker--docker-compose-recommended)
  - [Option B: Local .NET CLI](#option-b-local-net-cli)
- [Configuration & Startup Hygiene](#-configuration--startup-hygiene)
- [CI/CD & Deployment Strategy](#-cicd--deployment-strategy)
- [Database Migrations & Seeding](#-database-migrations--seeding)
- [Testing & Quality Assurance](#-testing--quality-assurance)
- [Architectural Trade-offs & Known Limitations](#-architectural-trade-offs--known-limitations)
- [Git Flow & Branching Strategy](#-git-flow--branching-strategy)
- [License](#-license)

---

## 📌 Project Overview

**Sunbula** is a personal life-management backend designed to unify fragmented daily workflows into a coherent domain model:

- **Productivity & Time Tracking:** Pomodoro timers, focus sessions, work categorization, and task prioritization.
- **Financial Intelligence:** Income and expense ledgers, multi-wallet cashflow tracking, categorized summaries, and creditor/debtor installment tracking.
- **Gamification Economy:** Earning in-game coins by logging focused work sessions and maintaining daily streaks, with an in-app nursery store to unlock, cultivate, and grow virtual plants.

---

## 🏛 Architecture & Design Principles

Sunbula is structured as a **Modular Monolith** — an architectural pattern combining single-unit deployment simplicity with independent domain modules, clean boundaries, and isolated persistence.

```mermaid
graph TB
    Client([Client Application / Angular]) --> Gateway[ASP.NET Core Web API Host]

    subgraph HOST["Sunbula Host (Composition Root)"]
        Gateway --> Auth[JWT Bearer & Query SSE Auth]
        Gateway --> CORS[CORS Middleware]
        Gateway --> Swagger[Swagger / OpenAPI]
        Gateway --> Health[Health Checks /health]
    end

    subgraph MODULES["Bounded Contexts (Clean Architecture)"]
        direction LR
        UI["UserIdentity<br/><i>Domain / App / Infra / Pres</i>"]
        TM["TaskManagement<br/><i>Domain / App / Infra / Pres</i>"]
        TT["TimeTracking<br/><i>Domain / App / Infra / Pres</i>"]
        FIN["Finance<br/><i>Domain / App / Infra / Pres</i>"]
        DBT["Debt<br/><i>Domain / App / Infra / Pres</i>"]
        PLT["StorePlant<br/><i>Domain / App / Infra / Pres</i>"]
    end

    subgraph SHARED["Shared Kernel (BuildingBlocks)"]
        SK["BaseEntity / ValueObject<br/>Domain Event Contracts<br/>Internal Service Contracts (IUserCoinService)"]
    end

    subgraph DB["SQL Server (Schema Isolation)"]
        direction LR
        D_UI[(identity Schema)]
        D_TM[(task Schema)]
        D_TT[(timetracking Schema)]
        D_FIN[(finance Schema)]
        D_DBT[(debt Schema)]
        D_PLT[(storeplant Schema)]
    end

    Gateway --> UI & TM & TT & FIN & DBT & PLT
    UI --> D_UI
    TM --> D_TM
    TT --> D_TT
    FIN --> D_FIN
    DBT --> D_DBT
    PLT --> D_PLT

    UI & TM & TT & FIN & DBT & PLT -. Contracts & Base Entities .-> SK
    UI -. UserAccountDeletedEvent (MediatR) .-> TM & TT & FIN & DBT & PLT
    PLT -. Synchronous Deduct / Refund .-> SK
```

### Core Tenets:
1. **Clean Architecture per Module:** Every module maintains autonomous `Domain`, `Application`, `Infrastructure`, and `Presentation` projects.
2. **Dedicated DbContexts:** Zero cross-context database foreign keys or LINQ joins. Each module owns its EF Core `DbContext` and schema namespace (`identity`, `storeplant`, etc.).
3. **Explicit Cross-Module Contracts:** Cross-module dependencies are mediated strictly via abstractions in `SharedKernel` or asynchronous domain events. Direct project dependencies between module domain/infrastructure layers are strictly prohibited.

---

## 🧩 Bounded Contexts (Modules)

```
Modules/{ModuleName}/
├── Domain/            # Aggregate Roots, Entities, Value Objects, Domain Events
├── Application/       # Service Abstractions, DTOs, Mappings, Event Handlers
├── Infrastructure/    # EF Core DbContext, Repositories, Migrations, External Integrations
└── Presentation/      # API Controllers (loaded dynamically into the Host)
```

| Module | Core Responsibility | Persistence Schema | Core Entities |
| :--- | :--- | :--- | :--- |
| **`UserIdentity`** | User credentials, password hashing, JWT generation, coin ledger | `identity` | `User`, `UserRefreshToken`, `UserSettings` |
| **`TaskManagement`** | Task hierarchies, deadlines, priority scoring, tags | `task` | `Task`, `Category`, `TaskColor` |
| **`TimeTracking`** | Focus session intervals, Pomodoro allocations, streak tracking | `timetracking` | `TimeSession`, `TimeSpanAllocation` |
| **`Finance`** | Income/expense cashflows, wallet management | `finance` | `Transaction`, `FinancialCategory`, `Wallet` |
| **`Debt`** | Debts, creditor/debtor directory, payment schedules | `debt` | `Debt`, `Installment` |
| **`StorePlant`** | Nursery catalog, plant purchase, growth and watering stages | `storeplant` | `Plant`, `UserPlant` |

---

## 📡 Cross-Module Consistency & Event Choreography

### Plant Purchase Flow (Compensating Transaction Pattern)

Because `UserIdentity` (coin balance) and `StorePlant` (plant ownership) reside in separate `DbContext` instances without two-phase commit (2PC), cross-module consistency is guaranteed using a **synchronous internal contract (`IUserCoinService`) with a compensating rollback (Saga pattern)**:

```mermaid
sequenceDiagram
    autonumber
    actor User
    participant StorePlant as UserPlantService (StorePlant)
    participant CoinService as IUserCoinService (UserIdentity)
    participant UserDB as UserIdentityDbContext
    participant PlantDB as StorePlantDbContext

    User->>StorePlant: PurchasePlantAsync(plantId)
    StorePlant->>StorePlant: Validate availability & non-duplicate ownership
    StorePlant->>CoinService: TryDeductCoinsAsync(userId, price)
    CoinService->>UserDB: Atomic deduction (ExecuteUpdateAsync)
    alt Insufficient Balance
        CoinService-->>StorePlant: false
        StorePlant-->>User: 400 Bad Request (InsufficientCoinsException)
    else Deduction Succeeded
        CoinService-->>StorePlant: true
        StorePlant->>PlantDB: BeginTransactionAsync()
        StorePlant->>PlantDB: Add(UserPlant) & SaveChangesAsync()
        alt Persistence Succeeded
            StorePlant->>PlantDB: CommitTransactionAsync()
            StorePlant-->>User: 200 OK (UserPlantDto)
        else Persistence Failed (Exception / Timeout)
            StorePlant->>PlantDB: RollbackTransactionAsync()
            StorePlant->>CoinService: RefundCoinsAsync(userId, price) [Compensating Action]
            CoinService->>UserDB: Restore CoinBalance (user.AddCoins)
            StorePlant-->>User: 500 Internal Error (Coins Restored)
        end
    end
```

### Event Choreography Table

| Event | Publisher | Subscribers | Delivery / Role |
| :--- | :--- | :--- | :--- |
| `UserAccountDeletedEvent` | **UserIdentity** | All Modules | In-Process MediatR: Cascades soft-delete / data cleanup across bounded contexts |
| `PlantPurchasedEvent` | **StorePlant** | **UserIdentity** | Domain event raised on purchase; serves as extension point for audit and achievements |
| `PlantWateredEvent` | **StorePlant** | **UserIdentity** | Domain event raised on plant watering; extension point for streak rewards |
| `CoinBalanceChangedEvent` | **UserIdentity** | **UserIdentity** | Broadcasts live coin changes to connected clients via Server-Sent Events (SSE) |

---

## 💻 Tech Stack

| Component | Technology | Version / Details |
| :--- | :--- | :--- |
| **Runtime & Language** | .NET | `10.0` (C# 13 default) |
| **Web Framework** | ASP.NET Core Web API | Kestrel Server, Minimal APIs + Dynamic Controllers |
| **ORM & Data Access** | Entity Framework Core | `10.0` (Code-First with Per-Module Migrations) |
| **Database Engine** | Microsoft SQL Server | `2022-latest` (Docker & Windows Server) |
| **Integration Testing** | Testcontainers.MsSql | `4.3.0` (Real containerized SQL Server fixtures) |
| **Test Runner & Assertions**| xUnit, FluentAssertions | `xUnit 2.9.3`, `FluentAssertions 8.9.0` |
| **Authentication** | JWT (JSON Web Tokens) | HMAC-SHA256, Refresh Token Rotation |
| **In-Process Messaging** | MediatR | `12.4.1` (`INotification` / `INotificationHandler`) |
| **Object Mapping** | AutoMapper | Assembly-scanned module profiles |
| **Containerization** | Docker & Docker Compose | Multi-stage production build |

---

## 🚀 Getting Started

### Option A: Docker & Docker Compose (Recommended)

Run the complete stack (API + SQL Server 2022) without installing local SDKs or database servers:

1. **Clone the repository:**
   ```bash
   git clone https://github.com/hassans22hh-ship-it/SunbulaGraduationProject.git
   cd SunbulaGraduationProject
   ```

2. **Configure environment variables:**
   ```bash
   cp .env.example .env
   ```

3. **Start the containers:**
   ```bash
   docker compose up -d --build
   ```

4. **Verify running containers:**
   ```bash
   docker compose ps
   ```
   - **API Host:** `http://localhost:5000`
   - **Health Check:** `http://localhost:5000/health`
   - **SQL Server:** `localhost:1433`

---

### Option B: Local .NET CLI

1. **Prerequisites:**
   - [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
   - Microsoft SQL Server 2022 (local or containerized)

2. **Setup configuration:**
   ```bash
   cp Sunbula/appsettings.Example.json Sunbula/appsettings.json
   ```

3. **Build the solution:**
   ```bash
   dotnet build SunbulaGraduationProject.slnx -c Release
   ```

4. **Run the API:**
   ```bash
   dotnet run --project Sunbula/Sunbula.csproj
   ```
   - Health Probe: `http://localhost:5142/health`

---

## 🔐 Configuration & Startup Hygiene

Configuration follows standard .NET hierarchical configuration providers (`appsettings.json` -> `appsettings.{Environment}.json` -> Environment Variables).

### Startup Hygiene & Security Gates:
- **Default Admin Seeding:** In `DatabaseInitializer.cs`, default admin seeding (`admin@sunbula.com` / `AdminPassword123!`) is strictly gated behind `env.IsDevelopment()`. Hardcoded credentials will never be seeded into Staging or Production databases.
- **Automated Migrations Flag:** Startup migrations are gated via configuration flag `"Database:AutoMigrate"`. Set to `false` in `appsettings.Production.json` to prevent race conditions during multi-instance deployment.

### Key Environment Mappings:

| Key | Environment Variable | Purpose |
| :--- | :--- | :--- |
| `Database:AutoMigrate` | `Database__AutoMigrate` | Enables/disables automatic startup migrations |
| `ConnectionStrings:SunbulaDb` | `ConnectionStrings__SunbulaDb` | SQL Server connection string |
| `Jwt:SecretKey` | `Jwt__SecretKey` | HMAC-SHA256 256-bit signing key |
| `Jwt:Issuer` | `Jwt__Issuer` | Valid token issuer(s) |
| `EmailSettings:Email` | `EmailSettings__Email` | SMTP sender address |
| `EmailSettings:Password` | `EmailSettings__Password` | SMTP application password |

---

## 🔄 CI/CD & Deployment Strategy

As a student graduation project, Sunbula uses a pragmatic cloud deployment strategy tailored for cost efficiency and ease of maintenance:

```mermaid
flowchart LR
    A[Git Push to Master] --> B[GitHub Actions Runner]
    B --> C[dotnet restore & build -c Release]
    C --> D[dotnet publish to staging artifact]
    D --> E[Upload app_offline.htm to MonsterASP]
    E --> F[FTP Sync Published Binaries to \\wwwroot]
    F --> G[Remove app_offline.htm]
    G --> H[IIS AppPool Recycles & Serves Traffic]
```

### Deployment Details:
- **Hosting Provider:** [MonsterASP.net](https://www.monsterasp.net/) (Windows / IIS Shared Hosting).
- **Deployment Mechanism:** Secure FTP synchronization via GitHub Actions (`.github/workflows/deploy.yml`).
- **File Locking Strategy:** An `app_offline.htm` file is uploaded to the application root before file transfer to instruct ASP.NET Core Module (ANCM) in IIS to release file locks on assemblies (`.dll`), then removed immediately after synchronization to trigger a clean application restart.

---

## 🗄 Database Migrations & Seeding

Each module manages its own migrations independently through separate assembly migrations:

```bash
# User Identity
dotnet ef database update --context UserIdentityDbContext --project Sunbula

# Task Management
dotnet ef database update --context TaskManagementDbContext --project Sunbula

# Time Tracking
dotnet ef database update --context TimeTrackingDbContext --project Sunbula

# Finance
dotnet ef database update --context FinanceDbContext --project Sunbula

# Debt
dotnet ef database update --context DebtDbContext --project Sunbula

# Store Plant
dotnet ef database update --context StorePlantDbContext --project Sunbula
```

---

## 🧪 Testing & Quality Assurance

Integration tests run against a real Microsoft SQL Server 2022 container via **`Testcontainers.MsSql`**, replacing the legacy `Microsoft.EntityFrameworkCore.InMemory` provider to ensure test fidelity across SQL transactions, foreign keys, schema isolation, and raw SQL operations.

```bash
# Ensure Docker daemon is accessible
DOCKER_HOST="unix:///var/run/docker.sock" dotnet test Sunbula.IntegrationTests/Sunbula.IntegrationTests.csproj -c Release
```

### Shared Fixture Architecture:
- `CustomWebApplicationFactory` implements `IAsyncLifetime` to start a single, shared SQL Server container across the test run via `[CollectionDefinition("IntegrationTests")]`.
- Replaces external outbound network dependencies (e.g. SMTP email sending) with `TestNoOpEmailService` to guarantee reliable offline test execution.
- Tests cover full authentication, registration, email confirmation, task priority filtering, debtor installment records, focus sessions, and plant purchases with coin deductions.

---

## ⚖️ Architectural Trade-offs & Known Limitations

Demonstrating engineering maturity requires acknowledging the architectural compromises made in this system:

1. **In-Process MediatR vs. Distributed Message Broker:**
   - *Current State:* Domain events are dispatched synchronously or in-process asynchronously using `MediatR`.
   - *Trade-off:* If the host process crashes mid-dispatch or scales to multiple instances, in-memory events are not queued or shared across nodes. Moving to a distributed broker (RabbitMQ, Azure Service Bus, or MassTransit) would be required for horizontally-scaled multi-node deployments.
2. **Dual DbContexts & Lack of Distributed Transactions (2PC):**
   - *Current State:* `StorePlant` and `UserIdentity` operate on separate `DbContext` connections. Cross-module operations (e.g. buying a plant with coins) use a synchronous check-and-deduct contract with a compensating rollback (Saga pattern) upon persistence failure.
   - *Trade-off:* While this prevents data loss without two-phase commit overhead, it introduces eventual consistency and requires careful maintenance of compensating actions.
3. **Shared Database Instance (Logical vs. Physical Separation):**
   - *Current State:* Modules maintain strict schema isolation (`identity`, `storeplant`, `finance`, etc.), but currently share a single physical SQL Server database instance to keep shared-hosting costs at zero.
   - *Trade-off:* Bounded contexts are logically isolated at the schema level, but compete for shared database CPU, I/O, and connection pools.
4. **FTP Deployment Downtime:**
   - *Current State:* Deployment to MonsterASP occurs via FTP with `app_offline.htm`.
   - *Trade-off:* The application experiences brief (30-60 second) maintenance downtime during release deployments, rather than zero-downtime blue/green or rolling deployments.
5. **Startup Migrations in Production:**
   - *Current State:* Migrations can be run on startup via `DatabaseInitializer.cs`.
   - *Trade-off:* While convenient for local development, automatic migration execution on startup is dangerous in multi-instance production environments. The `Database:AutoMigrate` flag must remain `false` in production, and migrations applied via CI/CD migration bundles.

---

## 🌿 Git Flow & Branching Strategy

- **`master`:** Production releases deployed to MonsterASP.
- **`develop`:** Integration branch for feature development.
- **`feature/{module}-{description}`:** Isolated feature branches.
- **`hotfix/{issue}`:** Critical patches branched from and merged into `master`.

---

## 📄 License

This project is licensed under the terms of the [MIT License](LICENSE).
