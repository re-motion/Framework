use SecurityManagerActaNova

-- USAGE:
-- 1. Set the variables declared below to configure the ACE.
-- 2. Add rows to #AccessTypes to allow/deny specific access types.
-- 3. Uncomment the ClassID-columns according to the layout of the database.
-- 4. Execute the script.
-- 5. Execute the 'commit transaction' statement at the bottom when satisfied with the results.
-- 5a. When not satisfied, execute the 'rollback transaction' statement to drop the temp-tables created during the script's execution.

-- NOTE:
-- Please verify the correctness of the ACE-settings within the test-installation before deploying the script in production.
-- Inconsistent ACE-settings can result in run-time errors when performing security evaluation, thus stopping the entire system until the problem is fixed.

begin transaction

declare @tenantCondition int;
declare @tenantHierarchyCondition int;
declare @groupCondition  int;
declare @groupHierarchyCondition int;
declare @userCondition int;
declare @specificTenantID uniqueidentifier;
declare @specificTenantIDClassID varchar(100);
declare @specificGroupID uniqueidentifier;
declare @specificGroupIDClassID varchar(100);
declare @specificGroupTypeID uniqueidentifier;
declare @specificGroupTypeIDClassID varchar(100);
declare @specificPositionID uniqueidentifier;
declare @specificPositionIDClassID varchar(100);
declare @specificUserID uniqueidentifier;
declare @specificUserIDClassID varchar(100);
declare @specificAbstractRoleID uniqueidentifier;
declare @specificAbstractRoleIDClassID varchar(100);

set @tenantCondition = 0; -- 0=None, 1=OwningTenant, 2=SpecificTenant
set @tenantHierarchyCondition = 0; -- 0=Undefined, 1=This, 2=Parent, 3=ThisAndParent
set @specificTenantID = null;
set @specificTenantIDClassID = null;

set @groupCondition = 0; -- 0=None, 1=OwningGroup, 2=SpecificGroup, 3=AnyGroupWithSpecificGroupType, 4=BranchOfOwningGroup
set @groupHierarchyCondition = 0; -- 0=Undefined, 1=This, 2=Parent, 4=Children, 3=ThisAndParent, 5=ThisAndChildren, 7=ThisAndParentAndChildren
set @specificGroupID = null; 
set @specificGroupIDClassID = null;

set @userCondition = 0; -- 0=None, 1=Owner, 2=SpecificUser, 3=SpecificPosition
set @specificUserID = null;
set @specificUserIDClassID = null;

set @specificGroupTypeID = null;
set @specificGroupTypeIDClassID = null;

set @specificPositionID = null;
set @specificPositionIDClassID = null;

set @specificAbstractRoleID = null;
set @specificAbstractRoleIDClassID = null;

create table #AccessTypes 
(
    ID uniqueidentifier NOT NULL, -- ID of the AccessTypeDefinition in table EnumValueDefinition
    Allowed bit NOT NULL          -- 1 = allowed, 0 = denied
);
--insert into #AccessTypes values ('79FFA475-5A0E-4E20-A445-80AA34583D3F', 1); -- Read|Remotion.Security.GeneralAccessTypes = allowed

select 
    NEWID() as [ID], 
    'AccessControlEntry' as [ClassID],
    MAX(ace.[Index]) + 1 as [Index],
    @tenantCondition as [TenantCondition],
    @tenantHierarchyCondition as [TenantHierarchyCondition],
    @groupCondition as [GroupCondition],
    @groupHierarchyCondition as [GroupHierarchyCondition],
    @userCondition as [UserCondition],
    @specificTenantID as [SpecificTenantID],
    --@specificTenantIDClassID as [SpecificTenantIDClassID],
    @specificGroupID as [SpecificGroupID],
    --@specificGroupIDClassID as [SpecificGroupIClassIDD],
    @specificGroupTypeID as [SpecificGroupTypeID],
    --@specificGroupTypeIDClassID as [SpecificGroupTypeIDClassID],
    @specificPositionID as [SpecificPositionID],
    --@specificPositionIDClassID as [SpecificPositionIDClassID],
    @specificUserID as [SpecificUserID],
    --@specificUserIDClassID as [SpecificUserIDClassID],
    @specificAbstractRoleID as [SpecificAbstractRoleID],
    @specificAbstractRoleIDClassID as [SpecificAbstractRoleIDClassID],
    acl.[ID] as [AccessControlListID],
    acl.[ClassID] as [AccessControlListIDClassID],
    acl.[StatefulAcl_ClassID]
