-- SQL Server Database Creation Script for IPL Ticket Booking System
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ipl_ticket_booking')
BEGIN
    CREATE DATABASE ipl_ticket_booking;
END
GO

USE ipl_ticket_booking;
GO

IF OBJECT_ID('dbo.Payment', 'U') IS NOT NULL DROP TABLE dbo.Payment;
IF OBJECT_ID('dbo.BookingAddon', 'U') IS NOT NULL DROP TABLE dbo.BookingAddon;
IF OBJECT_ID('dbo.AddonMaster', 'U') IS NOT NULL DROP TABLE dbo.AddonMaster;
IF OBJECT_ID('dbo.Booking', 'U') IS NOT NULL DROP TABLE dbo.Booking;
IF OBJECT_ID('dbo.SeatDetails', 'U') IS NOT NULL DROP TABLE dbo.SeatDetails;
IF OBJECT_ID('dbo.StandDetails', 'U') IS NOT NULL DROP TABLE dbo.StandDetails;
IF OBJECT_ID('dbo.TicketType', 'U') IS NOT NULL DROP TABLE dbo.TicketType;
IF OBJECT_ID('dbo.MatchDetails', 'U') IS NOT NULL DROP TABLE dbo.MatchDetails;
IF OBJECT_ID('dbo.Customer', 'U') IS NOT NULL DROP TABLE dbo.Customer;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Mobile VARCHAR(10) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'User'
);
GO

CREATE TABLE Customer (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Users(UserID),
    CustomerName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Mobile VARCHAR(10) NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    City NVARCHAR(50) NOT NULL,
    State NVARCHAR(50) NOT NULL,
    Pincode VARCHAR(6) NOT NULL,
    IdType NVARCHAR(30) NOT NULL,
    IdNumber NVARCHAR(50) NOT NULL,
    FavoriteTeam NVARCHAR(60) NOT NULL
);
GO

CREATE TABLE MatchDetails (
    MatchID INT IDENTITY(1,1) PRIMARY KEY,
    MatchNumber INT NOT NULL,
    Team1 NVARCHAR(60) NOT NULL,
    Team2 NVARCHAR(60) NOT NULL,
    MatchDate DATE NOT NULL,
    MatchTime NVARCHAR(20) NOT NULL,
    Stadium NVARCHAR(100) NOT NULL,
    City NVARCHAR(50) NOT NULL
);
GO

CREATE TABLE StandDetails (
    StandID INT IDENTITY(1,1) PRIMARY KEY,
    StandName NVARCHAR(50) NOT NULL,
    Tier NVARCHAR(20) NOT NULL,
    BasePrice DECIMAL(10,2) NOT NULL,
    TotalCapacity INT NOT NULL
);
GO

CREATE TABLE TicketType (
    TicketTypeID INT IDENTITY(1,1) PRIMARY KEY,
    TicketName NVARCHAR(50) NOT NULL UNIQUE,
    PriceMultiplier DECIMAL(4,2) NOT NULL,
    Inclusions NVARCHAR(250) NOT NULL
);
GO

CREATE TABLE SeatDetails (
    SeatID INT IDENTITY(1,1) PRIMARY KEY,
    MatchID INT NOT NULL FOREIGN KEY REFERENCES MatchDetails(MatchID) ON DELETE CASCADE,
    StandID INT NOT NULL FOREIGN KEY REFERENCES StandDetails(StandID) ON DELETE CASCADE,
    TicketTypeID INT NOT NULL FOREIGN KEY REFERENCES TicketType(TicketTypeID) ON DELETE CASCADE,
    SeatNumber VARCHAR(20) NOT NULL,
    RowNumber VARCHAR(5) NOT NULL,
    FinalPrice DECIMAL(10,2) NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Available'
);
GO

CREATE TABLE AddonMaster (
    AddonID INT IDENTITY(1,1) PRIMARY KEY,
    AddonName NVARCHAR(100) NOT NULL,
    AddonCategory NVARCHAR(50) NOT NULL,
    Price DECIMAL(10,2) NOT NULL
);
GO

CREATE TABLE Booking (
    BookingID INT IDENTITY(1,1) PRIMARY KEY,
    BookingRef NVARCHAR(20) NOT NULL UNIQUE,
    CustomerID INT NOT NULL FOREIGN KEY REFERENCES Customer(CustomerID) ON DELETE CASCADE,
    MatchID INT NOT NULL FOREIGN KEY REFERENCES MatchDetails(MatchID),
    StandID INT NOT NULL FOREIGN KEY REFERENCES StandDetails(StandID),
    TicketTypeID INT NOT NULL FOREIGN KEY REFERENCES TicketType(TicketTypeID),
    SeatID INT NOT NULL FOREIGN KEY REFERENCES SeatDetails(SeatID),
    TotalAmount DECIMAL(10,2) NOT NULL,
    BookingDate DATETIME NOT NULL DEFAULT GETDATE(),
    BookingStatus NVARCHAR(20) NOT NULL DEFAULT 'Confirmed'
);
GO

