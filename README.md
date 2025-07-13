# Bank Management System (WPF)

A comprehensive desktop application for managing bank accounts, users, and transactions, built with WPF (.NET 8) and a layered architecture. This project is designed for educational and practical use, demonstrating modern software engineering principles in C# and .NET.

---

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Database Setup](#database-setup)
- [Usage Guide](#usage-guide)
- [Project Structure](#project-structure)
- [Technologies Used](#technologies-used)
- [Contributing](#contributing)
- [License](#license)

---

## Features

- **User Authentication & Registration**
  - Secure login with password and PIN code.
  - Registration with unique account number generation.
  - Account lockout after multiple failed login attempts.

- **Client Management**
  - Add, update, delete, and search clients.
  - View client details and balances.
  - Role-based access for users and administrators.

- **Account Operations**
  - Deposit: Add funds to an account with PIN verification.
  - Withdraw: Remove funds with balance validation and warnings.
  - Transfer: Move funds between accounts, including fee calculation and validation.

- **User Management**
  - List all users with filtering by role and status.
  - Bulk lock/unlock users.
  - Export user list to CSV.
  - Analytics (placeholder for future development).

- **Transaction Logging**
  - All deposits, withdrawals, and transfers are logged.
  - View transaction history for auditing.

- **Responsive WPF UI**
  - Modern, user-friendly interface.
  - Real-time feedback and validation.
  - MVVM-ready codebase for future extensibility.

---

## Architecture

This solution uses a layered architecture for maintainability and scalability:

- **BankManagementWPF**: Presentation layer (WPF, .NET 8)
- **BankBusinessLayer**: Business logic and domain models (.NET Framework 4.8)
- **BankDataAccessLayer**: Data access, persistence, and database operations (.NET Framework 4.8)

Communication between layers is handled via project references and shared models.

---

## Getting Started

### Prerequisites

- Visual Studio 2022 or newer
- .NET 8 SDK
- .NET Framework 4.8 Developer Pack
- MySQL Server (for database)

### Build & Run

1. **Clone the repository:**git clone https://github.com/yourusername/BankManagementWPF.git**
2. **Open the solution in Visual Studio.**
3. **Restore NuGet packages** (__Project > Restore NuGet Packages__).
4. **Configure the database connection string** in `BankDataAccessLayer/clsDataAccessSettings.cs`.
5. **Set `BankManagementWPF` as the startup project.**
6. **Build and run** (__F5__).

---

## Database Setup

- The application uses MySQL for data storage.
- Create a database and run the provided SQL scripts (see `/DatabaseScripts/` if available) to set up tables for `Users`, `Clients`, and `TransferLogs`.
- Update the connection string in `clsDataAccessSettings.cs` to match your MySQL server settings.

---

## Usage Guide

### Registration

- New users can register by providing personal details, username, password, and PIN code.
- The system generates a unique account number for each client.

### Login

- Users log in with their username and password.
- After three failed attempts, the account is locked for security.

### Deposit

- Enter the deposit amount and PIN code.
- The system validates the PIN and updates the balance.
- All deposits are logged.

### Withdraw

- Enter the withdrawal amount.
- The system checks for sufficient balance and warns if the balance is low.
- All withdrawals are logged.

### Transfer

- Specify source and destination account numbers.
- Enter the transfer amount; the system calculates and displays the fee.
- Transfers are validated and logged.

### User Management (Admin)

- View all users, filter by role/status, and search.
- Lock or unlock multiple users at once.
- Export user data to CSV for reporting.

---

## Project Structure
<pre>
BankManagementWPF/
├── Views/                 # Chứa các UserControls và Windows của WPF (giao diện người dùng)
├── Models/                # Chứa các ViewModels và DTOs (Data Transfer Objects)
├── BankBusinessLayer/     # Lớp nghiệp vụ, xử lý logic chính của ứng dụng
├── BankDataAccessLayer/   # Lớp truy cập dữ liệu, kết nối với cơ sở dữ liệu
├── Resources/             # Tài nguyên như hình ảnh, biểu tượng,...
├── DatabaseScripts/       # Các script SQL dùng để khởi tạo hoặc cập nhật cơ sở dữ liệu
└── README.md              # Tài liệu mô tả dự án
</pre>
---

## Technologies Used

- **C# 12.0**
- **WPF (.NET 8)**
- **.NET Framework 4.8** (Business/Data layers)
- **MySQL** (Database)
- **MVVM Pattern** (recommended for future development)
- **LINQ, DataTable, Async/Await**

---

## Contributing

Contributions are welcome! Please fork the repository, create a feature branch, and submit a pull request. For major changes, open an issue first to discuss your ideas.

---

## License

This project is licensed under the MIT License.

---

*For questions, issues, or feature requests, please open an issue or contact the maintainer.*
