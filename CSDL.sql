-- =============================================
-- STUDENT INFORMATION MANAGEMENT SYSTEM (SIMS)
-- Database Script - SQL Server
-- =============================================

USE master;
GO

-- Tạo database nếu chưa tồn tại
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SIMS_DB')
BEGIN
    CREATE DATABASE SIMS_DB;
END
GO

USE SIMS_DB;
GO

-- 1. Role (Vai trò người dùng)
CREATE TABLE [Role] (
    RoleID      INT IDENTITY(1,1) PRIMARY KEY,
    RoleName    NVARCHAR(50) NOT NULL UNIQUE,   -- Admin, Faculty, Student
    Description NVARCHAR(255),
    Permissions NVARCHAR(MAX),                 -- Có thể lưu JSON hoặc text
    CreatedAt   DATETIME2 DEFAULT GETDATE()
);

-- 2. User (Tài khoản đăng nhập chung)
CREATE TABLE [User] (
    UserID         INT IDENTITY(1,1) PRIMARY KEY,
    Username       NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash   VARBINARY(64) NOT NULL,        -- SHA-256
    Email          NVARCHAR(255) NOT NULL UNIQUE,
    Phone          NVARCHAR(20),
    FullName       NVARCHAR(255) NOT NULL,
    DateOfBirth    DATE,
    Address        NVARCHAR(500),
    CreatedAt      DATETIME2 DEFAULT GETDATE(),
    UpdatedAt      DATETIME2 DEFAULT GETDATE(),
    LastLogin      DATETIME2,
    RoleID         INT NOT NULL,
    CONSTRAINT FK_User_Role FOREIGN KEY (RoleID) REFERENCES [Role](RoleID)
);
GO

-- 3. Department
CREATE TABLE Department (
    DepartmentID    INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentCode  NVARCHAR(20) NOT NULL UNIQUE,
    DepartmentName  NVARCHAR(255) NOT NULL,
    Description     NVARCHAR(500),
    HeadFacultyID   INT,                           -- Trưởng khoa
    Location        NVARCHAR(255),
    HeadOfDepartment NVARCHAR(255),
    Phone           NVARCHAR(20),
    Email           NVARCHAR(255),
    CreatedAt       DATETIME2 DEFAULT GETDATE()
);
GO

-- 4. AcademicPrograms (Chương trình đào tạo)
CREATE TABLE AcademicPrograms (
    ProgramID       INT IDENTITY(1,1) PRIMARY KEY,
    ProgramCode     NVARCHAR(20) NOT NULL UNIQUE,
    ProgramName     NVARCHAR(255) NOT NULL,
    DegreeType      NVARCHAR(100),                 -- Bachelor, Master, ...
    DurationYears   INT,
    RequiredCredits INT,
    DepartmentID    INT NOT NULL,
    Description     NVARCHAR(1000),
    IsActive        BIT DEFAULT 1,
    CreatedAt       DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Program_Department FOREIGN KEY (DepartmentID) 
        REFERENCES Department(DepartmentID)
);
GO

-- 5. Student
CREATE TABLE Student (
    StudentID       INT IDENTITY(1,1) PRIMARY KEY,
    UserID          INT NOT NULL UNIQUE,
    StudentCode     NVARCHAR(20) NOT NULL UNIQUE,   -- MSSV
    AdmissionDate   DATE,
    AdmissionType   NVARCHAR(50),                  -- Chính quy, Chất lượng cao, ...
    GPA             DECIMAL(3,2) DEFAULT 0,
    TotalCredits    INT DEFAULT 0,
    Status          NVARCHAR(50) DEFAULT 'Active', -- Active, Graduated, Suspended
    ProgramID       INT NOT NULL,
    DepartmentID    INT,
    EmergencyContact NVARCHAR(255),
    EmergencyPhone  NVARCHAR(20),
    CONSTRAINT FK_Student_User FOREIGN KEY (UserID) REFERENCES [User](UserID),
    CONSTRAINT FK_Student_AcademicPrograms FOREIGN KEY (ProgramID) REFERENCES AcademicPrograms(ProgramID),
    CONSTRAINT FK_Student_Department FOREIGN KEY (DepartmentID) REFERENCES Department(DepartmentID)
);
GO

