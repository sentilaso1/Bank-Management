-- CreateBankDatabase.sql
DROP DATABASE IF EXISTS BankDB;
CREATE DATABASE BankDB;
USE BankDB;

-- Tạo bảng Users
CREATE TABLE Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Account AccountNumber VARCHAR(20) UNIQUE,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100),
    Phone VARCHAR(20),
    Address TEXT,
    Role VARCHAR(20) NOT NULL,
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
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
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
    TransferDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FromAccountNumber VARCHAR(20),
    ToAccountNumber VARCHAR(20),
    Amount DECIMAL(15,2) NOT NULL,
    PerformedBy VARCHAR(50)
);

-- SeedData.sql
USE BankDB;

-- Thêm dữ liệu Users (Nhân viên và khách hàng)
INSERT INTO Users (UserID, Username, Password, FirstName, LastName, Email, Phone, Address, Role, Permission, IsActive, CreatedDate, AccountNumber) VALUES
(1, 'admin', 'admin123', 'Nguyễn', 'Quản Trị', 'admin@bank.com', '0901234567', '123 Đường ABC, TP.HCM', 'Administrator', 1, TRUE, '2025-07-27 21:59:00', NULL),
(2, 'manager', 'manager123', 'Trần', 'Giám Đốc', 'manager@bank.com', '0902345678', '456 Đường DEF, TP.HCM', 'Manager', 2, TRUE, '2025-07-27 21:59:00', NULL),
(3, 'auditor', 'auditor123', 'Đỗ', 'Kiểm Toán', 'auditor@bank.com', '0906666666', '789 Đường MNO, TP.HCM', 'Viewer', 4, TRUE, '2025-07-27 21:59:00', NULL),
(4, 'support', 'support123', 'Vũ', 'Hỗ Trợ', 'support@bank.com', '0907777777', '987 Đường PQR, TP.HCM', 'Viewer', 4, TRUE, '2025-07-27 21:59:00', NULL),
(5, 'ACC001', '1234', 'Nguyễn', 'Văn An', 'nguyenvanan@email.com', '0911111111', '100 Lê Lợi, Q1, TP.HCM', 'User', 3, TRUE, '2025-07-27 21:59:00', 'ACC001'),
(6, 'ACC002', '2345', 'Trần', 'Thị Bình', 'tranthibinh@email.com', '0922222222', '200 Nguyễn Huệ, Q1, TP.HCM', 'User', 3, TRUE, '2025-07-27 21:59:00', 'ACC002'),
(7, 'ACC003', '3456', 'Lê', 'Hoàng Cường', 'lehoangcuong@email.com', '0933333333', '300 Đồng Khởi, Q1, TP.HCM', 'User', 3, TRUE, '2025-07-27 21:59:00', 'ACC003'),
(8, 'ACC004', '4567', 'Phạm', 'Thị Dung', 'phamthidung@email.com', '0944444444', '400 Hai Bà Trưng, Q3, TP.HCM', 'User', 3, TRUE, '2025-07-27 21:59:00', 'ACC004'),
(9, 'ACC005', '5678', 'Hoàng', 'Văn Em', 'hoangvanem@email.com', '0955555555', '500 Cách Mạng Tháng 8, Q10, TP.HCM', 'User', 3, TRUE, '2025-07-27 21:59:00', 'ACC005'),
(10, 'ACC006', '6789', 'Ngô', 'Thanh Phong', 'ngothanhphong@email.com', '0966666666', '600 Lạc Long Quân, Q11, TP.HCM', 'User', 3, TRUE, '2025-07-27 21:59:00', 'ACC006'),
(11, 'ACC007', '7890', 'Đặng', 'Thúy Hà', 'dangthuyha@email.com', '0977777777', '700 Pasteur, Q3, TP.HCM', 'User', 3, TRUE, '2025-07-27 21:59:00', 'ACC007');

-- Thêm dữ liệu Clients (Khách hàng)
INSERT INTO Clients (FirstName, LastName, Email, Phone, Address, DateOfBirth, NationalID, AccountNumber, PinCode, Balance) VALUES
('Nguyễn', 'Văn An', 'nguyenvanan@email.com', '0911111111', '100 Lê Lợi, Q1, TP.HCM', '1990-05-15', '123456789', 'ACC001', '1234', 5000000.00),
('Trần', 'Thị Bình', 'tranthibinh@email.com', '0922222222', '200 Nguyễn Huệ, Q1, TP.HCM', '1985-08-20', '987654321', 'ACC002', '2345', 3500000.00),
('Lê', 'Hoàng Cường', 'lehoangcuong@email.com', '0933333333', '300 Đồng Khởi, Q1, TP.HCM', '1992-12-10', '456789123', 'ACC003', '3456', 7500000.00),
('Phạm', 'Thị Dung', 'phamthidung@email.com', '0944444444', '400 Hai Bà Trưng, Q3, TP.HCM', '1988-03-25', '789123456', 'ACC004', '4567', 2800000.00),
('Hoàng', 'Văn Em', 'hoangvanem@email.com', '0955555555', '500 Cách Mạng Tháng 8, Q10, TP.HCM', '1995-07-08', '321654987', 'ACC005', '5678', 4200000.00),
('Ngô', 'Thanh Phong', 'ngothanhphong@email.com', '0966666666', '600 Lạc Long Quân, Q11, TP.HCM', '1991-09-15', '852741963', 'ACC006', '6789', 6300000.00),
('Đặng', 'Thúy Hà', 'dangthuyha@email.com', '0977777777', '700 Pasteur, Q3, TP.HCM', '1987-11-20', '963852741', 'ACC007', '7890', 8100000.00),

-- Thêm dữ liệu LoginRegisters (Lịch sử đăng nhập mẫu)
INSERT INTO LoginRegisters (UserID, Username, LoginDateTime, IPAddress) VALUES
(1, 'admin', '2025-06-20 08:00:00', '192.168.1.100'),
(2, 'manager', '2025-06-20 08:30:00', '192.168.1.101'),
(3, 'cashier1', '2025-06-20 09:00:00', '192.168.1.102'),
(4, 'teller1', '2025-06-20 09:15:00', '192.168.1.103');

-- Thêm dữ liệu TransferLogs (Giao dịch mẫu)
INSERT INTO TransferLogs (TransferDate, FromAccountNumber, ToAccountNumber, Amount, PerformedBy) VALUES
('2025-06-20 10:00:00', 'ACC001', 'ACC002', 500000.00, 'ACC001'),
('2025-06-20 10:30:00', 'ACC003', 'ACC004', 1000000.00, 'ACC003'),
('2025-06-20 11:00:00', NULL, 'ACC001', 2000000.00, 'ACC001'),
('2025-06-20 11:30:00', 'ACC002', NULL, 300000.00, 'ACC002'),
('2025-06-20 12:00:00', 'ACC005', 'ACC001', 750000.00, 'ACC005'),
('2025-06-22 14:00:00', 'ACC005', 'ACC006', 150000.00, 'ACC005'),
('2025-06-22 15:30:00', 'ACC006', NULL, 50000.00, 'ACC006');
