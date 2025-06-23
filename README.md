# Report 1: Project Proposal Report (Initiation Phase)

## 1. Project Title

**Bank Management System**

## 2. Objectives and Expected Outcomes

The repository contains a multi-tier desktop application aimed at managing bank operations. The main objectives are:

- Provide a WPF interface for employees to manage clients, user accounts and financial transactions.
- Implement role-based authentication and authorization with granular permissions.
- Record audit logs for security and traceability.

Expected deliverables include:

- A working WPF application that supports login, user management, client management and transaction processing.
- A MySQL database with tables for users, clients, login history and transfer logs, seeded with sample data.
- Business and data access layers encapsulating core logic and database operations.

## 3. Scope and Technical Requirements

**In Scope**

- CRUD operations for clients and users.
- Deposit, withdraw and transfer transactions with balance tracking.
- Role-based permission checks using `PermissionService` and audit logging.
- Viewing login history and transfer logs.
- Local MySQL database integration via `MySql.Data`.

**Out of Scope**

- Internet-facing services or web UI.
- Advanced banking features such as real-time fraud detection.

**Technical Requirements**

- WPF front end targeting `.NET 8.0-windows` (`BankManagementWPF.csproj`).
- Business and Data Access layers targeting .NET Framework 4.8.
- MySQL connection settings provided through environment variables `BANK_DB_SERVER`, `BANK_DB_PORT`, `BANK_DB_NAME`, `BANK_DB_USER` and `BANK_DB_PASSWORD` as defined in `clsDataAccessSettings.cs`.
- Database schema and seed data provided in `Database/CreateTables.sql`.

The difference between .NET versions may require upgrading the libraries to `.NET 8` for compatibility.

## 4. Initial Implementation Plan

1. **Environment Setup** – Configure MySQL using `CreateTables.sql` and set the required environment variables. Prepare development tools (Visual Studio, .NET SDK).
2. **Data Access Layer** – Ensure CRUD and transaction methods (e.g., `Deposit`, `Withdraw`, `Transfer`) are stable and unit tested.
3. **Business Layer** – Implement business models (`User`, `Client`, `TransferLogs`) and integrate role mapping.
4. **User Interface** – Build WPF views for login, users, clients and transactions. Hook them to the business layer.
5. **Security & Permissions** – Configure `PermissionService`, role-based access and audit logs.
6. **Testing & Refinement** – Functional testing and user acceptance testing.
7. **Documentation & Deployment** – Prepare user guide and finalize deliverables.

### Preliminary Timeline

| Phase | Duration |
|-------|---------|
|Environment setup|1 week|
|Data layer & business logic|2 weeks|
|UI implementation|3 weeks|
|Security & permissions|1 week|
|Testing and fixes|1 week|
|Documentation & wrap-up|1 week|

Total estimated time: **9 weeks**.

## 5. Resources and Tools

- **Languages/Frameworks**: C#, WPF, .NET 8, .NET Framework 4.8.
- **Database**: MySQL (local or hosted) using `MySql.Data` connector.
- **IDE/Tools**: Visual Studio 2022 or later, Git for version control.
- **Additional Libraries**: packages listed in `packages.config` such as `BouncyCastle`, `Google.Protobuf`, and `MySql.Data`.
- **Repository Setup**: existing Git repository with solution and SQL scripts. No CI/CD pipeline defined yet; can add GitHub Actions for automated builds.

## 6. Risk Assessment

- **Framework Compatibility** – Mixing .NET 8 and .NET Framework 4.8 could lead to build issues. Plan to migrate all projects to the same target framework.
- **Database Connectivity** – Incorrect environment variables or missing MySQL server will block the application. Provide setup guides and connection checks.
- **Security** – Credentials stored in environment variables must be protected. User permissions should be thoroughly tested to avoid privilege escalation.
- **Data Integrity** – Transaction logic must handle failures to prevent inconsistent balances.

Mitigation includes regular backups, thorough testing and code reviews.

## 7. References

- [.NET Documentation](https://learn.microsoft.com/dotnet/)
- [MySQL Documentation](https://dev.mysql.com/doc/)
- [WPF Documentation](https://learn.microsoft.com/dotnet/desktop/wpf/)