-- 6. Faculty (Giảng viên)
CREATE TABLE Faculty (
    FacultyID       INT IDENTITY(1,1) PRIMARY KEY,
    UserID          INT NOT NULL UNIQUE,
    EmployeeCode    NVARCHAR(20) NOT NULL UNIQUE,
    HireDate        DATE,
    Qualification   NVARCHAR(100),            -- Tiến sĩ, Thạc sĩ,...
    Specialization  NVARCHAR(255),
    Position        NVARCHAR(100),            -- Lecturer, Professor,...
    DepartmentID    INT,
    OfficeLocation  NVARCHAR(255),
    CONSTRAINT FK_Faculty_User FOREIGN KEY (UserID) REFERENCES [User](UserID),
    CONSTRAINT FK_Faculty_Department FOREIGN KEY (DepartmentID) REFERENCES Department(DepartmentID)
);
GO

-- 7. Admin (Quản trị viên hệ thống)
CREATE TABLE Admin (
    AdminID             INT IDENTITY(1,1) PRIMARY KEY,
    UserID              INT NOT NULL UNIQUE,
    EmployeeCode        NVARCHAR(20) NOT NULL UNIQUE,
    HireDate            DATE,
    Position            NVARCHAR(100),
    DepartmentAssigned  NVARCHAR(255),
    CONSTRAINT FK_Admin_User FOREIGN KEY (UserID) REFERENCES [User](UserID)
);
GO

-- 8. Course (Môn học)
CREATE TABLE Course (
    CourseID        INT IDENTITY(1,1) PRIMARY KEY,
    CourseCode      NVARCHAR(20) NOT NULL UNIQUE,
    CourseName      NVARCHAR(255) NOT NULL,
    Credits         INT NOT NULL,
    Description     NVARCHAR(1000),
    Level           NVARCHAR(50),             -- Năm 1,2,3,4
    MaxStudents     INT DEFAULT 100,
    FacultyID       INT,                      -- Giảng viên phụ trách (có thể NULL)
    DepartmentID    INT NOT NULL,
    Semester        NVARCHAR(20),             -- Fall 2025, Spring 2026,...
    AcademicYear    NVARCHAR(20),             -- 2024-2025
    Schedule        NVARCHAR(500),
    Room            NVARCHAR(50),
    IsActive        BIT DEFAULT 1,
    CONSTRAINT FK_Course_Faculty FOREIGN KEY (FacultyID) REFERENCES Faculty(FacultyID),
    CONSTRAINT FK_Course_Department FOREIGN KEY (DepartmentID) REFERENCES Department(DepartmentID)
);
GO

-- 9. ProgramCourse (Môn học thuộc chương trình - bắt buộc/tự chọn)
CREATE TABLE ProgramCourse (
    ProgramCourseID     INT IDENTITY(1,1) PRIMARY KEY,
    ProgramID           INT NOT NULL,
    CourseID            INT NOT NULL,
    IsRequired          BIT DEFAULT 1,            -- Bắt buộc hay tự chọn
    SemesterRecommended INT,                      -- Học kỳ đề xuất
    PrerequisiteCourses NVARCHAR(500),            -- Danh sách mã môn tiên quyết
    CONSTRAINT FK_ProgramCourse_Program FOREIGN KEY (ProgramID) REFERENCES Program(ProgramID),
    CONSTRAINT FK_ProgramCourse_Course FOREIGN KEY (CourseID) REFERENCES Course(CourseID),
    CONSTRAINT UQ_ProgramCourse UNIQUE (ProgramID, CourseID)
);
GO

-- 10. Enrollment (Sinh viên đăng ký học môn)
CREATE TABLE Enrollment (
    EnrollmentID    INT IDENTITY(1,1) PRIMARY KEY,
    StudentID       INT NOT NULL,
    CourseID        INT NOT NULL,
    EnrollmentDate  DATETIME2 DEFAULT GETDATE(),
    Status          NVARCHAR(50) DEFAULT 'Enrolled', -- Enrolled, Completed, Dropped
    AttendanceCount INT DEFAULT 0,
    MidtermScore    DECIMAL(5,2),
    FinalScore      DECIMAL(5,2),
    Semester        NVARCHAR(20),
    AcademicYear    NVARCHAR(20),
    CONSTRAINT FK_Enrollment_Student FOREIGN KEY (StudentID) REFERENCES Student(StudentID),
    CONSTRAINT FK_Enrollment_Course FOREIGN KEY (CourseID) REFERENCES Course(CourseID),
    CONSTRAINT UQ_Enrollment UNIQUE (StudentID, CourseID, Semester, AcademicYear)
);
GO

