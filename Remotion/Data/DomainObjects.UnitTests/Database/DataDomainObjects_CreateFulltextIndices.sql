USE TestDomain

EXEC sp_fulltext_database 'enable'
CREATE FULLTEXT CATALOG [TestDomain_FT] IN PATH 'C:\Databases\ftdata'
CREATE UNIQUE CLUSTERED INDEX [IX_CeoView_ID] ON CeoView(ID)
CREATE FULLTEXT INDEX ON [CeoView]([Name]) KEY INDEX [IX_CeoView_ID] ON [TestDomain_FT]