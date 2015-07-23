USE <Database>
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Revision')
  DROP TABLE [dbo].[Revision]

GO
