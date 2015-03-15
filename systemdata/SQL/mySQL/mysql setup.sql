CREATE TABLE `ANSI` (
ID INTEGER  PRIMARY KEY  NOT NULL AUTO_INCREMENT,
EscapeCode NVARCHAR(50)  NULL,
Tag nvarchar(25)  NULL
);


CREATE TABLE Areas (
  ID INTEGER PRIMARY KEY NOT NULL AUTO_INCREMENT, 
  UID NVARCHAR(50), 
  Name nvarchar(45));


CREATE TABLE BannedIPAddresses (
ID INTEGER  PRIMARY KEY  NOT NULL AUTO_INCREMENT,
StartIPAddress nvarchar(20)  NOT NULL,
EndIPAddress nvarchar(20)  NOT NULL,
Note nvarchar(255)  NOT NULL,
BannedByPlayerID INTEGER  NOT NULL,
BannedDateTime DATE  NOT NULL
);


CREATE TABLE Doors (
ID INTEGER  PRIMARY KEY  NOT NULL AUTO_INCREMENT,
DoorSideAID INTEGER  NULL,
DoorSideBID INTEGER  NULL,
OpenState INTEGER  NULL,
Name nvarchar(45)  NULL,
Description nvarchar(1000)  NULL
);


CREATE TABLE DoorSides (
ID INTEGER  PRIMARY KEY  NOT NULL AUTO_INCREMENT,
Name nvarchar(45)  NULL,
Description nvarchar(1000)  NULL
);


CREATE TABLE Exits (
ID INTEGER  PRIMARY KEY  NOT NULL AUTO_INCREMENT,
ExitRoomAID INTEGER  NULL,
DirectionA nvarchar(50)  NULL,
ExitRoomBID INTEGER  NULL,
DirectionB nvarchar(50)  NULL,
DoorID INTEGER  NULL
);


CREATE TABLE HelpTopicAliases (
ID INTEGER NOT NULL PRIMARY KEY AUTO_INCREMENT,
HelpTopicAlias nvarchar(50) NOT NULL,
HelpTopicID INTEGER NULL
);


CREATE TABLE HelpTopics (
ID INTEGER  PRIMARY KEY  NOT NULL AUTO_INCREMENT,
HelpTopic nvarchar(50)  NOT NULL,
`Usage` nvarchar(255)  NULL,
Description nvarchar(1000)  NULL,
Example nvarchar(1000)  NULL,
SeeAlso nvarchar(1000)  NULL,
ViewTemplate nvarchar(255)  NULL
);

CREATE TABLE IAC (
ID INTEGER  NOT NULL PRIMARY KEY AUTO_INCREMENT,
Name nvarchar(50)  NULL,
OptionCode int  NULL,
NegotiateAtConnect bit  NOT NULL,
RequiresSubNegotiation bit  NOT NULL,
SubNegAssembly nvarchar(50)  NULL,
NegotiationStartValue nvarchar(50)  NULL
);

CREATE TABLE Mobs (
ID INTEGER  PRIMARY KEY  NOT NULL AUTO_INCREMENT,
MobTypeID int  NULL,
Name nvarchar(45)  NULL,
Title nvarchar(50)  NULL,
Description nvarchar(250)  NULL,
Age int  NULL,
CurrentRoomID int  NULL,
Prompt nvarchar(50)  NULL,
CreateDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
);


CREATE TABLE MobTypes (
ID INTEGER  NOT NULL PRIMARY KEY AUTO_INCREMENT,
MobTypeName NVARCHAR(50)  NOT NULL
);


CREATE TABLE MudChannelRoles (
ID INTEGER  NOT NULL PRIMARY KEY AUTO_INCREMENT,
MudChannelID INTEGER  NOT NULL,
RoleID INTEGER  NOT NULL
);


CREATE TABLE MudChannels (
ID INTEGER  NOT NULL PRIMARY KEY AUTO_INCREMENT,
MudChannelName NVARCHAR(50)  NOT NULL
);


CREATE TABLE MXP (
ID INTEGER  PRIMARY KEY  NOT NULL AUTO_INCREMENT,
ElementName nvarchar(50)  NULL,
ElementDefinition nvarchar(50)  NOT NULL
);


CREATE TABLE PlayerChannels (
ID INTEGER  NOT NULL PRIMARY KEY AUTO_INCREMENT,
PlayerID INTEGER  NOT NULL,
ChannelID INTEGER  NOT NULL
);


CREATE TABLE PlayerIPAddress (
    ID integer PRIMARY KEY  NOT NULL AUTO_INCREMENT,
    PlayerID integer NOT NULL,
    IPAddress nvarchar(50) NOT NULL
);


CREATE TABLE PlayerRoles (
ID INTEGER  NOT NULL PRIMARY KEY AUTO_INCREMENT,
PlayerID INTEGER  NULL,
RoleID INTEGER  NULL
);


CREATE TABLE Players (
  ID integer NOT NULL PRIMARY KEY AUTO_INCREMENT, 
  UserName nvarchar(45) NOT NULL, 
  Password nvarchar(45) NOT NULL, 
  DisplayName nvarchar(50), 
  Suffix nvarchar(45), 
  Prefix nvarchar(45), 
  Title nvarchar(50), 
  Description nvarchar(250), 
  Age int, 
  CreateDate nvarchar(50), 
  CurrentRoomID integer, 
  Prompt nvarchar(50), 
  WantAnsi bit, 
  WantMXP bit, 
  WantMCCP bit, 
  LastLogin nvarchar(50), 
  LastLogout nvarchar(50), 
  LastIPAddress nvarchar(50), 
  Email nvarchar(100), 
  HomePage nvarchar(4000), 
  PlanText nvarchar(4000), 
  BufferLength int NOT NULL);


CREATE TABLE PortalExits (
ID INTEGER  NOT NULL PRIMARY KEY AUTO_INCREMENT,
PortalID INTEGER  NOT NULL,
RoomAID INTEGER  NULL,
RoomBID INTEGER  NULL
);


CREATE TABLE Roles (
ID INTEGER  NOT NULL PRIMARY KEY AUTO_INCREMENT,
Name nvarchar(50)  NOT NULL,
SecurityRoleMask int  NOT NULL
);


CREATE TABLE Rooms (
  ID INTEGER NOT NULL PRIMARY KEY AUTO_INCREMENT, 
  UID NVARCHAR(50), 
  AreaID INTEGER, 
  Name nvarchar(45) NOT NULL, 
  Description nvarchar(1000), 
  RoomTypeID INTEGER);


CREATE TABLE RoomTypes (
ID INTEGER  NOT NULL PRIMARY KEY AUTO_INCREMENT,
Name nvarchar(50)  NOT NULL,
Description nvarchar(1000)  NULL
);


CREATE TABLE RoomVisuals (
  ID INTEGER NOT NULL PRIMARY KEY AUTO_INCREMENT, 
  RoomID INTEGER NOT NULL, 
  Name NVARCHAR(59) NOT NULL, 
  Description NVARCHAR(255) NOT NULL);


CREATE TABLE Typos (
ID INTEGER  PRIMARY KEY  NOT NULL AUTO_INCREMENT,
Note nvarchar(500)  NOT NULL,
SubmittedByPlayerID INTEGER  NOT NULL,
RoomID INTEGER  NOT NULL,
SubmittedDateTime nvarchar(30)  NOT NULL,
Resolved BOOLEAN NOT NULL,
ResolvedByPlayerID INTEGER  NULL,
ResolvedDateTime nvarchar(30)  NULL
);