-- =============================================
-- SIMS - Student Information Management System
-- File: Database/Setup.sql
-- Mô tả: Tạo database sạch + đầy đủ bảng Identity cho AppUser<AppRole<int>>
-- Cách dùng: Mở SSMS → chạy toàn bộ file này 1 lần khi clone dự án
-- =============================================

PRINT 'Bắt đầu tạo database SIMS_DB...'

-- Xóa DB cũ nếu tồn tại (để đảm bảo sạch 100%)
IF DB_ID('SIMS_DB') IS NOT NULL
BEGIN
    ALTER DATABASE SIMS_DB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SIMS_DB;
    PRINT 'Đã xóa database cũ.'
END
GO

-- Tạo database mới
CREATE DATABASE SIMS_DB;
GO

USE SIMS_DB;
GO

PRINT 'Đang tạo các bảng ASP.NET Core Identity (chuẩn cho AppUser + AppRole<int>)...'

-- 1. AspNetUsers (đầy đủ cột cho Identity + FullName)
CREATE TABLE AspNetUsers (
    Id                   INT IDENTITY(1,1) PRIMARY KEY,
    UserName             NVARCHAR(256) NULL,
    NormalizedUserName   NVARCHAR(256) NULL,
    Email                NVARCHAR(256) NULL,
    NormalizedEmail      NVARCHAR(256) NULL,
    EmailConfirmed       BIT NOT NULL DEFAULT 0,
    PasswordHash         NVARCHAR(MAX) NULL,
    SecurityStamp        NVARCHAR(MAX) NULL,
    ConcurrencyStamp     NVARCHAR(MAX) NULL,
    PhoneNumber          NVARCHAR(MAX) NULL,
    PhoneNumberConfirmed BIT NOT NULL DEFAULT 0,
    TwoFactorEnabled     BIT NOT NULL DEFAULT 0,
    LockoutEnd           DATETIME2 NULL,
    LockoutEnabled       BIT NOT NULL DEFAULT 1,
    AccessFailedCount    INT NOT NULL DEFAULT 0,
    FullName             NVARCHAR(100) NULL,
    CreatedAt            DATETIME2 DEFAULT GETDATE()
);

-- 2. AspNetRoles
CREATE TABLE AspNetRoles (
    Id                   INT IDENTITY(1,1) PRIMARY KEY,
    Name                 NVARCHAR(256) NULL,
    NormalizedName       NVARCHAR(256) NULL,
    ConcurrencyStamp     NVARCHAR(MAX) NULL
);

-- 3. Các bảng Identity còn lại
CREATE TABLE AspNetUserClaims (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    UserId     INT NOT NULL,
    ClaimType  NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    CONSTRAINT FK_AspNetUserClaims_AspNetUsers FOREIGN KEY (UserId) 
        REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE AspNetUserLogins (
    LoginProvider       NVARCHAR(128) NOT NULL,
    ProviderKey         NVARCHAR(128) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX) NULL,
    UserId              INT NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    CONSTRAINT FK_AspNetUserLogins_AspNetUsers FOREIGN KEY (UserId) 
        REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE AspNetUserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_AspNetUserRoles_Users FOREIGN KEY (UserId) 
        REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AspNetUserRoles_Roles FOREIGN KEY (RoleId) 
        REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

CREATE TABLE AspNetRoleClaims (
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    RoleId   INT NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    CONSTRAINT FK_AspNetRoleClaims_AspNetRoles FOREIGN KEY (RoleId) 
        REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

CREATE TABLE AspNetUserTokens (
    UserId        INT NOT NULL,
    LoginProvider NVARCHAR(128) NOT NULL,
    Name          NVARCHAR(128) NOT NULL,
    Value         NVARCHAR(MAX) NULL,
    PRIMARY KEY (UserId, LoginProvider, Name),
    CONSTRAINT FK_AspNetUserTokens_AspNetUsers FOREIGN KEY (UserId) 
        REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Index bắt buộc
CREATE UNIQUE INDEX IX_AspNetUsers_NormalizedUserName 
ON AspNetUsers(NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;

CREATE UNIQUE INDEX IX_AspNetUsers_NormalizedEmail 
ON AspNetUsers(NormalizedEmail) WHERE NormalizedEmail IS NOT NULL;

CREATE UNIQUE INDEX IX_AspNetRoles_NormalizedName 
ON AspNetRoles(NormalizedName) WHERE NormalizedName IS NOT NULL;

-- Tạo 3 Role cơ bản
INSERT INTO AspNetRoles (Name, NormalizedName) VALUES 
('Admin',   'ADMIN'),
('Faculty', 'FACULTY'),
('Student', 'STUDENT');

-- Tạo tài khoản Admin mẫu (mật khẩu: Admin@123)
-- Password hash được tạo bằng ASP.NET Identity → sẽ thêm bằng code sau
PRINT 'Đã tạo xong các bảng Identity!'

-- Tạo các bảng đồ án (sẽ do Migration tạo, nhưng để sẵn tên bảng tránh lỗi)
-- Nếu bạn dùng Migration thì bỏ qua phần này
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Departments')
CREATE TABLE Departments (
    DepartmentId   INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentCode NVARCHAR(20) NOT NULL,
    DepartmentName NVARCHAR(100) NOT NULL
);

PRINT '====================================================================='
PRINT 'HOÀN TẤT! Database SIMS_DB đã sẵn sàng.'
PRINT 'Bây giờ chạy lệnh sau trong terminal dự án:'
PRINT 'dotnet ef database update'
PRINT '→ EF Core sẽ tạo các bảng còn lại (Students, Courses, CourseSections...)'
PRINT '====================================================================='