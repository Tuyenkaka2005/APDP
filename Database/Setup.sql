
CREATE DATABASE SIMS_DB;
GO
USE SIMS_DB;
GO

PRINT 'Tạo bảng ASP.NET Core Identity (cho AppUser + AppRole<int>)...'

-- AspNetUsers (Identity + FullName)
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

-- AspNetRoles
CREATE TABLE AspNetRoles (
    Id                   INT IDENTITY(1,1) PRIMARY KEY,
    Name                 NVARCHAR(256) NULL,
    NormalizedName       NVARCHAR(256) NULL,
    ConcurrencyStamp     NVARCHAR(MAX) NULL
);

-- Các bảng Identity khác
CREATE TABLE AspNetUserClaims (Id INT IDENTITY(1,1) PRIMARY KEY, UserId INT NOT NULL, ClaimType NVARCHAR(MAX), ClaimValue NVARCHAR(MAX),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE);
CREATE TABLE AspNetUserLogins (LoginProvider NVARCHAR(128) NOT NULL, ProviderKey NVARCHAR(128) NOT NULL, ProviderDisplayName NVARCHAR(MAX), UserId INT NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey), FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE);
CREATE TABLE AspNetUserRoles (UserId INT NOT NULL, RoleId INT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE);
CREATE TABLE AspNetRoleClaims (Id INT IDENTITY(1,1) PRIMARY KEY, RoleId INT NOT NULL, ClaimType NVARCHAR(MAX), ClaimValue NVARCHAR(MAX),
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE);
CREATE TABLE AspNetUserTokens (UserId INT NOT NULL, LoginProvider NVARCHAR(128) NOT NULL, Name NVARCHAR(128) NOT NULL, Value NVARCHAR(MAX),
    PRIMARY KEY (UserId, LoginProvider, Name), FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE);

-- Index cho Identity
CREATE UNIQUE INDEX IX_AspNetUsers_NormalizedUserName ON AspNetUsers(NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;
CREATE UNIQUE INDEX IX_AspNetUsers_NormalizedEmail ON AspNetUsers(NormalizedEmail) WHERE NormalizedEmail IS NOT NULL;
CREATE UNIQUE INDEX IX_AspNetRoles_NormalizedName ON AspNetRoles(NormalizedName) WHERE NormalizedName IS NOT NULL;

-- Tạo 3 Role cơ bản
INSERT INTO AspNetRoles (Name, NormalizedName) VALUES 
('Admin', 'ADMIN'), ('Faculty', 'FACULTY'), ('Student', 'STUDENT');

PRINT 'Tạo bảng đồ án SIMS...'

-- Departments
CREATE TABLE Departments (
    DepartmentId   INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentCode NVARCHAR(20) NOT NULL UNIQUE,
    DepartmentName NVARCHAR(100) NOT NULL
);

-- AcademicPrograms
CREATE TABLE AcademicPrograms (
    AcademicProgramId INT IDENTITY(1,1) PRIMARY KEY,
    ProgramCode       NVARCHAR(20) NOT NULL UNIQUE,
    ProgramName       NVARCHAR(100) NOT NULL,
    DegreeType        NVARCHAR(50) NOT NULL DEFAULT 'Bachelor',
    DurationYears     INT NOT NULL DEFAULT 4,
    RequiredCredits   INT NOT NULL DEFAULT 140,
    DepartmentId      INT NOT NULL,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
);

-- Courses
CREATE TABLE Courses (
    CourseId       INT IDENTITY(1,1) PRIMARY KEY,
    CourseCode     NVARCHAR(20) NOT NULL UNIQUE,
    CourseName     NVARCHAR(100) NOT NULL,
    Credits        INT NOT NULL DEFAULT 3,
    Description    NVARCHAR(500) NULL,
    DepartmentId   INT NOT NULL,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
);

-- CourseSections
CREATE TABLE CourseSections (
    SectionId      INT IDENTITY(1,1) PRIMARY KEY,
    CourseId       INT NOT NULL,
    FacultyId      INT NULL,
    Semester       NVARCHAR(10) NOT NULL DEFAULT 'HK1',
    AcademicYear   INT NOT NULL DEFAULT YEAR(GETDATE()),
    Capacity       INT NOT NULL DEFAULT 60,
    EnrolledCount  INT NOT NULL DEFAULT 0,
    Room           NVARCHAR(20) NULL,
    Schedule       NVARCHAR(100) NULL,
    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId),
    FOREIGN KEY (FacultyId) REFERENCES AspNetUsers(Id)
);

