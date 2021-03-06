USE PerformanceTestDomain

DELETE FROM [dbo].[Person]
DELETE FROM [dbo].[Company]
DELETE FROM [dbo].[File]
DELETE FROM [dbo].[Client]


INSERT INTO [dbo].[Client] ([ID], [ClassID], [Name]) VALUES ('6F20355F-FA99-4c4e-B432-02C41F7BD390', 'Client', 'TestClient')

declare @number int
set @number = 0

while @number < 6000
begin
  set @number = @number + 1
  INSERT INTO [dbo].[File] ([ID], [ClassID], [Number], [ClientID]) VALUES (NEWID(), 'File', 'File '+ cast (@number as varchar), '6F20355F-FA99-4c4e-B432-02C41F7BD390')
end

set @number = 0

while @number < 2000
begin
  set @number = @number + 1
  INSERT INTO [dbo].[Person] ([ID], [ClassID], [FirstName], [LastName], [ClientID]) VALUES (NEWID(), 'Person', 'Hans', 'Huber '+ cast (@number as varchar), '6F20355F-FA99-4c4e-B432-02C41F7BD390')
end

set @number = 0

while @number < 2000
begin
	set @number = @number + 1
	INSERT INTO [dbo].[Company] ([ID], [ClassID], [Name], [ClientID]) VALUES (NEWID(), 'Company', 'Company ' + cast (@number as varchar), '6F20355F-FA99-4c4e-B432-02C41F7BD390');
end


--declare @clientID uniqueidentifier
--declare @count int
--set @count = 0
--
--while @count < 50
--begin
--  set @count = @count + 1
--
--  set @clientID = newid ()
--  INSERT INTO [dbo].[Client] ([ID], [ClassID], [Name]) VALUES (@clientID, 'Client', 'TestClient')
--
--  declare @number1 int
--  set @number1 = 0
--
--  while @number1 < 3000
--  begin
--    set @number1 = @number1 + 1
--    INSERT INTO [dbo].[File] ([ID], [ClassID], [Number], [Client]) VALUES (NEWID(), 'File', 'File '+ cast (@number1 as varchar), @clientID)
--  end
--end


