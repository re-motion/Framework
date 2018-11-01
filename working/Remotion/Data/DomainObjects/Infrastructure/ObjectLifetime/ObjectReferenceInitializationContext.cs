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
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime
{
  /// <summary>
  /// Represents the context of an object reference being initialized.
  /// </summary>
  public class ObjectReferenceInitializationContext : IObjectInitializationContext
  {
    private readonly ObjectID _objectID;
    private readonly ClientTransaction _rootTransaction;
    private readonly IEnlistedDomainObjectManager _enlistedDomainObjectManager;

    private DomainObject _registeredObject;

    public ObjectReferenceInitializationContext (
        ObjectID objectID,
        ClientTransaction rootTransaction,
        IEnlistedDomainObjectManager enlistedDomainObjectManager)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      ArgumentUtility.CheckNotNull ("rootTransaction", rootTransaction);
      ArgumentUtility.CheckNotNull ("enlistedDomainObjectManager", enlistedDomainObjectManager);

      if (rootTransaction != rootTransaction.RootTransaction)
        throw new ArgumentException ("The rootTransaction parameter must be passed a root transaction.", "rootTransaction");

      _objectID = objectID;
      _rootTransaction = rootTransaction;
      _enlistedDomainObjectManager = enlistedDomainObjectManager;
    }

    public ObjectID ObjectID
    {
      get { return _objectID; }
    }

    public ClientTransaction RootTransaction
    {
      get { return _rootTransaction; }
    }

    public IEnlistedDomainObjectManager EnlistedDomainObjectManager
    {
      get { return _enlistedDomainObjectManager; }
    }

    public DomainObject RegisteredObject
    {
      get { return _registeredObject; }
    }

    public virtual void RegisterObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      if (domainObject.ID != _objectID)
        throw new ArgumentException (string.Format ("The given DomainObject must have ID '{0}'.", _objectID), "domainObject");

      if (_registeredObject != null)
        throw new InvalidOperationException ("Only one object can be registered using this context.");

      _enlistedDomainObjectManager.EnlistDomainObject (domainObject);
      _registeredObject = domainObject;
    }
  }
}