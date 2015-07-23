
CREATE DATABASE RpaTest
ON PRIMARY (
	Name = 'RpaTest_Data',
	Filename = 'C:\Databases\RpaTest.mdf',
	Size = 10MB
)
LOG ON (
	Name = 'RpaTest_Log',
	Filename = 'C:\Databases\RpaTest.ldf',
	Size = 10MB	
)
GO

ALTER DATABASE RpaTest SET RECOVERY SIMPLE
GO