-- CreateBankDatabase.sql
DROP DATABASE IF EXISTS BankDB;
CREATE DATABASE BankDB;
USE BankDB;

-- Tạo bảng Users
CREATE TABLE Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100),
    Phone VARCHAR(20),
    Address TEXT,
    Permission INT DEFAULT 0,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Tạo bảng Clients
CREATE TABLE Clients (
    ClientID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100),
    Phone VARCHAR(20) NOT NULL,
    Address TEXT,
    DateOfBirth DATE,
    NationalID VARCHAR(20) UNIQUE,
    AccountNumber VARCHAR(20) UNIQUE NOT NULL,
    PinCode VARCHAR(10) NOT NULL,
    Balance DECIMAL(15,2) DEFAULT 0.00,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Tạo bảng LoginRegisters
CREATE TABLE LoginRegisters (
    LoginID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT,
    Username VARCHAR(50),
    LoginDateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    LogoutDateTime DATETIME NULL,
    IPAddress VARCHAR(45),
    IsSuccessful BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Tạo bảng TransferLogs
CREATE TABLE TransferLogs (
    TransferID INT AUTO_INCREMENT PRIMARY KEY,
    FromAccountNumber VARCHAR(20),
    ToAccountNumber VARCHAR(20),
    Amount DECIMAL(15,2) NOT NULL,
    TransferDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Description TEXT,
    TransferType ENUM('Transfer', 'Deposit', 'Withdraw') NOT NULL,
    ProcessedByUserID INT,
    FOREIGN KEY (ProcessedByUserID) REFERENCES Users(UserID)
);

-- SeedData.sql
USE BankDB;

-- Thêm dữ liệu Users (Nhân viên)
INSERT INTO Users (Username, Password, FirstName, LastName, Email, Phone, Address, Permission) VALUES
('admin', 'admin123', 'Nguyễn', 'Quản Trị', 'admin@bank.com', '0901234567', '123 Đường ABC, TP.HCM', -1),
('manager', 'manager123', 'Trần', 'Giám Đốc', 'manager@bank.com', '0902345678', '456 Đường DEF, TP.HCM', 1047579),
('cashier1', 'cashier123', 'Lê', 'Thu Ngân', 'cashier1@bank.com', '0903456789', '789 Đường GHI, TP.HCM', 11273),
('teller1', 'teller123', 'Phạm', 'Giao Dịch', 'teller1@bank.com', '0904567890', '321 Đường JKL, TP.HCM', 3081);

-- Thêm dữ liệu Clients (Khách hàng)
INSERT INTO Clients (FirstName, LastName, Email, Phone, Address, DateOfBirth, NationalID, AccountNumber, PinCode, Balance) VALUES
('Nguyễn', 'Văn An', 'nguyenvanan@email.com', '0911111111', '100 Lê Lợi, Q1, TP.HCM', '1990-05-15', '123456789', 'ACC001', '1234', 5000000.00),
('Trần', 'Thị Bình', 'tranthibinh@email.com', '0922222222', '200 Nguyễn Huệ, Q1, TP.HCM', '1985-08-20', '987654321', 'ACC002', '2345', 3500000.00),
('Lê', 'Hoàng Cường', 'lehoangcuong@email.com', '0933333333', '300 Đồng Khởi, Q1, TP.HCM', '1992-12-10', '456789123', 'ACC003', '3456', 7500000.00),
('Phạm', 'Thị Dung', 'phamthidung@email.com', '0944444444', '400 Hai Bà Trưng, Q3, TP.HCM', '1988-03-25', '789123456', 'ACC004', '4567', 2800000.00),
('Hoàng', 'Văn Em', 'hoangvanem@email.com', '0955555555', '500 Cách Mạng Tháng 8, Q10, TP.HCM', '1995-07-08', '321654987', 'ACC005', '5678', 4200000.00);

-- Thêm dữ liệu LoginRegisters (Lịch sử đăng nhập mẫu)
INSERT INTO LoginRegisters (UserID, Username, LoginDateTime, IPAddress) VALUES
(1, 'admin', '2025-06-20 08:00:00', '192.168.1.100'),
(2, 'manager', '2025-06-20 08:30:00', '192.168.1.101'),
(3, 'cashier1', '2025-06-20 09:00:00', '192.168.1.102'),
(4, 'teller1', '2025-06-20 09:15:00', '192.168.1.103');

-- Thêm dữ liệu TransferLogs (Giao dịch mẫu)
INSERT INTO TransferLogs (FromAccountNumber, ToAccountNumber, Amount, Description, TransferType, ProcessedByUserID) VALUES
('ACC001', 'ACC002', 500000.00, 'Chuyển tiền cho bạn', 'Transfer', 3),
('ACC003', 'ACC004', 1000000.00, 'Thanh toán hóa đơn', 'Transfer', 4),
(NULL, 'ACC001', 2000000.00, 'Nộp tiền mặt', 'Deposit', 3),
('ACC002', NULL, 300000.00, 'Rút tiền mặt', 'Withdraw', 4),
('ACC005', 'ACC001', 750000.00, 'Hoàn tiền', 'Transfer', 3);