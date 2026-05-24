IF DB_ID('LegacyPMDb') IS NULL
BEGIN
    CREATE DATABASE LegacyPMDb;
END
GO

USE LegacyPMDb;
GO

IF OBJECT_ID('dbo.TimeEntries', 'U') IS NOT NULL DROP TABLE dbo.TimeEntries;
IF OBJECT_ID('dbo.ProjectMembers', 'U') IS NOT NULL DROP TABLE dbo.ProjectMembers;
IF OBJECT_ID('dbo.NotificationLogs', 'U') IS NOT NULL DROP TABLE dbo.NotificationLogs;
IF OBJECT_ID('dbo.Reports', 'U') IS NOT NULL DROP TABLE dbo.Reports;
IF OBJECT_ID('dbo.Milestones', 'U') IS NOT NULL DROP TABLE dbo.Milestones;
IF OBJECT_ID('dbo.ProjectTasks', 'U') IS NOT NULL DROP TABLE dbo.ProjectTasks;
IF OBJECT_ID('dbo.Resources', 'U') IS NOT NULL DROP TABLE dbo.Resources;
IF OBJECT_ID('dbo.Projects', 'U') IS NOT NULL DROP TABLE dbo.Projects;
GO

CREATE TABLE dbo.Projects (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    OwnerId NVARCHAR(100) NOT NULL,
    Budget DECIMAL(18,2) NOT NULL,
    ActualCost DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL
);
GO

CREATE TABLE dbo.Resources (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    Role NVARCHAR(100) NOT NULL,
    Department NVARCHAR(100) NOT NULL,
    HourlyRate DECIMAL(18,2) NOT NULL,
    IsActive BIT NOT NULL
);
GO

CREATE TABLE dbo.ProjectTasks (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProjectId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    AssignedToId INT NULL,
    DueDate DATETIME NOT NULL,
    Priority NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    EstimatedHours DECIMAL(18,2) NOT NULL,
    ActualHours DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    CONSTRAINT FK_ProjectTasks_Projects FOREIGN KEY (ProjectId) REFERENCES dbo.Projects(Id),
    CONSTRAINT FK_ProjectTasks_Resources FOREIGN KEY (AssignedToId) REFERENCES dbo.Resources(Id)
);
GO

CREATE TABLE dbo.Milestones (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProjectId INT NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    DueDate DATETIME NOT NULL,
    IsCompleted BIT NOT NULL,
    CompletedAt DATETIME NULL,
    CONSTRAINT FK_Milestones_Projects FOREIGN KEY (ProjectId) REFERENCES dbo.Projects(Id)
);
GO

CREATE TABLE dbo.TimeEntries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProjectTaskId INT NOT NULL,
    ResourceId INT NOT NULL,
    Date DATETIME NOT NULL,
    Hours DECIMAL(18,2) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CreatedAt DATETIME NOT NULL,
    CONSTRAINT FK_TimeEntries_ProjectTasks FOREIGN KEY (ProjectTaskId) REFERENCES dbo.ProjectTasks(Id),
    CONSTRAINT FK_TimeEntries_Resources FOREIGN KEY (ResourceId) REFERENCES dbo.Resources(Id)
);
GO

CREATE TABLE dbo.ProjectMembers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProjectId INT NOT NULL,
    ResourceId INT NOT NULL,
    Role NVARCHAR(100) NOT NULL,
    JoinedAt DATETIME NOT NULL,
    CONSTRAINT FK_ProjectMembers_Projects FOREIGN KEY (ProjectId) REFERENCES dbo.Projects(Id),
    CONSTRAINT FK_ProjectMembers_Resources FOREIGN KEY (ResourceId) REFERENCES dbo.Resources(Id)
);
GO

CREATE TABLE dbo.Reports (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProjectId INT NOT NULL,
    ReportType NVARCHAR(100) NOT NULL,
    GeneratedAt DATETIME NOT NULL,
    GeneratedByUserId NVARCHAR(100) NOT NULL,
    CachedData VARBINARY(MAX) NULL,
    Parameters NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Reports_Projects FOREIGN KEY (ProjectId) REFERENCES dbo.Projects(Id)
);
GO

CREATE TABLE dbo.NotificationLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RecipientEmail NVARCHAR(200) NOT NULL,
    Subject NVARCHAR(200) NOT NULL,
    Body NVARCHAR(MAX) NOT NULL,
    SentAt DATETIME NOT NULL,
    Status NVARCHAR(200) NOT NULL,
    Culture NVARCHAR(10) NOT NULL
);
GO