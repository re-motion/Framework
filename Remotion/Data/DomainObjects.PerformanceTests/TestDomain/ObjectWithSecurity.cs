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
using Remotion.Security;

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  [Instantiable]
  [DBTable]
  [BindableDomainObject]
  public abstract class ObjectWithSecurity : DomainObject, ISecurableObject, IDomainObjectSecurityContextFactory
  {
    private SecurityContext _securityContext;
    private IObjectSecurityStrategy _domainObjectSecurityStrategy;

    public static ObjectWithSecurity NewObject ()
    {
      return NewObject<ObjectWithSecurity>();
    }

    protected ObjectWithSecurity ()
    {
    }

    #region Dummy properties

    public abstract int IntProperty1 { get; set; }
    public abstract int IntProperty2 { get; set; }
    public abstract int IntProperty3 { get; set; }
    public abstract int IntProperty4 { get; set; }
    public abstract int IntProperty5 { get; set; }
    public abstract int IntProperty6 { get; set; }
    public abstract int IntProperty7 { get; set; }
    public abstract int IntProperty8 { get; set; }
    public abstract int IntProperty9 { get; set; }
    public abstract int IntProperty10 { get; set; }

    public abstract DateTime DateTimeProperty1 { get; set; }
    public abstract DateTime DateTimeProperty2 { get; set; }
    public abstract DateTime DateTimeProperty3 { get; set; }
    public abstract DateTime DateTimeProperty4 { get; set; }
    public abstract DateTime DateTimeProperty5 { get; set; }
    public abstract DateTime DateTimeProperty6 { get; set; }
    public abstract DateTime DateTimeProperty7 { get; set; }
    public abstract DateTime DateTimeProperty8 { get; set; }
    public abstract DateTime DateTimeProperty9 { get; set; }
    public abstract DateTime DateTimeProperty10 { get; set; }

    public abstract string StringProperty1 { get; set; }
    public abstract string StringProperty2 { get; set; }
    public abstract string StringProperty3 { get; set; }
    public abstract string StringProperty4 { get; set; }
    public abstract string StringProperty5 { get; set; }
    public abstract string StringProperty6 { get; set; }
    public abstract string StringProperty7 { get; set; }
    public abstract string StringProperty8 { get; set; }
    public abstract string StringProperty9 { get; set; }
    public abstract string StringProperty10 { get; set; }

    public abstract bool BoolProperty1 { get; set; }
    public abstract bool BoolProperty2 { get; set; }
    public abstract bool BoolProperty3 { get; set; }
    public abstract bool BoolProperty4 { get; set; }
    public abstract bool BoolProperty5 { get; set; }
    public abstract bool BoolProperty6 { get; set; }
    public abstract bool BoolProperty7 { get; set; }
    public abstract bool BoolProperty8 { get; set; }
    public abstract bool BoolProperty9 { get; set; }
    public abstract bool BoolProperty10 { get; set; }

    public abstract OppositeClassWithAnonymousRelationProperties Unary1 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary2 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary3 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary4 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary5 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary6 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary7 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary8 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary9 { get; set; }
    public abstract OppositeClassWithAnonymousRelationProperties Unary10 { get; set; }

    [DBBidirectionalRelation("Virtual1", ContainsForeignKey = true)]
    public abstract ObjectWithSecurity Real1 { get; set; }
    [DBBidirectionalRelation("Virtual2", ContainsForeignKey = true)]
    public abstract ObjectWithSecurity Real2 { get; set; }
    [DBBidirectionalRelation("Virtual3", ContainsForeignKey = true)]
    public abstract ObjectWithSecurity Real3 { get; set; }
    [DBBidirectionalRelation("Virtual4", ContainsForeignKey = true)]
    public abstract ObjectWithSecurity Real4 { get; set; }
    [DBBidirectionalRelation("Virtual5", ContainsForeignKey = true)]
    public abstract ObjectWithSecurity Real5 { get; set; }
    [DBBidirectionalRelation("Virtual6", ContainsForeignKey = true)]
    public abstract ObjectWithSecurity Real6 { get; set; }
    [DBBidirectionalRelation("Virtual7", ContainsForeignKey = true)]
    public abstract ObjectWithSecurity Real7 { get; set; }
    [DBBidirectionalRelation("Virtual8", ContainsForeignKey = true)]
    public abstract ObjectWithSecurity Real8 { get; set; }
    [DBBidirectionalRelation("Virtual9", ContainsForeignKey = true)]
    public abstract ObjectWithSecurity Real9 { get; set; }
    [DBBidirectionalRelation("Virtual10", ContainsForeignKey = true)]
    public abstract ObjectWithSecurity Real10 { get; set; }

    [DBBidirectionalRelation("Real1")]
    public abstract ObjectList<ObjectWithSecurity> Virtual1 { get; set; }
    [DBBidirectionalRelation("Real2")]
    public abstract ObjectList<ObjectWithSecurity> Virtual2 { get; set; }
    [DBBidirectionalRelation("Real3")]
    public abstract ObjectList<ObjectWithSecurity> Virtual3 { get; set; }
    [DBBidirectionalRelation("Real4")]
    public abstract ObjectList<ObjectWithSecurity> Virtual4 { get; set; }
    [DBBidirectionalRelation("Real5")]
    public abstract ObjectList<ObjectWithSecurity> Virtual5 { get; set; }
    [DBBidirectionalRelation("Real6")]
    public abstract ObjectList<ObjectWithSecurity> Virtual6 { get; set; }
    [DBBidirectionalRelation("Real7")]
    public abstract ObjectList<ObjectWithSecurity> Virtual7 { get; set; }
    [DBBidirectionalRelation("Real8")]
    public abstract ObjectList<ObjectWithSecurity> Virtual8 { get; set; }
    [DBBidirectionalRelation("Real9")]
    public abstract ObjectList<ObjectWithSecurity> Virtual9 { get; set; }
    [DBBidirectionalRelation("Real10")]
    public abstract ObjectList<ObjectWithSecurity> Virtual10 { get; set; }

    #endregion

    public abstract int TheProperty { get; set; }

    IObjectSecurityStrategy ISecurableObject.GetSecurityStrategy ()
    {
      if (_domainObjectSecurityStrategy == null)
      {
        _domainObjectSecurityStrategy =
            new InstanceBasedReEntrancyGuardedObjectSecurityStrategyDecorator(
                new DomainObjectSecurityStrategyDecorator(
                    ObjectSecurityStrategy.Create(this, InvalidationToken.Create()),
                    this,
                    RequiredSecurityForStates.NewAndDeleted));
      }
      return _domainObjectSecurityStrategy;
    }

    Type ISecurableObject.GetSecurableType ()
    {
      return GetPublicDomainObjectType();
    }

    ISecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      if (_securityContext == null)
      {
        _securityContext = SecurityContext.Create(
            ((ISecurableObject)this).GetSecurableType(),
            null,
            null,
            null,
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
