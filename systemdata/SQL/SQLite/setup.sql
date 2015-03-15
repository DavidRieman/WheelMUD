CREATE TABLE [ANSI] (
[ID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[EscapeCode] NVARCHAR(50)  NULL,
[Tag] nvarchar(25)  NULL
);


CREATE TABLE [Areas] (
  [ID] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [UID] NVARCHAR(50), 
  [Name] nvarchar(45));


CREATE TABLE [BannedIPAddresses] (
[ID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[StartIPAddress] nvarchar(20)  NOT NULL,
[EndIPAddress] nvarchar(20)  NOT NULL,
[Note] nvarchar(255)  NOT NULL,
[BannedByPlayerID] INTEGER  NOT NULL,
[BannedDateTime] DATE  NOT NULL
);

CREATE TABLE [DayNames] (
  [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
  [DayName] NVARCHAR(25) NOT NULL
);

CREATE TABLE [Doors] (
[ID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[DoorSideAID] INTEGER  NULL,
[DoorSideBID] INTEGER  NULL,
[OpenState] INTEGER  NULL,
[Name] nvarchar(45)  NULL,
[Description] nvarchar(1000)  NULL
);

CREATE TABLE [DoorSides] (
[ID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[Name] nvarchar(45)  NULL,
[Description] nvarchar(1000)  NULL
);

CREATE TABLE [Exits] (
[ID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[ExitRoomAID] INTEGER  NULL,
[DirectionA] nvarchar(50)  NULL,
[ExitRoomBID] INTEGER  NULL,
[DirectionB] nvarchar(50)  NULL,
[DoorID] INTEGER  NULL
);

CREATE TABLE [HelpTopicAliases] (
[ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
[HelpTopicAlias] nvarchar(50) NOT NULL,
[HelpTopicID] INTEGER NULL
);

CREATE TABLE [HelpTopics] (
[ID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[HelpTopic] nvarchar(50)  NOT NULL,
[Usage] nvarchar(255)  NULL,
[Description] nvarchar(1000)  NULL,
[Example] nvarchar(1000)  NULL,
[SeeAlso] nvarchar(1000)  NULL,
[ViewTemplate] nvarchar(255)  NULL
);

CREATE TABLE [IAC] (
[ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[Name] nvarchar(50)  NULL,
[OptionCode] int  NULL,
[NegotiateAtConnect] bit  NOT NULL,
[RequiresSubNegotiation] bit  NOT NULL,
[SubNegAssembly] nvarchar(50)  NULL,
[NegotiationStartValue] nvarchar(50)  NULL
);

CREATE TABLE [Mobs] (
[ID] INTEGER  PRIMARY KEY AUTOINCREMENT NULL,
[MobTypeID] int  NULL,
[Name] nvarchar(45)  NULL,
[Title] nvarchar(50)  NULL,
[Description] nvarchar(250)  NULL,
[Age] int  NULL,
[CurrentRoomID] int  NULL,
[Prompt] nvarchar(50)  NULL,
[CreateDate] TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
);

CREATE TABLE [MobTypes] (
[ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[MobTypeName] NVARCHAR(50)  NOT NULL
);

CREATE TABLE [MonthNames] (
  [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
  [MonthName] NVARCHAR(25) NOT NULL
);

CREATE TABLE [MudChannelRoles] (
[ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[MudChannelID] INTEGER  NOT NULL,
[RoleID] INTEGER  NOT NULL
);

CREATE TABLE [MudChannels] (
[ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[MudChannelName] NVARCHAR(50)  NOT NULL
);

CREATE TABLE [MXP] (
[ID] INTEGER  PRIMARY KEY AUTOINCREMENT NULL,
[ElementName] nvarchar(50)  NULL,
[ElementDefinition] nvarchar(50)  NOT NULL
);

CREATE TABLE [PlayerChannels] (
[ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[PlayerID] INTEGER  NOT NULL,
[ChannelID] INTEGER  NOT NULL
);

CREATE TABLE [PlayerIPAddress] (
    [ID] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
    [PlayerID] integer NOT NULL,
    [IPAddress] nvarchar(50) NOT NULL
);

CREATE TABLE [PlayerRoles] (
[ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[PlayerID] INTEGER  NULL,
[RoleID] INTEGER  NULL
);

CREATE TABLE [Players] (
  [ID] integer NOT NULL PRIMARY KEY AUTOINCREMENT, 
  [UserName] nvarchar(45) NOT NULL, 
  [Password] nvarchar(45) NOT NULL, 
  [DisplayName] nvarchar(50), 
  [Suffix] nvarchar(45), 
  [Prefix] nvarchar(45), 
  [Title] nvarchar(50), 
  [Description] nvarchar(250), 
  [Age] int, 
  [CreateDate] nvarchar(50), 
  [CurrentRoomID] integer, 
  [Prompt] nvarchar(50), 
  [WantAnsi] bit, 
  [WantMXP] bit, 
  [WantMCCP] bit, 
  [LastLogin] nvarchar(50), 
  [LastLogout] nvarchar(50), 
  [LastIPAddress] nvarchar(50), 
  [Email] nvarchar(100), 
  [HomePage] nvarchar(4000), 
  [PlanText] nvarchar, 
  [BufferLength] int NOT NULL DEFAULT ('40'));

CREATE TABLE [PortalExits] (
[ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[PortalID] INTEGER  NOT NULL,
[RoomAID] INTEGER  NULL,
[RoomBID] INTEGER  NULL
);

CREATE TABLE [Roles] (
[ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[Name] nvarchar(50)  NOT NULL,
[SecurityRoleMask] int  NOT NULL
);

CREATE TABLE [Rooms] (
  [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
  [UID] NVARCHAR(50), 
  [AreaID] INTEGER, 
  [Name] nvarchar(45) NOT NULL, 
  [Description] nvarchar(1000), 
  [RoomTypeID] INTEGER);

CREATE TABLE [RoomTypes] (
[ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
[Name] nvarchar(50)  NOT NULL,
[Description] nvarchar(1000)  NULL
);

CREATE TABLE [RoomVisuals] (
  [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
  [RoomID] INTEGER NOT NULL CONSTRAINT [fk_Rooms_Visuals] REFERENCES [Rooms]([ID]) NOT DEFERRABLE INITIALLY DEFERRED, 
  [Name] NVARCHAR(59) NOT NULL, 
  [Description] NVARCHAR(255) NOT NULL);

CREATE TABLE [Typos] (
[ID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
[Note] nvarchar(500)  NOT NULL,
[SubmittedByPlayerID] INTEGER  NOT NULL,
[RoomID] INTEGER  NOT NULL,
[SubmittedDateTime] nvarchar(30)  NOT NULL,
[Resolved] BOOLEAN DEFAULT 'False' NOT NULL,
[ResolvedByPlayerID] INTEGER  NULL,
[ResolvedDateTime] nvarchar(30)  NULL
);

-- Begin inserting data in ANSI
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(1, '1m', '<%b%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(2, '40m', '<%bblack%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(3, '44m', '<%bblue%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(4, '44m', '<%bblue%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(5, '46m', '<%bcyan%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(6, '42m', '<%bgreen%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(7, '30m', '<%black%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(8, '34m', '<%blue%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(9, '45m', '<%bmagenta%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(10, '1m', '<%bold%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(11, '41m', '<%bred%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(12, '47m', '<%bwhite%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(13, '43m', '<%byellow%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(14, '2J', '<%cls%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(15, '36m', '<%cyan%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(16, '32m', '<%green%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(17, '35m', '<%magenta%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(18, '0m', '<%n%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(19, '0m', '<%normal%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(20, '31m', '<%red%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(21, '0m', '<%reset%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(22, '4m', '<%u%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(23, '4m', '<%underline%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(24, '37m', '<%white%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(25, '33m', '<%yellow%>');
INSERT INTO [ANSI] ([ID], [EscapeCode], [Tag])
VALUES(26, '1E', '<%nl%>');

-- Begin inserting data in Areas
INSERT INTO [Areas] ([AreaID], [Name])
VALUES(1, 'Krondor');

-- Begin inserting data in Effects

-- Begin inserting data in Exits
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(1, 2, 'West', 1, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(2, 5, 'South', 2, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(3, 3, 'West', 2, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(4, 10, 'North', 2, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(5, 36, 'South', 3, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(6, 4, 'West', 3, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(7, 37, 'North', 3, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(8, 9, 'South', 4, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(9, 15, 'West', 4, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(10, 14, 'North', 4, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(11, 6, 'South', 5, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(12, 7, 'West', 6, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(13, 8, 'West', 7, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(14, 40, 'North', 7, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(15, 39, 'South', 8, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(16, 26, 'West', 8, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(17, 9, 'North', 8, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(18, 38, 'East', 9, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(19, 11, 'North', 10, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(20, 12, 'West', 11, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(21, 13, 'West', 12, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(22, 14, 'South', 13, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(23, 33, 'West', 13, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(24, 42, 'West', 14, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(25, 41, 'East', 14, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(26, 53, 'South', 15, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(27, 16, 'West', 15, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(28, 17, 'West', 16, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(29, 19, 'South', 17, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(30, 18, 'West', 17, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(31, 27, 'North', 17, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(32, 20, 'South', 19, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(33, 21, 'South', 20, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(34, 43, 'South', 21, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(35, 23, 'West', 21, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(36, 22, 'East', 21, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(37, 47, 'South', 22, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(38, 24, 'East', 22, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(39, 45, 'South', 23, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(40, 46, 'North', 23, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(41, 48, 'North', 24, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(42, 25, 'East', 24, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(43, 26, 'North', 25, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(44, 49, 'West', 27, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(45, 28, 'North', 27, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(46, 29, 'North', 28, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(47, 50, 'East', 28, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(48, 34, 'West', 29, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(49, 30, 'East', 29, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(50, 52, 'South', 30, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(51, 31, 'East', 30, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(52, 32, 'South', 31, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(53, 33, 'East', 32, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(54, 51, 'South', 34, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(55, 35, 'North', 34, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(56, 44, 'South', 43, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(57, 55, 'South', 54, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(58, 56, 'East', 54, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(59, 58, 'South', 55, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(60, 57, 'East', 56, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(61, 18, 'East', 57, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(62, 65, 'South', 58, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(63, 59, 'West', 58, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(64, 60, 'West', 59, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(65, 62, 'South', 60, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(66, 61, 'West', 60, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(67, 64, 'North', 61, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(68, 69, 'South', 62, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(69, 74, 'West', 63, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(70, 64, 'East', 63, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(71, 73, 'North', 64, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(72, 66, 'South', 65, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(73, 68, 'East', 66, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(74, 105, 'South', 67, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(75, 68, 'North', 67, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(76, 70, 'South', 69, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(77, 71, 'West', 70, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(78, 87, 'South', 71, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(79, 89, 'South', 72, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(80, 85, 'North', 72, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(81, 77, 'North', 73, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(82, 75, 'South', 74, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(83, 76, 'West', 74, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(84, 95, 'South', 75, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(85, 91, 'North', 76, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(86, 88, 'North', 77, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(87, 102, 'West', 78, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(88, 98, 'East', 78, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(89, 80, 'West', 79, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(90, 104, 'East', 79, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(91, 81, 'West', 80, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(92, 83, 'South', 81, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(93, 82, 'West', 81, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(94, 84, 'North', 81, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(95, 86, 'West', 85, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(96, 87, 'East', 85, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(97, 98, 'North', 88, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(98, 90, 'South', 89, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(99, 92, 'North', 91, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(100, 93, 'West', 92, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(101, 94, 'West', 93, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(102, 97, 'South', 95, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(103, 98, 'West', 96, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(104, 99, 'North', 96, 'South');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(105, 100, 'East', 96, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(106, 101, 'East', 100, 'West');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(107, 103, 'West', 102, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(108, 104, 'West', 103, 'East');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(109, 106, 'South', 105, 'North');
INSERT INTO [Exits] ([ExitID], [ExitRoomAID], [DirectionA], [ExitRoomBID], [DirectionB])
VALUES(110, 107, 'South', 106, 'North');

-- Begin inserting data in IAC
INSERT INTO [IAC] ([ID], [Name], [OptionCode], [NegotiateAtConnect], [RequiresSubNegotiation], [SubNegAssembly], [NegotiationStartValue])
VALUES(1, 'MXP', 91, 'True', 'True', 'SNMXP', 'WILL');
INSERT INTO [IAC] ([ID], [Name], [OptionCode], [NegotiateAtConnect], [RequiresSubNegotiation], [SubNegAssembly], [NegotiationStartValue])
VALUES(2, 'ECHO', 1, 'False', 'False', NULL, 'WONT');
INSERT INTO [IAC] ([ID], [Name], [OptionCode], [NegotiateAtConnect], [RequiresSubNegotiation], [SubNegAssembly], [NegotiationStartValue])
VALUES(3, 'COMPRESS2', 86, 'True', 'True', 'SNMCCP', 'DO');
INSERT INTO [IAC] ([ID], [Name], [OptionCode], [NegotiateAtConnect], [RequiresSubNegotiation], [SubNegAssembly], [NegotiationStartValue])
VALUES(4, 'TERMINALTYPE', 24, 'True', 'True', 'SNTerminalType', 'DO');
INSERT INTO [IAC] ([ID], [Name], [OptionCode], [NegotiateAtConnect], [RequiresSubNegotiation], [SubNegAssembly], [NegotiationStartValue])
VALUES(5, 'NAWS', 31, 'True', 'True', 'SNNaws', 'DO');

-- Begin inserting data in MXP
INSERT INTO [MXP] ([ID], [ElementName], [ElementDefinition])
VALUES(1, 'godgo', '<!ELEMENT godgo ''<send href="godgo &text;">''>');
INSERT INTO [MXP] ([ID], [ElementName], [ElementDefinition])
VALUES(2, 'Item', '<!ELEMENT Item ''<send href="buy &text;">''>');
INSERT INTO [MXP] ([ID], [ElementName], [ElementDefinition])
VALUES(3, 'lookat', '<!ELEMENT lookat  ''<send href="look &text;">''>');
INSERT INTO [MXP] ([ID], [ElementName], [ElementDefinition])
VALUES(4, 'myexit', '<!ELEMENT myexit ''<send href="&text;">''>');

-- Begin inserting data in Mobs
INSERT INTO [Mobs] ([MobID], [CreateDate], [Name], [Title], [Description], [Age], [GenderID], [RaceID], [CurrentRoomID], [Prompt], [Online], [SpouseID], [ClanID], [ProfessionID])
VALUES(1, '3/24/2008 3:37:13 PM', 'George', 'The Guardian', 'This is a non-descript guard. Nothing to see here. Move along, move along...', NULL, '1', NULL, 1, NULL, 'False', NULL, NULL, NULL);

-- Begin inserting data in PortalExits
INSERT INTO [PortalExits] ([PortalExitID], [PortalID], [RoomAID], [RoomBID])
VALUES(1, 1, 1, 2);

-- Begin inserting data in Roles
INSERT INTO [Roles] ([RoleID], [Name], [SecurityRoleMask])
VALUES(1, 'FULLADMIN', 8192);
INSERT INTO [Roles] ([RoleID], [Name], [SecurityRoleMask])
VALUES(2, 'PLAYER', 32);
INSERT INTO [Roles] ([RoleID], [Name], [SecurityRoleMask])
VALUES(3, 'NONE', 0);
INSERT INTO [Roles] ([RoleID], [Name], [SecurityRoleMask])
VALUES(4, 'MOBILE', 1);
INSERT INTO [Roles] ([RoleID], [Name], [SecurityRoleMask])
VALUES(5, 'ITEM', 2);
INSERT INTO [Roles] ([RoleID], [Name], [SecurityRoleMask])
VALUES(6, 'ROOM', 4);
INSERT INTO [Roles] ([RoleID], [Name], [SecurityRoleMask])
VALUES(7, 'TUTORIALPLAYER', 16);
INSERT INTO [Roles] ([RoleID], [Name], [SecurityRoleMask])
VALUES(8, 'HELPER', 64);
INSERT INTO [Roles] ([RoleID], [Name], [SecurityRoleMask])
VALUES(9, 'MINORBUILDER', 256);
INSERT INTO [Roles] ([RoleID], [Name], [SecurityRoleMask])
VALUES(10, 'FULLBUILDER', 512);
INSERT INTO [Roles] ([RoleID], [Name], [SecurityRoleMask])
VALUES(11, 'MINORADMIN', 4096);
INSERT INTO [Roles] ([RoleID], [Name], [SecurityRoleMask])
VALUES(12, 'ALL', 65535);
INSERT INTO [Roles] ([RoleID], [Name], [SecurityRoleMask])
VALUES(13, 'MARRIED', 128);

-- Begin inserting data in Rooms
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(1, 1, 'Gatehouse', 'You are standing in an ancient, disused gatehouse.  The gates are flung open, and from the rust on the immense hinges, you would guess that they have been kept this way for years.  To the east is Weathersby city proper.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(2, 1, 'Trade Road', 'This is the west-most end of Trade Road.  Eastwards, the road continues deep into the city, its length disguised by distance.  Just to the west, you see a largish gatehouse in the city wall.  It seems rather quiet in the area so close to a city gate...');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(3, 1, 'Trade Road', 'This great road runs directly through the heart of Weathersby. The area seems odd around here; the air still.  A gate marks the end of the road to the west, and the road continues to the east.  Two shops stand across from each other here, the north seeming more busy, the south seeming to attract a better quality of customer.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(4, 1, 'Trade Road', 'Here Trade Road is intersected by another, smaller road. A street sign identifies it as ''Clay Street''.  A cursory look north and south reveals a shop or two, but not much else of interest.  Looking west, you spy a gate in the city wall, and Trade Road continues to the east.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(5, 1, 'Alley', 'You squeeze down this alleyway, scrunching up obscenely to pass by the rubbish.  The alley opens up between two buildings on Trade Road southwards, and to the north the alley continues.  There is a slight smell of decay that teases your nose, making you feel as if it were time to move on...');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(6, 1, 'Alley', 'An alley leads south here from an entrace eastwards on Clay Street.  Trash and rubbish grime up your footwear as you tread lightly through the alleyway; obviously this is not one of the highlight attractions of Weathersby.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(7, 1, 'Clay Street', 'Clay Street ends abruptly here.  Looking around, you espy few exits -- one going back down Clay Street to the east, over the wall, and a shop to the south.  The wall doesn''t look too safe, however; what with all those guards on top.  The shop to the south looks like a considerably nicer place to go.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(8, 1, 'Clay Street', 'Clay Street curves about to the west here.  Southwards you see the intersection of Trade Road and Clay Street.  To the north is a large building from which you hear a lot of laughing and general merryment.  The building seems tall and imposing, making the evident good times inside that much more out of place...');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(9, 1, 'Clay Street', 'Clay Street heads north here, and intersects Trade Road to the south.  The buildings in this area of town look fairly delapidated; this is not where ''urban renewal'' happens evidently.  Westwards, a single intact stone gargoyle marks the rubble of an ancient church.  A sad feeling akin to gloom presses down upon your shoulders, making your own load feel that much more heavier to bear here.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(10, 1, 'Alley', 'A dead cat lies across the entrance to this alleyway as if some sort of guardian or omen.  None of the pedestrians on Trade Road to the north even glance in this direction as they pass on by.  The alley continues on to the south.  A strong smell of old blood and rot wafts through the area...');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(11, 1, 'Alley', 'A table has been set up here, with a rather nice tablecloth and wooden utensils adorning it.  Two corpses sit propped up in chairs before the table, their relative decay giving their age to be about two weeks dead.  You shift your weight uneasily, startling two rats sniffing around the taller corpse.  The alley leads north and east from here.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(12, 1, 'Alley', 'A small straw pallet lies in the middle of this alleyway, with a few bits of wood and string are gathered around it as decoration.  The alley opens up on the south end of Clay Street to the east, and leads deeper in between the buildings to the west.  A strong smell makes your eyes water; there is graffiti on the walls, drawn with an unsteady hand and a large supply of fecal material.  A more sinister smell teases you just below the stench here just before a breeze from the east blows it away from you.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(13, 1, 'Clay Street', 'Clay Street ends right up against the southern wall here,  the only way to go is back north towards Trade Road.  A small breeze teases you with an awful smell, and whisks it away just as quickly.  Something is making you feel rather uneasy about this area, but its probably just your imagination.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(14, 1, 'Clay Street', 'The intersection of Trade Road and Clay Street is just to the north from here.  To the south, Clay Road continues towards the southern city wall, not far away.  A nicely built building with an exquisite architecture is to the east, while the distinctive smell of tannin comes from a shop from the west.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(15, 1, 'Trade Road', 'The traffic on this road seems light at best, becoming a bit thicker to the east.  Westwards, you see the intersection of Trade Road and Clay Street, which runs north-south.  A large open-air building is to the north, and southwards is a large open lot; a fairgrounds, closed for the time being.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(16, 1, 'Trade Road', 'Trade Road seems to widen here slightly, allowing more traffic to travel through the area.  You spot near-identical gatehouses to the east and west marking the boundries of Trade Road, and this neighborhood.  To the south is a large fairgrounds area, closed for the time being.  A building to the north seems to be locked up tight, with a large, ''FOR RENT'' sign out front...');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(17, 1, 'Trade Road', 'Several people are walking along the road here, but they seem to be avoiding you with eyes and movements for the most part.  The citizenry seem quiet; depressed or merely sedentary you would guess.  Trade Road leads far to the west from here, and there is a largish gatehouse not far to the east.  A north-south running road labeled ''Staid Avenue'' intersects here, taking much of the traffic away from the main road to the south.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(18, 1, 'Gatehouse', 'This is a rather large gatehouse in the city wall. The gate looks to be a good defensible point in the city wall, an architectural artifact of more dangerous times in these realms.  Westwards a large road leads through Weathersby.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(19, 1, 'Staid Avenue', 'This is Staid Avenue, just north of where it intersects Trade Road.  The neighborhood here seems fairly low-class, but not unliveable by any means.  Most of the buildings seem to be low cost housing for the common populace of Weathersby; making you guess there is a very high population density in this area.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(20, 1, 'Staid Avenue', 'Staid Avenue continues through the city here, and to the north you spot another intersection of roads.  There seems to be many buildings used as domiciles in this area.  You wouldn''t exactly call this neighborhood a ''bad'' neighborhood yet, but the potential is certainly there.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(21, 1, 'Harple Road', 'The street sign identifies this east-west road to be ''Harple Road''.  Staid Avenue leads south from here towards Trade Road and the southern section of Weathersby.  Harple Road seems to run along part of the length of the wall here, lined with apartment buildings and a few duplexed houses.  To the north is one particularly bad looking boarding house.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(22, 1, 'Harple Road', 'This, the western section of Harple Road, is bordered by the large, divided buildings and houses typical of this section of town.  Idly, you notice an amazing lack of the graffiti typical of other large, heavily populated towns in this neighborhood.  Seems as if the average populace in Weathersby is considerably more well-behaved than most other places.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(23, 1, 'Harple Road', 'Harple Road runs erratically here, the buildings on either side seemingly crooked and warped by the uneven ground.  Just to the west Staid Avenue runs south from Harple, towards Trade Road.  The neighborhood seems quiet, pensive even.  Strange...');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(24, 1, 'Harple Road', 'This is the western end of Harple Road.  To the west you can see where Staid Avenue leads south from Harple not far from here.  There doesn''t seem to be much going on around here at all...a very calm neighborhood.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(25, 1, 'Alley', 'This is a back alley near Harple Road that leads to the south.  A putrid stench puts you off for a moment; the alley is filled with garbage and human wastes.  Breathing through your mouth makes it a little more tolerable, but not by much. The buildings to either side of you rise high up, cutting off most of the light available from the sky.  The stench and darkness raises your hackles, and you shiver involuntarily.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(26, 1, 'Alley', 'This is the southern end of an alley that connects with Clay Street to the west.  An ill wind ruffles your hair from the north, carrying a smell of sewage, of rot, sickness and decomposition.  Your stomach squirms like a live animal in your gut, making you feel distinctly uneasy.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(27, 1, 'Staid Avenue', 'Staid Avenue heads south from Trade Road towards the river. The quality of the shops and buildings in this area are getting noticeably worse now, and the litter in the street is getting thicker the farther south you go.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(28, 1, 'Staid Avenue', 'You are near the southern end of Staid Road, where it intersects Klelk Boulevard near the river.  The neighborhood seems to be getting worse and worse the closer to the river you actually get.  Westwards is an open shop with a monstrous anchor out in front being used for a tethering post.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(29, 1, 'Klelk Boulevard', 'Klelk Boulevard runs here parallel to the river''s edge.  To the east you see the docks jutting out into the water, its wooden planks well-used by the crews of the trading ships that load and unload every day.  Lining the street on the north side of Klelk in both directions you see some of the typical dives you would expect close to the waterfront.  North from here Staid Avenue heads towards Trade Road and the central areas of the city.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(30, 1, 'Klelk Boulevard', 'The city wall begins here, where the river leads slightly away from the city.  The buildings here are run down, but not quite condemnable yet.  On the whole, this doesn''t seem like a nice place to live, no matter how busy the area seems to be. You would guess that the sailors of the river bring money to this section of town; hence all the low-grade shops and wayhouses, not to mention what might be called a ''tavern'' to the north.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(31, 1, 'Klelk Boulevard', 'Klelk Boulevard ends here, right up against the curving city wall.  The wall is especially high here, perhaps to protect the guards above from the rabble below.  The wall is stained with trash and refuse, obviously missed shots at the guards patrolling on the top...');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(32, 1, 'Alley', 'This small alley seems well traveled.  Tracks in the dust and dirt lead through, in all directions.  The alley continues to the west, and south you see Klelk Boulevard.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(33, 1, 'Alley', 'This is a small alley, dusty and forgotten.  A few lone pieces of assorted junk lie about, unwanted and thrown away.  This alley seems not to be travelled at all, or used for trash and refuse.  The path between buildings continues to the east, and west is an opening up onto Clay Street.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(34, 1, 'Klelk Boulevard', 'Klelk Boulevard ends here abruptly.  It appears that there used to be more to the east, but fresh mortarwork extends the wall right through the road.  To the south are the city docks, extending into the swiftly flowing river.  A lone ship sails past, travelling eastward and sporting a merchant''s flag.  A ruined building nearly stands to the north.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(35, 1, 'City Docks', 'Wooden gangways and delicate rope bridges form a warren of ship and dock, wood, water, and hemp.  The docks are small-- much too small for how many water vessels seem to pass by this way.  One large ship is docked well away from the other boats, an odd sigil marked on its side in the place of a name.  Its gangway is up right now, however...no entrance today.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(36, 1, 'Butcher''s Shop', 'A somewhat unpleasant smell strikes your nose as you walk through the open screen door of the shop.  The scent of raw meat, blood and sweat are clearly noticable; standard for a good butchery shop.  A glass panel separates you from a bewildering array of various slabs and shoulders of beef, deer, rabbit, and others.  A sign hangs nearby, wooden numbers hanging from it on a large nail...take a number?');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(37, 1, 'Jeweller''s', 'A few strategically placed lanterns keep this shop well-lit in the absence of any windows.  Surprisingly, there is only a plain bench with a few chairs in the shop; you see absolutely nothing to buy at all!  As you observe, it slowly becomes clear that this merchant''s method of sale consists of having the customer describe what they want, whereupon the merchant brings something similar from the back through a large metal door.  This strikes you as a fairly inefficient method of shop-keeping, but very secure.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(38, 1, 'Rubble and Ruin', 'You follow a path through the rocks and mortar to what was once the center of the church.  A few old apple cores and a broken wooden truck tell tales of this area being a playground for children now, but the ancient, broken stonework around stand testament to a better age.  For a moment, your imagination calls images forth of a dynamic pulpit and a full set of pews around you...images quickly shattered by the rustle of your clothing in the wind and a distant sound of someone talking on the street.  A sense of sadness consumes you as you stand here, and then you turn to go.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(39, 1, 'Brewery', 'Barrels of beer and wine are stacked high against the walls, waiting to be shipped out to various taverns everywhere.  The smell of fermentation is strong in the air, and the floorboards of the brewery are permanently stained brown and red from spilled alcohol.  Various signs with exciting slogans are scattered around the area, obviously for the worker''s morale.  You also notice that several barrels are open for the employees enjoyment.  The owner of this establishment is obviously no terrible taskmaster!');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(40, 1, 'Sakifan''s Library', 'BOOKS!    Shelves of books, piles of books, walls of books, stacks of books!  You boggle at the sheer mass of paper and ink surrounding you in this shopfor a moment.  If its in print, it''s probably here...somewhere.  Looking around, you realize that''s the key; finding what you need in these mounds and mountains of mess.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(41, 1, 'Leatherworker''s Shop', 'The smell of old tannin wafts through the air in this shop. A large countertop runs along the length of three of the interior walls, with many strips and bits of leather are lying on it in various stages of completion.  Many finished products are hanging on the walls, awaiting a buyer.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(42, 1, 'Ceran''s Contracting', 'This shop consists simply of a large wooden desk with a few chairs sitting in front of it.  Along the walls are renderings of many of Weathersby''s buildings, and in the corner is an easel with sketchings of what looks to be a large trade center.  The air seems cooler in here than anywhere else, and you feel a faint draft from the ceiling.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(43, 1, 'Boarding House', 'This room serves as the main entrance and lobby for this run-down boarding house.  The wooden walls are stained with rot and gods know what else.  The floors are even worse.  A rickety old desk stands near the interior door, with a torn and ripped ledger book on it.  The air seems humid with sweat and desperation, making you feel distinctly uncomfortable.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(44, 1, 'Hallway', 'This hallway runs the length of the building, with several small rooms splitting off from it at regular intervals.  One ajar door lends you a glimpse into the tiny, tiny rooms; room enough for one bed, one chest of drawers, and one person to stand.  There seems to be plenty of room for dirt and grunge, however.  Looking around, you see no reason to enter the tiny rooms.  The lobby to the south is the only exit of interest.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(45, 1, 'A private home', 'This seems to be a rather small, well-lived in home.  From the outside it looks exactly the same as many of the other houses typical of this neighborhood of Weathersby, but on the inside -- well, ok, it looks the same as all the rest.  You see the standard trappings of family life; a careworn rocking chair in the corner, and a small toy truck abandoned on a sitting table nearby.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(46, 1, 'A private home', 'This seems to be a rather small, well-lived in home.  From the outside it looks exactly the same as many of the other houses typical of this neighborhood of Weathersby, but on the inside -- well, ok, it looks the same as all the rest.  You see the standard trappings of family life; a stack of wood in the corner for the fireplace and a few loaves of dough wrapped in towels to rise.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(47, 1, 'A private home', 'This seems to be a rather small, well-lived in home.  From the outside it looks exactly the same as many of the other houses typical of this neighborhood of Weathersby, but on the inside -- well, ok, it looks the same as all the rest.  You see the standard trappings of family life; a plate with the crumbs of the last meal lying next to an overstuffed easy chair that faces the window and the street beyond.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(48, 1, 'A private home', 'This seems to be a rather small, well-lived in home.  From the outside it looks exactly the same as many of the other houses typical of this neighborhood of Weathersby, but on the inside -- well, ok, it looks the same as all the rest.  You see the standard trappings of family life; a half-played out game of checkers on a table between two stiff-backed wooden chairs.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(49, 1, 'A private home', 'This seems to be a rather small, well-lived in home.  From the outside it looks exactly the same as many of the other houses typical of this neighborhood of Weathersby, but on the inside -- well, ok, it looks the same as all the rest.  You see the standard trappings of family life; an empty bottle of ale holding a copy of the Weathersby city news down in the draft.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(50, 1, 'Ship Shop', 'This shop is decorated with ropes and nets along the walls. On second thought, you realize that the ropes and nets are the shop''s wares, along with many other various useful sailing equipment.  The shopkeeper''s desk is a simple plank of wood laid across two small anchors...this place has a definite atmosphere!  An open window to the river lets the breeze over the river into the shop to ruffle your hair gently.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(51, 1, 'Wrecked Shop', 'This appears to have once been some sort of dry-goods shop. You step lightly through the moldy spilled grain and broken barrels to take a look around.  This place has been utterly trashed; your first guess would be goblins, your second, drunk sailors.  Considering the area, drunk sailors would probably be your best bet.  There''s nothing here anymore, nothing at all.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(52, 1, 'Grebe''s Tavern', 'Dive.  That''s all you can say about this place; its a dive.  You step carefully over the dead and/or drunk sailors on the floor toward the center of the room.  There''s no decorations on the wall; they''ve all been ripped down in anger or for use as weapons at one time or another.  Looking around, you search vainly for a clean table to sit at, and then give up.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(53, 1, 'Trading Post', 'Bolts of cloth and barrels of spices stand in monstrous heaps, arranged in a chaotic haphazard of valuble trade goods. As you watch, a small man enters from the back room, grabs a box of something unidentifiable, and leaves just as quickly.  A schedule on the wall keeps track of the caravans and ships passing through the city -- very important information for a merchant.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(54, 1, 'A Well Travelled Road', 'This road still continues east to west, still an extremely wide road, made for heavy traffic.  To the east, the forest begins to close in on either side of the road, getting more and more thick the farther east you go.    A smaller road branches off to your north here, looking as if it is not a very popular road, probably made by some farmer or woodcutter who wanted easier access to his home.    West you can see the walled city of Weathersby.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(55, 1, 'A Shady Lane', 'This small two track leads into the woods from here, twisting and turning its way through the forest.  To the south, you may enter onto a well travelled road which runs east and west.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(56, 1, 'A Well Travelled Road', 'The main part of this road runs east to west, being wide enough to fit two carts abreast.  It is obvious from the amount of cart and wagon tracks you see that this is one of the main roads in the land.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(57, 1, 'The City Entrance', 'You stand at the eastern gate of the lands most well-loved city, Weathersby.  East leads you out into many uncharted and uncivilized lands, while to the west, the city awaits with all its splendor and majesty.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(58, 1, 'A Shady Lane', 'You are standing on a small, dusty path leading north into the forest.  South of you, the forest lightens and gives way to an open area, while to the north, the forest only gets deeper and darker.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(59, 1, 'Dirt Road', 'The road runs east into the wilderness.  Far off to the west you see the tops of the spires of the great city of Jareth.  A large house lies south of the path.  It looks very inviting.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(60, 1, 'Dirt Road', 'The road runs east and west, with a branch going north as well. The forest here seems more peaceful, more relaxing than the forest to the west.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(61, 1, 'Dirt Road', 'The road bends here, turning south.  This road seems to branch in many places, leading to many different realms of Dibrova.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(62, 1, 'On a Wide Road', 'The section of road is a bit smaller and less travelled than the one you just left, but it is easily passable just the same.  The forest around you seems alive with sound and motion.  It is really quite enjoyable.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(63, 1, 'Skirting the Clearing', 'The clearing still lies south of you, however the trail you follow leads away from clearing into deeper forest in an easterly direction.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(64, 1, 'Edge of a Clearing', 'Just to the south of you, the forest opens up into a large grassy field, it''s size undeterminable.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(65, 1, 'A Forest Trail', 'You stand on what is now just a trail through the deep dark forest.  You begin to hear the sound of animals making their way around you now that you are far away from any kind of civilization.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(66, 1, 'On the River', 'The river runs east to west here, the forest trail runs to the south.  To the north, you think you may see some sort of clearing, but no trail is visible form here. South lies nothing but deep forest.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(67, 1, 'A Dark and Gloomy Trail', 'The forest is dark, and becomes even darker the further you go in.  You can hear the sound of running water to the south, possibly it leads to a place a bit more bright and cheery than this...?');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(68, 1, 'On the River', 'It is quite dark here, with trees overhanging in many places.  The river continues east and west, with a bit more light to the east.  To the north, you think you may be able to see a trail, though from the looks of it, the trail is not used much at all.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(69, 1, 'On a Wide Road', 'The road continues north and south, the traffic seeming much heavier here. You feel no menace to your surroundings whatsoever, as people walk by and wave cheerily at you.  Even all the little forest animals seem undisturbed and unafraid.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(70, 1, 'On a Wide Road', 'The road makes a bend here, heading east and south.  The forest around you has been neatly cleared away from the path, all manner of obstruction removed. You must be near a large city.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(71, 1, 'A Wide Bend', 'The road bends west, with another bend directly following.  It also heads north for quite some distance.  The road seems to be very well-kept, which makes the travelling light and enjoyable.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(72, 1, 'A Wide Road', 'The road heads straight north and south from here.  People pass you by almost constantly, a flurry of activity now that you near Midgaard.  In fact, you can begin to see the spires of the city to northeast.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(73, 1, 'Large Field', 'You stand in a huge field of tall, brown grass.  The trail, still very discernable, heads in a southerly direction.  To the north, a large forest begins.  Under your feet, the road is very hard-packed, as if many heavy feet pass this way regularly.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(74, 1, 'The Dark Forest', 'You stomp your way through the underbrush.  The dark forest of Miden''nir get pretty thick here, and the branches high above your head are so thick that they block out all direct sunlight.  While it is much too thick to go further east, you might be able to make your way though the forest to the south and west.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(75, 1, 'Old Stone Road', 'The forest trail runs north to south here, a much more travelled road to the south.  Under the dirt and loose gravel you think you may be able to see some old stones which might have formed a well used road long ago.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(76, 1, 'Deep In The Forest Of Miden''nir', 'There is a sickening stench here.  It smells of blood and death. To the north, you catch glimpses of daylight.  To the east, you simply cannot see.  The trees are close and stifling.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(77, 1, 'Grassy Field', 'This field seems to go on forever.  The trail continues north and south of here.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(78, 1, 'Along the Cliff', 'The trail runs along the top of the cliff, east to west.  This seems to be an extremely well-travelled trail, so much that it could more correctly be called a road.  Most of the tracks seem to head east, so you must assume that there is something of interest in that direction, however it is hard to see over the hill which you are slowly climbing.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(79, 1, 'Near the Bridges', 'Your trek has brought you just west of the twin bridges which lead over the ocean into the great city so near.  You hear the sounds of a large city quite clearly from where you stand.  It seems almost out of place, standing here in the outdoor setting, hearing all the hustle and bustle of the city so near.  You may head to the toll booths just east of here, or leave to the west into the wilderness.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(80, 1, 'Toll Booth', 'You stand at the first of two toll booths which block the way onto the bridges leading into McGintey Cove.   The second, just east of you, looks just as busy as this one.  If you are not sure what to do next, possibly reading the sign on the outside of the booth may give you some idea.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(81, 1, 'Between the Booths', 'You stand between two toll booths, both of which block the way into the city.  Take your pick, east or west.  There also seems to be some sort of park to the south, and to the north is an enormous stable, where alll steeds are kept, since none are allowed in the city.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(82, 1, 'Eastern Toll Booth', 'You stand at the easternmost toll booth which allows access to the great seaport city, McGintey Cove.  If you are not quite sure how to gain access, just read the sign posted on te outer wall of teh booth.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(83, 1, 'Stables', 'This building is so large, has so many rows of stalls running back into it''s depths, you cannot imagine trying to find any one particular within this building.  A sign is posted to your left, explaining the stable rules.  The exit is to your south.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(84, 1, 'Memorial Park', 'This tranquil spot perched on the edge of the cliffside offers a beautiful vantage point from which to admire the city.  The grass is extremely well kept, the benches all freshly painted and clean.  A large sign dedicates this park to the founder of the city.  You may enter back onto McGintey Road to the north, or rest here for a while before continuing on your way.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(85, 1, 'A Fork in the Road', 'You may head east, west, or north along trails that all look to be about the same in terms of ease of travel.  There looks to be a bit more traffic to the north, however the trails leading east and west don''t exactly look unused themselves.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(86, 1, 'Forest Road', 'You are in an unfinished room.  The road runs east into thick, green woodlands where it seems the trees are almost alive from the way the playful breeze ruffles through their branches.  To the west the road splits to the north ro continues its way west.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(87, 1, 'A Wide Road', 'The road turns here heading east and south.  It seems to run quite some way to the east, however to the south it makes another turn.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(88, 1, 'Near a Cliff', 'You still stand in the grassy field, however directly south of you the world seems to end!  It looks as if there may be a large cliff to your south, and from the smell of things, it must lead down to the sea.  To the north, the field runs for some distance until reaching a far-off forest.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(89, 1, 'A Wide Road Near the City', 'Midgaard lies just northeast of you, a hop, skip, and jump away.  To the south is a long, wide road which is filled with traffic at all ours of the day and night, it being the main route from Midgaard to Jareth, the Free City.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(90, 1, 'Just West of Midgaard', 'Just east of you lie the gates to the city of Midgaard.  To the south begins the way down a long, wide, and extremely well traveled road.  To the west you may enter into a large forest.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(91, 1, 'A Tunnel In The Mountains', 'The tunnel gets lighter to the north, presumably leading out, while to the south the passage gets smaller and smaller.  A small alcove has been carved into the east wall.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(92, 1, 'Diminishing Trail', 'The trail looks like it may be bending here, however with the way all the vegetation is growing in on the trail, it is hard to tell.  It looks like the trail may head east and north from here.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(93, 1, 'Faint Trail', 'The trail is so faint now that you can barely see it.  You almost have to hope you are heading in the right direction, through openings in trees and such, so faint is the trail.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(94, 1, 'A Dead End', 'The trail leads into a huge wall made of shrubbery easily 10 feet high. You wont be going any further this way.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(95, 1, 'Old Stone Road', 'The road runs north and south still.  There is a large gate, closed, way off to the north, a large building just beyond the gates.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(96, 1, 'Cliff''s Edge', 'The trail still runs east to west here along the rim of the drop off, but in looking south over the edge, you see the someone has made some crude steps that run along the wall of the cliff.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(97, 1, 'Old Stone Road', 'The road runs north up to a large iron gate, a gate which closes the way to what appears to be an old cathedral or church.  To the south the road heads back toward the main forest trail.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(98, 1, 'Cliff''s Edge', 'The trail runs right up to the edge of the cliff and T''s off, running east and west from here.  Looking down and over the edge of the cliff, you find that you were right about what lies at the bottom of the cliff, the sea!    To the north the trail cuts through a huge grassy field.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(99, 1, 'On a Rock Landing', 'This rudely carved rock landing marks the start of a long flight of steps leading down to the sea shore.  The footing looks to be treacherous at best, so be careful!');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(100, 1, 'The Ambush Point', 'This is a overgrown foot-trail south of the Inn.  It leads west, but you would be hard pressed to follow it far.  A number of bushes are trampled on and some medium sized branches have been knocked down.  Obviously there has been a battle here rather recently.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(101, 1, 'The Bar', 'This is where people use to come and enjoy the food, drink and hospitality of the Innkeeper, but as of late, he only serves the few adventurers that manage to survive a trip through the forest. You can leave south and return to the common room.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(102, 1, 'Along the Cliff', 'Heading east, the trail tops a large rise, continuing it''s way toward whatever lies in that direction.  To the west, the trail heads it way along, not really really seeming to head anywhere.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(103, 1, 'Atop the Hill', 'Wonder of wonders!  You feel as if you could see the whole world from where you currently stand.  Just east of you, sitting proudly on a large rock, almost a plateau, is the huge seaport town of McGintey Cove.  The twin bridges which lead from the edge of the cliff to the city gate are the only entrance or exit from the town. The trail wlso runs west far into the distance, skirting the southern edge of a huge field, which seems t go on forever.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(104, 1, 'West of Town', 'The road runs east toward the huge seaport atop the plateau and west, topping a tall hill.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(105, 1, 'A Dark and Gloomy Trail', 'The forest around you is so thick now that there is not enough light to see by.  The birds you heard singing before are now quiet, all you can hear now is the sound of your own heavy breathing.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(106, 1, 'A Dark and Gloomy Trail', 'Impenetrable is the only word you could use to describe the darkness surrounding you.  Trees brush against you on either side, feeling almost like they tug at your clothing. The trail seems to continue north, although it seems much more sensible to go south.');
INSERT INTO [Rooms] ([RoomID], [AreaID], [Name], [Description])
VALUES(107, 1, 'A Dead End Trail', 'The path comes to an abrupt end here, as the trees close around you and make any further exploration impossible. It does seem strabge, though, that a trail through the woods would end so abruptly.');

-- Begin inserting data in Skills

-- Begin inserting data in SystemLog

-- Begin inserting data in sqlite_sequence
INSERT INTO [sqlite_sequence] ([name], [seq])
VALUES('ANSI', '26');
INSERT INTO [sqlite_sequence] ([name], [seq])
VALUES('Areas', '1');
INSERT INTO [sqlite_sequence] ([name], [seq])
VALUES('Exits', '110');
INSERT INTO [sqlite_sequence] ([name], [seq])
VALUES('IAC', '5');
INSERT INTO [sqlite_sequence] ([name], [seq])
VALUES('MXP', '4');
INSERT INTO [sqlite_sequence] ([name], [seq])
VALUES('Roles', '13');
INSERT INTO [sqlite_sequence] ([name], [seq])
VALUES('Rooms', '107');
INSERT INTO [sqlite_sequence] ([name], [seq])
VALUES('Mobs', '1');
INSERT INTO [sqlite_sequence] ([name], [seq])
VALUES('PortalExits', '1');
