USE RemotionSecurityManagerWebClientTest
GO

-- Drop all structured types
DROP TYPE IF EXISTS [dbo].[TVP_String]
DROP TYPE IF EXISTS [dbo].[TVP_Binary]
DROP TYPE IF EXISTS [dbo].[TVP_AnsiString]
DROP TYPE IF EXISTS [dbo].[TVP_Boolean]
DROP TYPE IF EXISTS [dbo].[TVP_Boolean_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Byte]
DROP TYPE IF EXISTS [dbo].[TVP_Byte_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Int16]
DROP TYPE IF EXISTS [dbo].[TVP_Int16_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Int32]
DROP TYPE IF EXISTS [dbo].[TVP_Int32_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Int64]
DROP TYPE IF EXISTS [dbo].[TVP_Int64_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Decimal]
DROP TYPE IF EXISTS [dbo].[TVP_Decimal_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Single]
DROP TYPE IF EXISTS [dbo].[TVP_Single_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Double]
DROP TYPE IF EXISTS [dbo].[TVP_Double_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_DateTime2]
DROP TYPE IF EXISTS [dbo].[TVP_DateTime2_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Date]
DROP TYPE IF EXISTS [dbo].[TVP_Date_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Guid]
DROP TYPE IF EXISTS [dbo].[TVP_Guid_Distinct]

-- Drop all views that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FileView')
  DROP VIEW [dbo].[FileView]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FileItemView')
  DROP VIEW [dbo].[FileItemView]
GO

-- Drop foreign keys of all tables that will be created below
DECLARE @statement nvarchar (4000)
SET @statement = ''
SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' 
    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id 
    WHERE fk.xtype = 'F' AND t.name IN ('File', 'FileItem')
    ORDER BY t.name, fk.name
exec sp_executesql @statement
GO

-- Drop all tables that will be created below
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'File')
  DROP TABLE [dbo].[File]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'FileItem')
  DROP TABLE [dbo].[FileItem]
GO
