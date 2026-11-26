-- ============================================================================
-- Member Management & Document Database System
-- Database Initialization Script for SQL Server (Local / SQLEXPRESS)
-- ============================================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'MemberManagementDb')
BEGIN
    CREATE DATABASE [MemberManagementDb];
    PRINT 'Database MemberManagementDb created successfully.';
END
ELSE
BEGIN
    PRINT 'Database MemberManagementDb already exists.';
END
GO

USE [MemberManagementDb];
GO

-- 1. Users Table (Authentication & Role-based Access)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Users')
BEGIN
    CREATE TABLE [dbo].[Users] (
        [UserId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Username] NVARCHAR(50) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(255) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(100) NULL,
        [Role] NVARCHAR(50) NOT NULL DEFAULT 'Viewer', -- Admin, DataEntry, Finance, Viewer
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Users table created.';
END
GO

-- 2. Members Table (সদস্য তথ্য)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Members')
BEGIN
    CREATE TABLE [dbo].[Members] (
        [MemberId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MemberCode] NVARCHAR(20) NOT NULL UNIQUE,
        [FullName] NVARCHAR(150) NOT NULL,
        [FatherName] NVARCHAR(150) NULL,
        [MotherName] NVARCHAR(150) NULL,
        [DateOfBirth] DATE NULL,
        [NationalId] NVARCHAR(50) NULL,
        [MobileNumber] NVARCHAR(30) NOT NULL,
        [Email] NVARCHAR(100) NULL,
        [Address] NVARCHAR(300) NULL,
        [District] NVARCHAR(50) NULL,
        [Division] NVARCHAR(50) NULL,
        [PhotoPath] NVARCHAR(255) NULL,
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'Active', -- Active / Inactive
        [JoiningDate] DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [Remarks] NVARCHAR(500) NULL
    );

    CREATE NONCLUSTERED INDEX [IX_Members_MobileNumber] ON [dbo].[Members]([MobileNumber]);
    CREATE NONCLUSTERED INDEX [IX_Members_FullName] ON [dbo].[Members]([FullName]);
    CREATE NONCLUSTERED INDEX [IX_Members_District] ON [dbo].[Members]([District]);
    CREATE NONCLUSTERED INDEX [IX_Members_Status] ON [dbo].[Members]([Status]);

    PRINT 'Members table created with indexes.';
END
GO

-- 3. Documents Table (ডকুমেন্ট আপলোড)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Documents')
BEGIN
    CREATE TABLE [dbo].[Documents] (
        [DocumentId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MemberId] INT NOT NULL,
        [DocumentType] NVARCHAR(100) NOT NULL, -- National ID, Passport, Birth Certificate, Educational Certificate, Photo, Other
        [FileName] NVARCHAR(255) NOT NULL,
        [FilePath] NVARCHAR(300) NOT NULL,
        [FileSize] BIGINT NULL,
        [FileExtension] NVARCHAR(20) NULL,
        [UploadDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [Remarks] NVARCHAR(500) NULL,
        CONSTRAINT [FK_Documents_Members] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Members]([MemberId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_Documents_MemberId] ON [dbo].[Documents]([MemberId]);
    PRINT 'Documents table created.';
END
GO

-- 4. Payments Table (পেমেন্ট/ফি)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Payments')
BEGIN
    CREATE TABLE [dbo].[Payments] (
        [PaymentId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MemberId] INT NOT NULL,
        [InvoiceNo] NVARCHAR(50) NOT NULL UNIQUE,
        [FeeType] NVARCHAR(100) NOT NULL, -- Registration Fee, Monthly Fee, Annual Fee, Renewal Fee, Donation, Other
        [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0.00,
        [PaymentDate] DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
        [PaymentMethod] NVARCHAR(50) NOT NULL DEFAULT 'Cash', -- Cash, bKash, Nagad, Bank Transfer, Cheque
        [TransactionId] NVARCHAR(100) NULL,
        [ReceiptPath] NVARCHAR(300) NULL,
        [Remarks] NVARCHAR(500) NULL,
        [ReceivedBy] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_Payments_Members] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Members]([MemberId]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_Payments_MemberId] ON [dbo].[Payments]([MemberId]);
    CREATE NONCLUSTERED INDEX [IX_Payments_PaymentDate] ON [dbo].[Payments]([PaymentDate]);
    PRINT 'Payments table created.';
END
GO

-- 5. Seed Initial Data
-- Insert Default Users (Password hashes are SHA256 of: admin123, entry123, finance123, viewer123)
-- SHA256("admin123") = 240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9
-- SHA256("entry123") = b250db04b901b0b5d03bb6359f4fdbceae2cb335beaa7d4e5185c1387d8a6ff6
-- SHA256("finance123") = d376a20bc51f5e8b61c1626ddbdbe2b5919e1c272714a84092b7bc6ab89d36e8
-- SHA256("viewer123") = 8d2c448d2fcab101cf2894ea9c811f0a216e2518e38d77d1ca476a6e50e82845
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'admin')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [PasswordHash], [FullName], [Email], [Role], [IsActive])
    VALUES 
    ('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'System Administrator', 'admin@organization.org', 'Admin', 1),
    ('dataentry', 'b250db04b901b0b5d03bb6359f4fdbceae2cb335beaa7d4e5185c1387d8a6ff6', 'Data Entry Officer', 'entry@organization.org', 'DataEntry', 1),
    ('finance', 'd376a20bc51f5e8b61c1626ddbdbe2b5919e1c272714a84092b7bc6ab89d36e8', 'Accounts & Finance Manager', 'finance@organization.org', 'Finance', 1),
    ('viewer', '8d2c448d2fcab101cf2894ea9c811f0a216e2518e38d77d1ca476a6e50e82845', 'Guest Viewer', 'viewer@organization.org', 'Viewer', 1);
    PRINT 'Default users inserted.';
END
GO

-- Seed Sample Members if table is empty
IF NOT EXISTS (SELECT 1 FROM [dbo].[Members])
BEGIN
    INSERT INTO [dbo].[Members] 
    ([MemberCode], [FullName], [FatherName], [MotherName], [DateOfBirth], [NationalId], [MobileNumber], [Email], [Address], [District], [Division], [PhotoPath], [Status], [JoiningDate], [Remarks])
    VALUES
    ('MEM-1001', N'মোহাম্মদ রফিকুল ইসলাম', N'মোঃ নুরুল ইসলাম', N'মোছাঃ রাবেয়া খাতুন', '1988-04-12', '19881234567890123', '01711223344', 'rafiqul@gmail.com', N'বাড়ি ১২, রোড ৪, ধানমন্ডি', N'Dhaka', N'Dhaka', NULL, 'Active', '2025-01-15', N'প্রতিষ্ঠাতা আজীবন সদস্য'),
    ('MEM-1002', N'তানজিলা আক্তার', N'আব্দুল করিম', N'মমতাজ বেগম', '1992-08-23', '19929876543210987', '01819876543', 'tanzila@gmail.com', N'গ্রাম: বরুড়া, থানা: কোতোয়ালী', N'Cumilla', N'Chittagong', NULL, 'Active', '2025-02-10', N'নিয়মিত সদস্য'),
    ('MEM-1003', N'কামরুল হাসান সজীব', N'সুলতান আহমেদ', N'ফরিদা পারভীন', '1995-11-05', '19954567891234567', '01912345678', 'kamrul@gmail.com', N'হোল্ডিং নং ৪৫, জেল রোড', N'Sylhet', N'Sylhet', NULL, 'Active', '2025-03-01', N'কার্যনির্বাহী সদস্য'),
    ('MEM-1004', N'ফারহানা রহমান স্মৃতি', N'মাহবুবুর রহমান', N'নাসিমা বেগম', '1990-06-18', '19901122334455667', '01611335577', 'farhana@yahoo.com', N'সেক্টর ৩, উত্তরা মডেল টাউন', N'Dhaka', N'Dhaka', NULL, 'Active', '2025-03-12', N'সাধারণ সদস্য'),
    ('MEM-1005', N'আব্দুল্লাহ আল মামুন', N'শামসুল হক', N'আয়েশা খাতুন', '1985-01-30', '19856677889900112', '01715566778', 'mamun@gmail.com', N'মাস্টারপাড়া, জিরো পয়েন্ট', N'Rajshahi', N'Rajshahi', NULL, 'Inactive', '2024-11-20', N'ঠিকানা পরিবর্তন সংক্রান্ত কারণে স্থগিত'),
    ('MEM-1006', N'শারমিন সুলতানা', N'রেজাউল করিম', N'জাহানারা বেগম', '1993-09-14', '19932233445566778', '01519988776', 'sharmin@gmail.com', N'বয়রা মেইন রোড', N'Khulna', N'Khulna', NULL, 'Active', '2025-04-05', N'নিয়মিত সদস্য');
    PRINT 'Sample members inserted.';
END
GO

-- Seed Sample Payments if table is empty
IF NOT EXISTS (SELECT 1 FROM [dbo].[Payments])
BEGIN
    INSERT INTO [dbo].[Payments] 
    ([MemberId], [InvoiceNo], [FeeType], [Amount], [PaymentDate], [PaymentMethod], [TransactionId], [ReceiptPath], [Remarks], [ReceivedBy])
    VALUES
    (1, 'INV-202501-1001', N'Registration Fee', 5000.00, '2025-01-15', 'bKash', 'BK987X123', NULL, N'রেজিস্ট্রেশন ফি পরিশোধিত', 'admin'),
    (1, 'INV-202502-1002', N'Annual Fee', 2000.00, '2025-02-01', 'Bank Transfer', 'TXN554433', NULL, N'বার্ষিক নবায়ন ফি', 'finance'),
    (2, 'INV-202502-1003', N'Registration Fee', 5000.00, '2025-02-10', 'Cash', NULL, NULL, N'নগদ ক্যাশে জমা', 'finance'),
    (3, 'INV-202503-1004', N'Registration Fee', 5000.00, '2025-03-01', 'Nagad', 'NGD776655', NULL, N'নগদ অ্যাকাউন্টে পরিশোধ', 'admin'),
    (4, 'INV-202503-1005', N'Registration Fee', 5000.00, '2025-03-12', 'bKash', 'BK3344556', NULL, N'রেজিস্ট্রেশন ফি পরিশোধিত', 'dataentry'),
    (6, 'INV-202504-1006', N'Registration Fee', 5000.00, '2025-04-05', 'Cash', NULL, NULL, N'ক্যাশ রসিদ নং ১২৪', 'finance');
    PRINT 'Sample payments inserted.';
END
GO

-- Seed Sample Documents if table is empty
IF NOT EXISTS (SELECT 1 FROM [dbo].[Documents])
BEGIN
    INSERT INTO [dbo].[Documents]
    ([MemberId], [DocumentType], [FileName], [FilePath], [FileSize], [FileExtension], [UploadDate], [Remarks])
    VALUES
    (1, N'National ID (NID)', 'rafiqul_nid.pdf', '/uploads/documents/sample_nid.pdf', 245120, '.pdf', '2025-01-15', N'জাতীয় পরিচয়পত্রের সত্যায়িত ফটোকপি'),
    (2, N'Passport', 'tanzila_passport.pdf', '/uploads/documents/sample_passport.pdf', 312000, '.pdf', '2025-02-10', N'পাসপোর্টের প্রথম দুই পাতার কপি'),
    (3, N'National ID (NID)', 'kamrul_nid.jpg', '/uploads/documents/sample_nid.jpg', 185400, '.jpg', '2025-03-01', N'এনআইডি কার্ড স্ক্যান কপি'),
    (4, N'Educational Certificate', 'farhana_certificate.pdf', '/uploads/documents/sample_cert.pdf', 450200, '.pdf', '2025-03-12', N'স্নাতকোত্তর সনদের কপি');
    PRINT 'Sample documents inserted.';
END
GO

PRINT '================================================================';
PRINT 'MemberManagementDb database setup completed successfully!';
PRINT '================================================================';
GO