into #AccessControlEntry
from StatefulAccessControlListView acl
inner join AccessControlEntry ace on acl.ID = ace.AccessControlListID
group by acl.[ID], acl.[ClassID], acl.[StatefulAcl_ClassID];

select
    NEWID() as [ID],
    'Permission' as [ClassID],
    NULL as [Allowed],
    atr.[AccessTypeID] [AccessTypeDefinitionID],
    atr.[AccessTypeIDClassID] as [AccessTypeDefinitionIDClassID],
    ace.[ID] as [AccessControlEntryID]
into #Permission
from #AccessControlEntry ace
inner join AccessTypeReference atr on ace.[StatefulAcl_ClassID] = atr.[SecurableClassID];

update p set
  p.Allowed = at.Allowed
from #Permission p
inner join #AccessTypes at on p.AccessTypeDefinitionID = at.ID

insert into AccessControlEntry
(
    [ID],
    [ClassID],
    [Index],
    [TenantCondition],
    [TenantHierarchyCondition],
    [GroupCondition],
    [GroupHierarchyCondition],
    [UserCondition],
    [SpecificTenantID],
    --[SpecificTenantIDClassID],
    [SpecificGroupID],
    --[SpecificGroupIDClassID],
    [SpecificGroupTypeID],
    --[SpecificGroupTypeIDClassID],
    [SpecificPositionID],
    --[SpecificPositionIDClassID],
    [SpecificUserID],
    --[SpecificUserIDClassID],
    [SpecificAbstractRoleID],
    [SpecificAbstractRoleIDClassID],
    [AccessControlListID],
    [AccessControlListIDClassID]
)
select 
    [ID],
    [ClassID],
    [Index],
    [TenantCondition],
    [TenantHierarchyCondition],
    [GroupCondition],
    [GroupHierarchyCondition],
    [UserCondition],
    [SpecificTenantID],
    --[SpecificTenantIDClassID],
    [SpecificGroupID],
    --[SpecificGroupIDClassID],
    [SpecificGroupTypeID],
    --[SpecificGroupTypeIDClassID],
    [SpecificPositionID],
    --[SpecificPositionIDClassID],
    [SpecificUserID],
    --[SpecificUserIDClassID],
    [SpecificAbstractRoleID],
    [SpecificAbstractRoleIDClassID],
    [AccessControlListID],
    [AccessControlListIDClassID]
from #AccessControlEntry;

insert into Permission
(
    [ID],
    [ClassID],
    [Allowed],
    [AccessTypeDefinitionID],
    [AccessTypeDefinitionIDClassID],
    [AccessControlEntryID]
)
select 
    [ID],
    [ClassID],
    [Allowed],
    [AccessTypeDefinitionID],
    [AccessTypeDefinitionIDClassID],
    [AccessControlEntryID]
from #Permission;

update Revision set [Value] = NEWID() where [GlobalKey] = '446DF534-DBEA-420E-9AC1-0B19D51B0ED3'

select ace.*, cd.[Name]
from #AccessControlEntry ace
inner join SecurableClassDefinition cd on cd.[ID] = ace.[StatefulAcl_ClassID]

select p.[AccessControlEntryID], at.[Name], p.[Allowed]
from #Permission p
inner join EnumValueDefinition at on at.[ID] = p.[AccessTypeDefinitionID]
order by p.[AccessControlEntryID], at.[Name]

--rollback transaction
--commit transaction