-- Students
CREATE TABLE Students (
    StudentId         INT IDENTITY(1,1) PRIMARY KEY,
    UserId            INT NOT NULL UNIQUE,
    StudentCode       NVARCHAR(20) NOT NULL UNIQUE,
    FullName          NVARCHAR(100) NOT NULL,
    AdmissionDate     DATE DEFAULT GETDATE(),
    AcademicProgramId INT NULL,
    GPA               DECIMAL(3,2) DEFAULT 0,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (AcademicProgramId) REFERENCES AcademicPrograms(AcademicProgramId)
);

-- Enrollments
CREATE TABLE Enrollments (
    EnrollmentId   INT IDENTITY(1,1) PRIMARY KEY,
    StudentId      INT NOT NULL,
    SectionId      INT NOT NULL,
    EnrollDate     DATETIME2 DEFAULT GETDATE(),
    Status         NVARCHAR(20) DEFAULT 'Enrolled',
    MidtermScore   DECIMAL(5,2) NULL,
    FinalScore     DECIMAL(5,2) NULL,
    TotalScore     DECIMAL(5,2) NULL,
    LetterGrade    NVARCHAR(5) NULL,
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId),
    FOREIGN KEY (SectionId) REFERENCES CourseSections(SectionId),
    CONSTRAINT UQ_Student_Section UNIQUE (StudentId, SectionId)
);

-- Grades
CREATE TABLE Grades (
    GradeId        INT IDENTITY(1,1) PRIMARY KEY,
    EnrollmentId   INT NOT NULL,
    FinalGrade     DECIMAL(5,2) NULL,
    LetterGrade    NVARCHAR(5) NULL,
    GradeDate      DATETIME2 DEFAULT GETDATE(),
    GradedByFacultyId INT NULL,
    FOREIGN KEY (EnrollmentId) REFERENCES Enrollments(EnrollmentId) ON DELETE CASCADE,
    FOREIGN KEY (GradedByFacultyId) REFERENCES AspNetUsers(Id)
);

-- Faculty (liên kết 1-1 với AspNetUsers)
CREATE TABLE Faculty (
    FacultyId      INT IDENTITY(1,1) PRIMARY KEY,
    UserId         INT NOT NULL UNIQUE,
    EmployeeCode   NVARCHAR(20) NOT NULL UNIQUE,
    FullName       NVARCHAR(100) NOT NULL,
    DepartmentId   INT NULL,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
);

-- Admins (liên kết 1-1 với AspNetUsers)
CREATE TABLE Admins (
    AdminId        INT IDENTITY(1,1) PRIMARY KEY,
    UserId         INT NOT NULL UNIQUE,
    EmployeeCode   NVARCHAR(20) NOT NULL UNIQUE,
    FullName       NVARCHAR(100) NOT NULL,
    Position       NVARCHAR(50) DEFAULT 'Administrator',
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Tạo bảng __EFMigrationsHistory để tránh lỗi migration
CREATE TABLE __EFMigrationsHistory (
    MigrationId    NVARCHAR(150) NOT NULL PRIMARY KEY,
    ProductVersion NVARCHAR(32) NOT NULL
);

-- Đánh dấu migration Identity + FullSIMS đã chạy
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES
('00000000000000_CreateIdentitySchema', '8.0.15'),
('20251201151306_FullSIMS', '8.0.15');

PRINT 'HOÀN TẤT 100%! Database SIMS_DB đã có đầy đủ bảng!'
PRINT 'Các thành viên chỉ cần chạy file này 1 lần là có DB giống bạn 100%!'
PRINT 'Chạy web → đăng ký tài khoản → vào /Admin để gán role!'
