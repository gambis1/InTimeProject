DROP TABLE TimeTrack;
DROP TABLE Person;
DROP TABLE Project;

CREATE TABLE [dbo].[Person] (
    [Id]         INT          NOT NULL PRIMARY KEY IDENTITY(1,1),
    [PersonName] VARCHAR (30) NOT NULL,
	[AccessCode] UNIQUEIDENTIFIER DEFAULT(NEWID())
);

CREATE TABLE [dbo].[Project] (
    [Id]          INT           NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ProjectName] VARCHAR (30)  NOT NULL,
    [ProjectAssignedTime]   TIME (7),
    [Description] VARCHAR (MAX) NULL,
);

CREATE TABLE [dbo].[TimeTrack] (
    [Id]          INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ProjectId]   INT      NOT NULL,
    [PersonId]    INT      NOT NULL,
	[AssignedTime] INT,
    [WorkTime] TIME (7) NOT NULL,
    CONSTRAINT [FK_Time_ToPerson] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_Time_ToProject] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([Id])
);
