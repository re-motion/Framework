// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using Remotion.Collections.Caching;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.Security;
using Remotion.Globalization;
using Remotion.ObjectBinding;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.Web.Test.Domain
{
  [MultiLingualResources("Remotion.Data.DomainObjects.Web.Test.Globalization.ClassForRelationTest")]
  [DBTable("TableForRelationTest")]
  [Instantiable]
  [DBStorageGroup]
  public abstract class ClassForRelationTest : BindableDomainObject, ISecurableObject, IDomainObjectSecurityContextFactory
  {
    private SecurityContext _securityContext;

    public static ClassForRelationTest NewObject ()
    {
      return DomainObject.NewObject<ClassForRelationTest>();
    }


    public ClassForRelationTest ()
    {
    }

    [StringProperty(IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    public override string DisplayName
    {
      get { return Name; }
    }

    [StorageClassNone]
    public ClassWithAllDataTypes.EnumType EnumProperty
    {
      get { return ClassWithAllDataTypes.EnumType.Value3; }
    }

    [ItemType(typeof(ClassWithAllDataTypes))]
    [ObjectBinding(ReadOnly = true)]
    [StorageClassNone]
    public DomainObjectCollection ComputedList
    {
      get { return new DomainObjectCollection(); }
    }

    [DBColumn("TableWithAllDataTypesMandatory")]
    [DBBidirectionalRelation("ClassesForRelationTestMandatoryNavigateOnly")]
    [Mandatory]
    public abstract ClassWithAllDataTypes ClassWithAllDataTypesMandatory {get; set;}

    [DBColumn("TableWithAllDataTypesOptional")]
    [DBBidirectionalRelation("ClassesForRelationTestOptionalNavigateOnly")]
    public abstract ClassWithAllDataTypes ClassWithAllDataTypesOptional { get; set;}

    [DBBidirectionalRelation("ClassForRelationTestMandatory")]
    [Mandatory]
    [ObjectBinding(ReadOnly = true)]
    public abstract ObjectList<ClassWithAllDataTypes> ClassesWithAllDataTypesMandatoryNavigateOnly { get; }

    [DBBidirectionalRelation("ClassForRelationTestOptional")]
    [ObjectBinding(ReadOnly = true)]
    public abstract ObjectList<ClassWithAllDataTypes> ClassesWithAllDataTypesOptionalNavigateOnly { get; }

    IObjectSecurityStrategy ISecurableObject.GetSecurityStrategy ()
    {
      return new DomainObjectSecurityStrategyDecorator(
          ObjectSecurityStrategy.Create(this, InvalidationToken.Create()),
          this,
          RequiredSecurityForStates.NewAndDeleted);
    }

    Type ISecurableObject.GetSecurableType ()
    {
      return GetPublicDomainObjectType();
    }

    ISecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      if (_securityContext == null)
      {
        _securityContext =
            SecurityContext.Create(
                GetPublicDomainObjectType(),
                null,
                null,
                Name.Substring(0, Name.Length - 1),
                new Dictionary<string, EnumWrapper>(),
                new EnumWrapper[0]);
      }
      return _securityContext;
    }

    bool IDomainObjectSecurityContextFactory.IsNew
    {
      get { return State.IsNew; }
    }

    bool IDomainObjectSecurityContextFactory.IsDeleted
    {
      get { return State.IsDeleted; }
    }
  }
}
