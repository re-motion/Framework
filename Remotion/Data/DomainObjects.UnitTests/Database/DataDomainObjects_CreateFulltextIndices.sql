USE DBPrefix_TestDomain

EXEC sp_fulltext_database 'enable'
CREATE FULLTEXT CATALOG [DBPrefix_TestDomain_FT] IN PATH 'C:\Databases\ftdata'
CREATE UNIQUE CLUSTERED INDEX [IX_CeoView_ID] ON CeoView(ID)
CREATE FULLTEXT INDEX ON [CeoView]([Name]) KEY INDEX [IX_CeoView_ID] ON [DBPrefix_TestDomain_FT]