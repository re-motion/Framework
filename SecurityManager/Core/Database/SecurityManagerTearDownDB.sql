USE RemotionSecurityManager
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
DROP TYPE IF EXISTS [dbo].[TVP_DateTime]
DROP TYPE IF EXISTS [dbo].[TVP_DateTime_Distinct]
DROP TYPE IF EXISTS [dbo].[TVP_Guid]
DROP TYPE IF EXISTS [dbo].[TVP_Guid_Distinct]
-- Drop all synonyms
-- Drop all indexes
-- Drop all views
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AccessControlEntryView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AccessControlEntryView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PermissionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[PermissionView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StateCombinationView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[StateCombinationView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AccessControlListView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AccessControlListView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StatefulAccessControlListView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[StatefulAccessControlListView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StatelessAccessControlListView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[StatelessAccessControlListView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StateUsageView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[StateUsageView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'MetadataObjectView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[MetadataObjectView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EnumValueDefinitionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[EnumValueDefinitionView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AbstractRoleDefinitionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AbstractRoleDefinitionView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AccessTypeDefinitionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AccessTypeDefinitionView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'AccessTypeReferenceView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[AccessTypeReferenceView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CultureView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[CultureView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'LocalizedNameView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[LocalizedNameView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SecurableClassDefinitionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SecurableClassDefinitionView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StateDefinitionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[StateDefinitionView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StatePropertyDefinitionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[StatePropertyDefinitionView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'StatePropertyReferenceView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[StatePropertyReferenceView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'GroupView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[GroupView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'GroupTypeView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[GroupTypeView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'GroupTypePositionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[GroupTypePositionView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'PositionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[PositionView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'RoleView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[RoleView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'SubstitutionView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[SubstitutionView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'TenantView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[TenantView]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'UserView' AND TABLE_SCHEMA = 'dbo')
  DROP VIEW [dbo].[UserView]
-- Drop foreign keys of all tables
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_AccessControlEntry_SpecificTenantID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'AccessControlEntry')
  ALTER TABLE [dbo].[AccessControlEntry] DROP CONSTRAINT FK_AccessControlEntry_SpecificTenantID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_AccessControlEntry_SpecificGroupID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'AccessControlEntry')
  ALTER TABLE [dbo].[AccessControlEntry] DROP CONSTRAINT FK_AccessControlEntry_SpecificGroupID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_AccessControlEntry_SpecificGroupTypeID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'AccessControlEntry')
  ALTER TABLE [dbo].[AccessControlEntry] DROP CONSTRAINT FK_AccessControlEntry_SpecificGroupTypeID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_AccessControlEntry_SpecificPositionID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'AccessControlEntry')
  ALTER TABLE [dbo].[AccessControlEntry] DROP CONSTRAINT FK_AccessControlEntry_SpecificPositionID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_AccessControlEntry_SpecificUserID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'AccessControlEntry')
  ALTER TABLE [dbo].[AccessControlEntry] DROP CONSTRAINT FK_AccessControlEntry_SpecificUserID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_AccessControlEntry_SpecificAbstractRoleID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'AccessControlEntry')
  ALTER TABLE [dbo].[AccessControlEntry] DROP CONSTRAINT FK_AccessControlEntry_SpecificAbstractRoleID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_AccessControlEntry_AccessControlListID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'AccessControlEntry')
  ALTER TABLE [dbo].[AccessControlEntry] DROP CONSTRAINT FK_AccessControlEntry_AccessControlListID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Permission_AccessTypeDefinitionID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Permission')
  ALTER TABLE [dbo].[Permission] DROP CONSTRAINT FK_Permission_AccessTypeDefinitionID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Permission_AccessControlEntryID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Permission')
  ALTER TABLE [dbo].[Permission] DROP CONSTRAINT FK_Permission_AccessControlEntryID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_StateCombination_AccessControlListID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'StateCombination')
  ALTER TABLE [dbo].[StateCombination] DROP CONSTRAINT FK_StateCombination_AccessControlListID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_AccessControlList_StatefulAcl_ClassID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'AccessControlList')
  ALTER TABLE [dbo].[AccessControlList] DROP CONSTRAINT FK_AccessControlList_StatefulAcl_ClassID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_AccessControlList_StatelessAcl_ClassID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'AccessControlList')
  ALTER TABLE [dbo].[AccessControlList] DROP CONSTRAINT FK_AccessControlList_StatelessAcl_ClassID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_StateUsage_StateDefinitionID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'StateUsage')
  ALTER TABLE [dbo].[StateUsage] DROP CONSTRAINT FK_StateUsage_StateDefinitionID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_StateUsage_StateCombinationID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'StateUsage')
  ALTER TABLE [dbo].[StateUsage] DROP CONSTRAINT FK_StateUsage_StateCombinationID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_EnumValueDefinition_StatePropertyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'EnumValueDefinition')
  ALTER TABLE [dbo].[EnumValueDefinition] DROP CONSTRAINT FK_EnumValueDefinition_StatePropertyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_AccessTypeReference_SecurableClassID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'AccessTypeReference')
  ALTER TABLE [dbo].[AccessTypeReference] DROP CONSTRAINT FK_AccessTypeReference_SecurableClassID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_AccessTypeReference_AccessTypeID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'AccessTypeReference')
  ALTER TABLE [dbo].[AccessTypeReference] DROP CONSTRAINT FK_AccessTypeReference_AccessTypeID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_LocalizedName_CultureID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'LocalizedName')
  ALTER TABLE [dbo].[LocalizedName] DROP CONSTRAINT FK_LocalizedName_CultureID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_SecurableClassDefinition_BaseSecurableClassID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'SecurableClassDefinition')
  ALTER TABLE [dbo].[SecurableClassDefinition] DROP CONSTRAINT FK_SecurableClassDefinition_BaseSecurableClassID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_StatePropertyReference_SecurableClassID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'StatePropertyReference')
  ALTER TABLE [dbo].[StatePropertyReference] DROP CONSTRAINT FK_StatePropertyReference_SecurableClassID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_StatePropertyReference_StatePropertyID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'StatePropertyReference')
  ALTER TABLE [dbo].[StatePropertyReference] DROP CONSTRAINT FK_StatePropertyReference_StatePropertyID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Group_TenantID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Group')
  ALTER TABLE [dbo].[Group] DROP CONSTRAINT FK_Group_TenantID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Group_ParentID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Group')
  ALTER TABLE [dbo].[Group] DROP CONSTRAINT FK_Group_ParentID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Group_GroupTypeID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Group')
  ALTER TABLE [dbo].[Group] DROP CONSTRAINT FK_Group_GroupTypeID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_GroupTypePosition_GroupTypeID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'GroupTypePosition')
  ALTER TABLE [dbo].[GroupTypePosition] DROP CONSTRAINT FK_GroupTypePosition_GroupTypeID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_GroupTypePosition_PositionID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'GroupTypePosition')
  ALTER TABLE [dbo].[GroupTypePosition] DROP CONSTRAINT FK_GroupTypePosition_PositionID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Role_GroupID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Role')
  ALTER TABLE [dbo].[Role] DROP CONSTRAINT FK_Role_GroupID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Role_PositionID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Role')
  ALTER TABLE [dbo].[Role] DROP CONSTRAINT FK_Role_PositionID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Role_UserID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Role')
  ALTER TABLE [dbo].[Role] DROP CONSTRAINT FK_Role_UserID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Substitution_SubstitutingUserID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Substitution')
  ALTER TABLE [dbo].[Substitution] DROP CONSTRAINT FK_Substitution_SubstitutingUserID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Substitution_SubstitutedUserID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Substitution')
  ALTER TABLE [dbo].[Substitution] DROP CONSTRAINT FK_Substitution_SubstitutedUserID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Substitution_SubstitutedRoleID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Substitution')
  ALTER TABLE [dbo].[Substitution] DROP CONSTRAINT FK_Substitution_SubstitutedRoleID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_Tenant_ParentID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'Tenant')
  ALTER TABLE [dbo].[Tenant] DROP CONSTRAINT FK_Tenant_ParentID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_User_TenantID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'User')
  ALTER TABLE [dbo].[User] DROP CONSTRAINT FK_User_TenantID
IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id WHERE fk.type = 'F' AND fk.name = 'FK_User_OwningGroupID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'User')
  ALTER TABLE [dbo].[User] DROP CONSTRAINT FK_User_OwningGroupID
-- Drop all tables
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AccessControlEntry' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[AccessControlEntry]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Permission' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Permission]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StateCombination' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[StateCombination]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AccessControlList' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[AccessControlList]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StateUsage' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[StateUsage]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EnumValueDefinition' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[EnumValueDefinition]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'AccessTypeReference' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[AccessTypeReference]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Culture' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Culture]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'LocalizedName' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[LocalizedName]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SecurableClassDefinition' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[SecurableClassDefinition]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StatePropertyDefinition' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[StatePropertyDefinition]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'StatePropertyReference' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[StatePropertyReference]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Group' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Group]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'GroupType' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[GroupType]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'GroupTypePosition' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[GroupTypePosition]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Position' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Position]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Role' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Role]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Substitution' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Substitution]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'Tenant' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[Tenant]
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'User' AND TABLE_SCHEMA = 'dbo')
  DROP TABLE [dbo].[User]