-- 11. Grade (Bảng điểm chi tiết)
CREATE TABLE Grade (
    GradeID         INT IDENTITY(1,1) PRIMARY KEY,
    EnrollmentID    INT NOT NULL,
    StudentID       INT NOT NULL,
    CourseID        INT NOT NULL,
    FinalGrade      DECIMAL(5,2),
    LetterGrade     NVARCHAR(5),              -- A, B+, B, ...
    GradeStatus     NVARCHAR(50) DEFAULT 'Pending', -- Passed, Failed, Incomplete
    GradeDate       DATETIME2 DEFAULT GETDATE(),
    GradedByFacultyID INT,
    Comments        NVARCHAR(1000),
    CONSTRAINT FK_Grade_Enrollment FOREIGN KEY (EnrollmentID) REFERENCES Enrollment(EnrollmentID),
    CONSTRAINT FK_Grade_Faculty FOREIGN KEY (GradedByFacultyID) REFERENCES Faculty(FacultyID)
);
GO

-- =============================================
-- INSERT DỮ LIỆU MẪU (SEED DATA)
-- =============================================

-- Role
INSERT INTO [Role] (RoleName, Description) VALUES
('Admin', 'System administrator'),
('Faculty', 'Lecturer'),
('Student', 'Student');

-- User mẫu (mật khẩu đã hash SHA256 của "123456")
-- Lưu ý: Trong thực tế phải dùng bcrypt/Argon2, đây chỉ là mẫu
INSERT INTO [User] (Username, PasswordHash, Email, FullName, RoleID) VALUES
('admin', 0xA665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3, 'admin@university.edu.vn', 'Administrator', 1),
('gv001', 0xA665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3, 'nguyenvana@university.edu.vn', 'Nguyen Dang Tuyen', 2),
('sv2021001', 0xA665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3, 'sinhvien1@student.edu.vn', 'Bui Gia Duy', 3);

-- Department mẫu
INSERT INTO Department (DepartmentCode, DepartmentName) VALUES
('IT', 'Information technology'),
('Economy', 'Faculty of Economics');

-- Program mẫu
INSERT INTO AcademicPrograms (ProgramCode, ProgramName, DegreeType, DurationYears, RequiredCredits, DepartmentID) VALUES
('CNTT2021', 'Bachelor of Information Technology 2025', 'Bachelor', 4, 140, 1);

-- Gán sinh viên vào bảng Student
INSERT INTO Student (UserID, StudentCode, ProgramID, DepartmentID) VALUES
(3, '2021001', 1, 1);

-- Gán giảng viên
INSERT INTO Faculty (UserID, EmployeeCode, DepartmentID) VALUES
(2, 'GV001', 1);

-- Gán admin
INSERT INTO Admin (UserID, EmployeeCode) VALUES
(1, 'AD001');

-- Course mẫu
INSERT INTO Course (CourseCode, CourseName, Credits, DepartmentID, FacultyID, Semester, AcademicYear) VALUES
('INT101', 'Basic C# Programming', 4, 1, 1, 'Fall', '2025-2026'),
('DB101', 'Database', 3, 1, 1, 'Fall', '2025-2026');

-- ProgramCourse mẫu
INSERT INTO ProgramCourse (ProgramID, CourseID, IsRequired, SemesterRecommended) VALUES
(1, 1, 1, 1),
(1, 2, 1, 2);

-- Enrollment mẫu
INSERT INTO Enrollment (StudentID, CourseID, Semester, AcademicYear) VALUES
(1, 1, 'Fall', '2025-2026');

PRINT 'Tạo CSDL SIMS_DB thành công! Đã có dữ liệu mẫu.';