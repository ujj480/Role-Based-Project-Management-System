-- SQL Server 2019 schema for ProjectTracker
IF DB_ID('ProjectTracker') IS NULL
BEGIN
    CREATE DATABASE ProjectTracker;
END
GO
USE ProjectTracker;
GO

IF OBJECT_ID('dbo.UserRoles') IS NOT NULL DROP TABLE dbo.UserRoles;
IF OBJECT_ID('dbo.Tasks') IS NOT NULL DROP TABLE dbo.Tasks;
IF OBJECT_ID('dbo.ProjectMembers') IS NOT NULL DROP TABLE dbo.ProjectMembers;
IF OBJECT_ID('dbo.Projects') IS NOT NULL DROP TABLE dbo.Projects;
IF OBJECT_ID('dbo.Users') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.Roles') IS NOT NULL DROP TABLE dbo.Roles;
GO

CREATE TABLE Roles(
    RoleId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(50) UNIQUE NOT NULL
);
CREATE TABLE Users(
    UserId INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash CHAR(64) NOT NULL,
    FullName NVARCHAR(150) NULL,
    Email NVARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT(1),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE UserRoles(
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId) ON DELETE CASCADE,
    RoleId INT NOT NULL FOREIGN KEY REFERENCES Roles(RoleId) ON DELETE CASCADE,
    CONSTRAINT PK_UserRoles PRIMARY KEY(UserId,RoleId)
);

CREATE TABLE Projects(
    ProjectId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Description NVARCHAR(1000) NULL,
    OwnerUserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE ProjectMembers(
    ProjectId INT NOT NULL FOREIGN KEY REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId) ON DELETE CASCADE,
    RoleInProject NVARCHAR(50) NULL,
    CONSTRAINT PK_ProjectMembers PRIMARY KEY(ProjectId,UserId)
);

CREATE TABLE Tasks(
    TaskId INT IDENTITY PRIMARY KEY,
    ProjectId INT NOT NULL FOREIGN KEY REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(2000) NULL,
    Status NVARCHAR(50) NOT NULL,
    AssignedTo INT NULL FOREIGN KEY REFERENCES Users(UserId),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy INT NOT NULL FOREIGN KEY REFERENCES Users(UserId)
);

-- Seed roles
INSERT INTO Roles(Name) VALUES ('Admin'),('Project Manager'),('Developer'),('Viewer');

-- Seed admin user (password: Admin@123) with fixed salt in appSettings, hash precomputed
INSERT INTO Users(Username,PasswordHash,FullName,Email,IsActive) VALUES
('admin','67723e4f4d0bfd542e5ebf75bb6e15ef0520f7256d788911ffe4cb0e55bad3f9','System Admin','admin@example.com',1);

-- Assign Admin role
INSERT INTO UserRoles(UserId,RoleId) SELECT u.UserId, r.RoleId FROM Users u CROSS JOIN Roles r WHERE u.Username='admin' AND r.Name='Admin';
GO
