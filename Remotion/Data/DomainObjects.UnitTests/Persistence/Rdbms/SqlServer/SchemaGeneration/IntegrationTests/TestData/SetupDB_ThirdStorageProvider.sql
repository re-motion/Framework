USE DBPrefix_SchemaGenerationTestDomain3
-- Create all tables
CREATE TABLE [dbo].[InheritanceRoot]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [PropertyAboveInheritanceRoot] nvarchar (max) NULL,
  [PropertyInheritanceRoot] nvarchar (max) NULL,
  [PropertyBelowInheritanceRoot] nvarchar (max) NULL,
  CONSTRAINT [PK_InheritanceRoot] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[IndexTestTable]
(
  [ObjectID] integer NOT NULL,
  [ClassID] varchar NOT NULL,
  [Timestamp] datetime2 NULL,
  [ID] uniqueidentifier NOT NULL,
  [FirstName] varchar(100) NOT NULL,
  [LastName] varchar(100) NOT NULL,
  [XmlColumn1] xml NOT NULL,
  CONSTRAINT [PK_IndexTestTable_ID] PRIMARY KEY CLUSTERED ([ID])
)
CREATE TABLE [dbo].[PKTestTable]
(
  [ObjectID] integer NOT NULL,
  [ClassID] varchar NOT NULL,
  [Timestamp] datetime2 NULL,
  [ID] uniqueidentifier NOT NULL,
  [Name] varchar(100) NOT NULL,
  CONSTRAINT [PK_PKTestTable_ID] PRIMARY KEY NONCLUSTERED ([ID])
)
-- Create foreign key constraints for tables that were created above
-- Create a view for every class
GO
CREATE VIEW [dbo].[AboveInheritanceRootView] ([ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot])
  AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot]
    FROM [dbo].[InheritanceRoot]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[BelowInheritanceRootView] ([ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot])
  AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot]
    FROM [dbo].[InheritanceRoot]
    WHERE [ClassID] IN ('BelowInheritanceRoot')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[NewViewName] ([ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot])
  AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot]
    FROM [dbo].[InheritanceRoot]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[AddedView] ([ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot])
  AS
  SELECT [ID], [ClassID], [Timestamp], [PropertyAboveInheritanceRoot], [PropertyInheritanceRoot], [PropertyBelowInheritanceRoot]
    FROM [dbo].[InheritanceRoot]
    WHERE [ClassID] IN ('ClassID')
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[IndexTestView] ([ObjectID], [ClassID], [Timestamp], [ID], [FirstName], [LastName], [XmlColumn1])
  AS
  SELECT [ObjectID], [ClassID], [Timestamp], [ID], [FirstName], [LastName], [XmlColumn1]
    FROM [dbo].[IndexTestTable]
  WITH CHECK OPTION
GO
CREATE VIEW [dbo].[PKTestView] ([ObjectID], [ClassID], [Timestamp], [ID], [Name])
  AS
  SELECT [ObjectID], [ClassID], [Timestamp], [ID], [Name]
    FROM [dbo].[PKTestTable]
  WITH CHECK OPTION
GO
-- Create indexes for tables that were created above
CREATE UNIQUE NONCLUSTERED INDEX [IDX_NonClusteredUniqueIndex]
  ON [dbo].[IndexTestTable] ([ID])
  WITH (IGNORE_DUP_KEY = ON, ONLINE = OFF)
CREATE NONCLUSTERED INDEX [IDX_NonClusteredNonUniqueIndex]
  ON [dbo].[IndexTestTable] ([FirstName], [LastName])
  INCLUDE ([ID])
  WITH (IGNORE_DUP_KEY = OFF, ONLINE = OFF)
CREATE UNIQUE NONCLUSTERED INDEX [IDX_IndexWithSeveralOptions]
  ON [dbo].[IndexTestTable] ([FirstName] DESC)
  WITH (IGNORE_DUP_KEY = ON, ONLINE = OFF, PAD_INDEX = ON, FILLFACTOR = 5, SORT_IN_TEMPDB = ON, STATISTICS_NORECOMPUTE = ON, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, MAXDOP = 2)
CREATE PRIMARY XML INDEX [IDX_PrimaryXmlIndex]
  ON [dbo].[IndexTestTable] ([XmlColumn1])
  WITH (PAD_INDEX = ON, FILLFACTOR = 3, SORT_IN_TEMPDB = ON, STATISTICS_NORECOMPUTE = ON, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, MAXDOP = 2)
CREATE XML INDEX [IDX_SecondaryXmlIndex1]
  ON [dbo].[IndexTestTable] ([XmlColumn1])
  USING XML INDEX [IDX_PrimaryXmlIndex]
  FOR Path
  WITH (PAD_INDEX = ON, SORT_IN_TEMPDB = OFF, STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS = OFF, ALLOW_PAGE_LOCKS = OFF)
CREATE XML INDEX [IDX_SecondaryXmlIndex2]
  ON [dbo].[IndexTestTable] ([XmlColumn1])
  USING XML INDEX [IDX_PrimaryXmlIndex]
  FOR Value
  WITH (PAD_INDEX = OFF, FILLFACTOR = 8, SORT_IN_TEMPDB = ON)
CREATE XML INDEX [IDX_SecondaryXmlIndex3]
  ON [dbo].[IndexTestTable] ([XmlColumn1])
  USING XML INDEX [IDX_PrimaryXmlIndex]
  FOR Property
  WITH (STATISTICS_NORECOMPUTE = ON, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = OFF)
CREATE UNIQUE CLUSTERED INDEX [IDX_ClusteredUniqueIndex]
  ON [dbo].[PKTestTable] ([Name])
  WITH (IGNORE_DUP_KEY = ON, ONLINE = OFF)
-- Create synonyms for tables that were created above
CREATE SYNONYM [dbo].[AddedViewSynonym] FOR [dbo].[AddedView]
