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
using Remotion.Collections.Caching;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Security;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.ObjectBinding.IntegrationTests.TestDomain.Security
{
  [BindableDomainObject]
  [Instantiable]
  [DBTable]
  [Uses(typeof(BindableSecurableObjectMixin))]
  public abstract class BindableSecurableObject : DomainObject, ISecurableObject, ISecurityContextFactory
  {
    public static BindableSecurableObject NewObject (ClientTransaction clientTransaction, IObjectSecurityStrategy securityStrategy)
    {
      using (clientTransaction.EnterNonDiscardingScope())
      {
        return NewObject<BindableSecurableObject>(ParamList.Create(securityStrategy));
      }
    }

    private IObjectSecurityStrategy _securityStrategy;
    private string _readOnlyProperty = string.Empty;
    private string _propertyToOverride = string.Empty;


    protected BindableSecurableObject (IObjectSecurityStrategy securityStrategy)
    {
      _securityStrategy = securityStrategy;
    }

    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded(loadMode);
      _securityStrategy = ObjectSecurityStrategy.Create(this, InvalidationToken.Create());
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _securityStrategy;
    }

    Type ISecurableObject.GetSecurableType ()
    {
      return GetPublicDomainObjectType();
    }

    public DataContainer GetDataContainer (ClientTransaction transaction)
    {
      var dataManager = (DataManager)PrivateInvoke.GetNonPublicProperty(transaction, "DataManager");
      return dataManager.GetDataContainerWithLazyLoad(ID, true);
    }

    public abstract string StringProperty { get; set; }

    public abstract string OtherStringProperty { get; set; }

    [DBBidirectionalRelation("Children")]
    public abstract BindableSecurableObject Parent { get; set; }

    [DBBidirectionalRelation("Parent")]
    public abstract ObjectList<BindableSecurableObject> Children { get; /*no setter*/ }

    [DBBidirectionalRelation("OtherChildren")]
    public abstract BindableSecurableObject OtherParent { get; set; }

    [DBBidirectionalRelation("OtherParent")]
    public abstract ObjectList<BindableSecurableObject> OtherChildren { get; }

    public abstract string PropertyWithDefaultPermission { get; set; }

    public abstract string PropertyWithCustomPermission
    {
      [DemandPermission(TestAccessTypes.First)] get;
      [DemandPermission(TestAccessTypes.Second)] set;
    }

    public string ReadOnlyProperty
    {
      get { return _readOnlyProperty; }
    }

    public virtual string PropertyToOverride
    {
      get { return _propertyToOverride; }
      [DemandPermission(TestAccessTypes.Second)]
      set { _propertyToOverride = value; }
    }

    ISecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      return SecurityContext.CreateStateless(GetPublicDomainObjectType());
    }

    public new void Delete ()
    {
      base.Delete();
    }
  }
}
