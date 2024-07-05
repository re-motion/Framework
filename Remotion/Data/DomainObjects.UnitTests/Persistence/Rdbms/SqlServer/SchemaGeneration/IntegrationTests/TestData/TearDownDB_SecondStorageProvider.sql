USE DBPrefix_SchemaGenerationTestDomain2
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
DROP TYPE IF EXISTS [dbo].[TVP_Guid]
DROP TYPE IF EXISTS [dbo].[TVP_Guid_Distinct]
-- Drop all synonyms
-- Drop all indexes
-- Drop all views
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OfficialView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[OfficialView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SpecialOfficialView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SpecialOfficialView]
-- Drop foreign keys of all tables
-- Drop all tables
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Official' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Official]
