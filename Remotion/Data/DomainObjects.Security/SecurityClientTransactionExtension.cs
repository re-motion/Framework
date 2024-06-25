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
using System.Diagnostics.CodeAnalysis;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Reflection;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Security
{
  /// <summary>
  /// Adds security checks to the following re-store operations: object creation and deletion, queries, relation and property reads and writes.
  /// </summary>
  /// <remarks>
  /// The operations are not guarded against re-entry. Instead, the security implementation itself is responsible for not performing recursive 
  /// checks on the same object instance.
  /// </remarks>
  public class SecurityClientTransactionExtension : ClientTransactionExtensionBase
  {
    private static readonly IReadOnlyList<AccessType> s_findAccessType = ImmutableSingleton.Create(AccessType.Get(GeneralAccessTypes.Find));
    private static readonly IReadOnlyList<AccessType> s_deleteAccessType = ImmutableSingleton.Create(AccessType.Get(GeneralAccessTypes.Delete));
    private static readonly NullMethodInformation s_nullMethodInformation = new NullMethodInformation();

    public static string DefaultKey
    {
      get { return typeof(SecurityClientTransactionExtension).GetFullNameChecked(); }
    }

    private SecurityClient? _securityClient;


    public SecurityClientTransactionExtension ()
        : this(DefaultKey)
    {
    }

    protected SecurityClientTransactionExtension (string key)
      : base(key)
    {
    }

    public override QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("queryResult", queryResult);

      if (clientTransaction.ParentTransaction != null)
        return queryResult; // filtering already done in parent transaction

      if (SecurityFreeSection.IsActive)
        return queryResult;

      var queryResultList = new List<T?>(queryResult.AsEnumerable());
      var securityClient = GetSecurityClient();

      using (EnterScopeOnDemand(clientTransaction))
      {
        for (int i = queryResultList.Count - 1; i >= 0; i--)
        {
          var securableObject = queryResultList[i] as ISecurableObject;
          if (securableObject == null)
            continue;

          var hasAccess = securityClient.HasAccess(securableObject, s_findAccessType);
          if (!hasAccess)
            queryResultList.RemoveAt(i);
        }
      }

      if (queryResultList.Count != queryResult.Count)
        return new QueryResult<T>(queryResult.Query, queryResultList.ToArray());
      else
        return queryResult;
    }

    public override void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("type", type);

      if (!(typeof(ISecurableObject).IsAssignableFrom(type)))
        return;

      if (SecurityFreeSection.IsActive)
        return;

      var securityClient = GetSecurityClient();
      using (EnterScopeOnDemand(clientTransaction))
      {
        securityClient.CheckConstructorAccess(type);
      }
    }

    public override void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      if (SecurityFreeSection.IsActive)
        return;

      if (domainObject.TransactionContext[clientTransaction].State.IsNew)
        return;

      var securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      var securityClient = GetSecurityClient();
      using (EnterScopeOnDemand(clientTransaction))
      {
        securityClient.CheckAccess(securableObject, s_deleteAccessType);
      }
    }

    public override void PropertyValueReading (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      PropertyReading(clientTransaction, domainObject, propertyDefinition.PropertyInfo);
    }

    public override void RelationReading (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      if (relationEndPointDefinition.IsAnonymous)
        return;

      PropertyReading(clientTransaction, domainObject, relationEndPointDefinition.PropertyInfo);
    }

    private void PropertyReading (ClientTransaction clientTransaction, DomainObject domainObject, IPropertyInformation propertyInfo)
    {
      if (SecurityFreeSection.IsActive)
        return;

      var securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      var securityClient = GetSecurityClient();
      var methodInformation = propertyInfo.GetGetMethod(true) ?? s_nullMethodInformation;
      using (EnterScopeOnDemand(clientTransaction))
      {
        securityClient.CheckPropertyReadAccess(securableObject, methodInformation);
      }
    }

    public override void PropertyValueChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? oldValue,
        object? newValue)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      PropertyChanging(clientTransaction, domainObject, propertyDefinition.PropertyInfo);
    }

    public override void RelationChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      if (relationEndPointDefinition.IsAnonymous)
        return;

      PropertyChanging(clientTransaction, domainObject, relationEndPointDefinition.PropertyInfo);
    }

    public override void SubTransactionInitialize (ClientTransaction parentClientTransaction, ClientTransaction subTransaction)
    {
      ArgumentUtility.CheckNotNull("parentClientTransaction", parentClientTransaction);
      ArgumentUtility.CheckNotNull("subTransaction", subTransaction);

      TryInstall(subTransaction);
    }

    private void PropertyChanging (ClientTransaction clientTransaction, DomainObject domainObject, IPropertyInformation propertyInfo)
    {
      if (SecurityFreeSection.IsActive)
        return;

      var securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      var securityClient = GetSecurityClient();
      var methodInformation = propertyInfo.GetSetMethod(true) ?? s_nullMethodInformation;
      using (EnterScopeOnDemand(clientTransaction))
      {
        securityClient.CheckPropertyWriteAccess(securableObject, methodInformation);
      }
    }

    [MemberNotNull(nameof(_securityClient))]
    private SecurityClient GetSecurityClient ()
    {
      if (_securityClient == null)
        _securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      return _securityClient;
    }

    private IDisposable? EnterScopeOnDemand (ClientTransaction clientTransaction)
    {
      if (clientTransaction.ActiveTransaction != clientTransaction)
        return clientTransaction.EnterNonDiscardingScope();

      if (ClientTransaction.Current != clientTransaction)
        return clientTransaction.EnterNonDiscardingScope();

      return null;
    }
  }
}
