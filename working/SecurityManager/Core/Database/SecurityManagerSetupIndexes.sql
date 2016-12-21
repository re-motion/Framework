USE RemotionSecurityManager
GO

-- Clustered Indexes

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'CIDX_Permission')
  CREATE CLUSTERED INDEX [CIDX_Permission] ON [Permission] ( [AccessControlEntryID] ASC ) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'CIDX_Role')
  CREATE CLUSTERED INDEX [CIDX_Role] ON [Role] ( [UserID] ASC ) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'CIDX_Substitution')
  CREATE CLUSTERED INDEX [CIDX_Substitution] ON [Substitution] ( [SubstitutedUserID] ASC ) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

-- AccessControlEntry

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_AccessControlEntry_SpecificTenantID')
  CREATE INDEX [IDX_AccessControlEntry_SpecificTenantID] ON [AccessControlEntry] ([SpecificTenantID])

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_AccessControlEntry_SpecificGroupID')
  CREATE INDEX [IDX_AccessControlEntry_SpecificGroupID] ON [AccessControlEntry] ([SpecificGroupID])

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_AccessControlEntry_SpecificGroupTypeID')
  CREATE INDEX [IDX_AccessControlEntry_SpecificGroupTypeID] ON [AccessControlEntry] ([SpecificGroupTypeID])

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_AccessControlEntry_SpecificPositionID')
  CREATE INDEX [IDX_AccessControlEntry_SpecificPositionID] ON [AccessControlEntry] ([SpecificPositionID])

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_AccessControlEntry_SpecificUserID')
  CREATE INDEX [IDX_AccessControlEntry_SpecificUserID] ON [AccessControlEntry] ([SpecificUserID])

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_AccessControlEntry_SpecificAbstractRoleID')
  CREATE INDEX [IDX_AccessControlEntry_SpecificAbstractRoleID] ON [AccessControlEntry] ([SpecificAbstractRoleID])

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_AccessControlEntry_AccessControlListID')
  CREATE INDEX [IDX_AccessControlEntry_AccessControlListID] ON [AccessControlEntry] ([AccessControlListID])

-- AccessControlList

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_AccessControlList_StatefulAcl_ClassID')
  CREATE INDEX [IDX_AccessControlList_StatefulAcl_ClassID] ON [AccessControlList] ([StatefulAcl_ClassID]) INCLUDE ([ID], [ClassID], [StatefulAcl_ClassIDClassID])

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_AccessControlList_StatelessAcl_ClassID')
  CREATE INDEX [IDX_AccessControlList_StatelessAcl_ClassID] ON [AccessControlList] ([StatelessAcl_ClassID]) INCLUDE ([ID], [ClassID], [StatelessAcl_ClassIDClassID])

-- AccessTypeReference

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_AccessTypeReference_SecurableClassID')
  CREATE INDEX [IDX_AccessTypeReference_SecurableClassID] ON [AccessTypeReference] ([SecurableClassID])
  
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_AccessTypeReference_AccessTypeID')
  CREATE INDEX [IDX_AccessTypeReference_AccessTypeID] ON [AccessTypeReference] ([AccessTypeID])

-- EnumValueDefinition

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_EnumValueDefinition_MetadataItemID')
  CREATE INDEX [IDX_EnumValueDefinition_MetadataItemID] ON [EnumValueDefinition] ([MetadataItemID])

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_EnumValueDefinition_StatePropertyID_Value')
  CREATE INDEX [IDX_EnumValueDefinition_StatePropertyID_Value] ON [EnumValueDefinition] ([StatePropertyID], [Value])

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_EnumValueDefinition_Name')
  CREATE INDEX [IDX_EnumValueDefinition_Name] ON [EnumValueDefinition] ([Name]) INCLUDE ([ID], [ClassID])

-- Group

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_Group_UniqueIdentifier')
  CREATE INDEX [IDX_Group_UniqueIdentifier] ON [Group] ([UniqueIdentifier]) INCLUDE ([ID], [ClassID])

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_Group_TenantID')
  CREATE INDEX [IDX_Group_TenantID] ON [Group] ([TenantID])

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_Group_ParentID')
  CREATE INDEX [IDX_Group_ParentID] ON [Group] ([ParentID])
  
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_Group_Name')
  CREATE INDEX [IDX_Group_Name] ON [Group] ([Name])
    
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_Group_ShortName')
  CREATE INDEX [IDX_Group_ShortName] ON [Group] ([ShortName])

-- GroupTypePosition

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_GroupTypePosition_GroupTypeID')
  CREATE INDEX [IDX_GroupTypePosition_GroupTypeID] ON [GroupTypePosition] ([GroupTypeID])
  
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_GroupTypePosition_PositionID')
  CREATE INDEX [IDX_GroupTypePosition_PositionID] ON [GroupTypePosition] ([PositionID])

-- LocalizedName

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_LocalizedName_CultureID')
  CREATE INDEX [IDX_LocalizedName_CultureID] ON [LocalizedName] ([CultureID])

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_LocalizedName_MetadataObjectID')
  CREATE INDEX [IDX_LocalizedName_MetadataObjectID] ON [LocalizedName] ([MetadataObjectID])

-- Permission

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_Permission_AccessControlEntryID')
  CREATE INDEX [IDX_Permission_AccessControlEntryID] ON [Permission] ([AccessControlEntryID])

-- Position

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_Position_UniqueIdentifier')
  CREATE INDEX [IDX_Position_UniqueIdentifier] ON [Position] ([UniqueIdentifier]) INCLUDE ([ID], [ClassID])

-- Role

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_Role_GroupID')
  CREATE INDEX [IDX_Role_GroupID] ON [Role] ([GroupID])

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_Role_UserID')
  CREATE INDEX [IDX_Role_UserID] ON [Role] ([UserID])

-- SecurableClassDefinition

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_SecurableClassDefinition_Name')
  CREATE INDEX [IDX_SecurableClassDefinition_Name] ON [SecurableClassDefinition] ([Name]) INCLUDE ([ID], [ClassID], [BaseSecurableClassID], [BaseSecurableClassIDClassID])
  
-- StateCombination

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_StateCombination_AccessControlListID')
  CREATE INDEX [IDX_StateCombination_AccessControlListID] ON [StateCombination] ([AccessControlListID])

-- StatePropertyDefinition

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_StatePropertyDefinition_Name')
  CREATE INDEX [IDX_StatePropertyDefinition_Name] ON [StatePropertyDefinition] ([Name])

-- StatePropertyReference

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_StatePropertyReference_SecurableClassID')
  CREATE INDEX [IDX_StatePropertyReference_SecurableClassID] ON [StatePropertyReference] ([SecurableClassID]) INCLUDE ([ID], [StatePropertyID], [StatePropertyIDClassID])

-- StateUsage

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_StateUsage_StateCombinationID')
  CREATE INDEX [IDX_StateUsage_StateCombinationID] ON [StateUsage] ([StateCombinationID]) INCLUDE ([ID], [StateDefinitionID])

-- Substitution

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_Substitution_SubstitutedUserID')
  CREATE INDEX [IDX_Substitution_SubstitutedUserID] ON [Substitution] ([SubstitutedUserID])

-- Tenant

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_Tenant_UniqueIdentifier')
  CREATE INDEX [IDX_Tenant_UniqueIdentifier] ON [Tenant] ([UniqueIdentifier]) INCLUDE ([ID], [ClassID])

-- User

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IDX_User_UserName')
  CREATE INDEX [IDX_User_UserName] ON [User] ([UserName]) INCLUDE ([ID], [ClassID])