CREATE TABLE BookingAddon (
    BookingAddonID INT IDENTITY(1,1) PRIMARY KEY,
    BookingID INT NOT NULL FOREIGN KEY REFERENCES Booking(BookingID) ON DELETE CASCADE,
    AddonID INT NOT NULL FOREIGN KEY REFERENCES AddonMaster(AddonID),
    Quantity INT NOT NULL DEFAULT 1,
    Price DECIMAL(10,2) NOT NULL
);
GO

CREATE TABLE Payment (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    BookingID INT NOT NULL FOREIGN KEY REFERENCES Booking(BookingID) ON DELETE CASCADE,
    PaymentRef NVARCHAR(30) NOT NULL UNIQUE,
    PaymentMode NVARCHAR(30) NOT NULL,
    CardOrUpiNumber NVARCHAR(50) NOT NULL,
    AmountPaid DECIMAL(10,2) NOT NULL,
    PaymentDate DATETIME NOT NULL DEFAULT GETDATE(),
    PaymentStatus NVARCHAR(20) NOT NULL DEFAULT 'Success'
);
GO

-- Seed Initial Data
INSERT INTO Users (Username, Password, FullName, Email, Mobile, Role) VALUES
('admin', 'password123', 'System Administrator', 'admin@ipl.com', '9876543210', 'Admin'),
('rahul', 'password123', 'Rahul Sharma', 'rahul.s@gmail.com', '9876543211', 'User');

INSERT INTO Customer (UserID, CustomerName, Email, Mobile, Address, City, State, Pincode, IdType, IdNumber, FavoriteTeam) VALUES
(1, 'System Administrator', 'admin@ipl.com', '9876543210', 'IPL HQ, Wankhede Stadium', 'Mumbai', 'Maharashtra', '400020', 'Aadhaar Card', '123456789012', 'Chennai Super Kings'),
(2, 'Rahul Sharma', 'rahul.s@gmail.com', '9876543211', '12 MG Road', 'Bengaluru', 'Karnataka', '560034', 'Aadhaar Card', '234567890123', 'Royal Challengers Bengaluru');

INSERT INTO MatchDetails (MatchNumber, Team1, Team2, MatchDate, MatchTime, Stadium, City) VALUES
(1, 'Chennai Super Kings', 'Mumbai Indians', '2026-04-01', '19:30', 'M. A. Chidambaram Stadium', 'Chennai'),
(2, 'Royal Challengers Bengaluru', 'Kolkata Knight Riders', '2026-04-02', '19:30', 'M. Chinnaswamy Stadium', 'Bengaluru'),
(3, 'Gujarat Titans', 'Delhi Capitals', '2026-04-03', '19:30', 'Narendra Modi Stadium', 'Ahmedabad');

INSERT INTO StandDetails (StandName, Tier, BasePrice, TotalCapacity) VALUES
('East Stand Lower', 'Tier 1', 1000.00, 200),
('West Stand Upper', 'Tier 2', 1800.00, 180),
('VIP Pavilion Stand', 'Tier 1', 3000.00, 150);

INSERT INTO TicketType (TicketName, PriceMultiplier, Inclusions) VALUES
('Standard Pass', 1.00, 'Match Access Only'),
('Gold Pass', 1.50, 'Match Access + F&B Coupon'),
('VIP Pavilion', 2.00, 'Lounge Access + AC Enclosure');

INSERT INTO AddonMaster (AddonName, AddonCategory, Price) VALUES
('Official Team Jersey', 'Merchandise', 999.00),
('Signed Match Cap', 'Merchandise', 499.00),
('VIP Buffet Meal Box', 'Food & Beverage', 650.00),
('Snack & Beverage Combo', 'Food & Beverage', 300.00);

-- Fully populated available seats for ALL matches, stands, and ticket types
INSERT INTO SeatDetails (MatchID, StandID, TicketTypeID, SeatNumber, RowNumber, FinalPrice, Status) VALUES
(1, 1, 1, 'S1-A01', 'A', 1000.00, 'Available'),
(1, 1, 1, 'S1-A02', 'A', 1000.00, 'Available'),
(1, 1, 1, 'S1-A03', 'A', 1000.00, 'Available'),
(1, 1, 2, 'S1-B01', 'B', 1500.00, 'Available'),
(1, 2, 2, 'S2-A01', 'A', 2700.00, 'Available'),
(1, 3, 3, 'S3-VIP1', 'VIP', 6000.00, 'Available'),
(2, 1, 1, 'S1-A01', 'A', 1000.00, 'Available'),
(2, 1, 1, 'S1-A02', 'A', 1000.00, 'Available'),
(2, 2, 2, 'S2-B01', 'B', 2700.00, 'Available'),
(2, 3, 3, 'S3-VIP2', 'VIP', 6000.00, 'Available'),
(3, 1, 1, 'S1-A01', 'A', 1000.00, 'Available'),
(3, 2, 1, 'S2-C05', 'C', 1800.00, 'Available'),
(3, 3, 3, 'S3-VIP3', 'VIP', 6000.00, 'Available');
GO
