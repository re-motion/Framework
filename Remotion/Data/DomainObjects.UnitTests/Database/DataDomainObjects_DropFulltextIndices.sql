USE TestDomain

IF EXISTS (SELECT * FROM sys.fulltext_indexes INNER JOIN sys.fulltext_catalogs ON sys.fulltext_indexes.fulltext_catalog_id = sys.fulltext_catalogs.fulltext_catalog_id WHERE [Name] = 'TestDomain_FT')
BEGIN
  DROP FULLTEXT INDEX ON [CeoView]
END 
GO

IF EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_CeoView_ID')
  DROP INDEX IX_CeoView_ID ON CeoView;
GO

IF EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE [Name] = 'TestDomain_FT')
BEGIN
  DROP FULLTEXT CATALOG [TestDomain_FT]
END
GO
