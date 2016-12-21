USE RemotionSecurityManager
GO

ALTER TABLE [Tenant]
  ADD CONSTRAINT TenantUniqueIdentifier UNIQUE NONCLUSTERED ([UniqueIdentifier])

ALTER TABLE [User]
  ADD CONSTRAINT UniqueUserName UNIQUE NONCLUSTERED ([UserName])

ALTER TABLE [Group]
  ADD CONSTRAINT GroupUniqueIdentifier UNIQUE NONCLUSTERED ([UniqueIdentifier])

ALTER TABLE [Position]
  ADD CONSTRAINT PositionUniqueIdentifier UNIQUE NONCLUSTERED ([UniqueIdentifier])

ALTER TABLE [Culture]
  ADD CONSTRAINT CultureUniqueIdentifier UNIQUE NONCLUSTERED ([CultureName])
