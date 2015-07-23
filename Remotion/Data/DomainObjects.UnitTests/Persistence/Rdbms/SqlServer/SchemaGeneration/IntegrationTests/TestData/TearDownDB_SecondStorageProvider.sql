USE SchemaGenerationTestDomain2
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
