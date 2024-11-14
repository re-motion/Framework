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
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  public abstract class TestDomainBase : DomainObject, ISupportsGetObject
  {
    public static event EventHandler StaticCtorHandler;
    public static event EventHandler StaticLoadHandler;
    public static event EventHandler StaticInitializationHandler;

    public static void ClearStaticCtorHandlers ()
    {
      if (StaticCtorHandler != null)
      {
        var registeredHandlers = StaticCtorHandler.GetInvocationList();
        foreach (EventHandler registeredHandler in registeredHandlers)
          StaticCtorHandler -= registeredHandler;
      }
    }

    private IUnloadEventReceiver _unloadEventReceiver;
    private ILoadEventReceiver _loadEventReceiver;

    public event EventHandler ProtectedLoaded;

    public bool CtorCalled;
    public ClientTransaction CtorTx;

    public bool OnReferenceInitializingCalledBeforeCtor;
    public bool OnReferenceInitializingCalled;
    public ClientTransaction OnReferenceInitializingTx;
    public ClientTransaction OnReferenceInitializingActiveTx;
    public ObjectID OnReferenceInitializingID;

    public bool OnLoadedCalled;
    public ClientTransaction OnLoadedTx;
    public LoadMode OnLoadedLoadMode;
    public int OnLoadedCallCount;

    public bool OnUnloadingCalled;
    public ClientTransaction OnUnloadingTx;
    public DateTime OnUnloadingDateTime;
    public DomainObjectState UnloadingState;

    public bool OnUnloadedCalled;
    public ClientTransaction OnUnloadedTx;
    public DateTime OnUnloadedDateTime;
    public DomainObjectState UnloadedState;

    protected TestDomainBase ()
    {
      if (StaticCtorHandler != null)
        StaticCtorHandler(this, EventArgs.Empty);
      CtorCalled = true;
      CtorTx = ClientTransaction.Current;
      OnReferenceInitializingCalledBeforeCtor = OnReferenceInitializingCalled;
    }

    [StorageClassNone]
    public DataContainer InternalDataContainer
    {
      get
      {
        var transaction = RootTransaction.ActiveTransaction;
        return GetInternalDataContainerForTransaction(transaction);
      }
    }

    public DataContainer GetInternalDataContainerForTransaction (ClientTransaction transaction)
    {
      var dataManager = (DataManager)PrivateInvoke.GetNonPublicProperty(transaction, "DataManager");
      return dataManager.GetDataContainerWithLazyLoad(ID, true);
    }

    public DomainObject GetRelatedObject (string propertyName)
    {
      return (DomainObject)Properties[propertyName].GetValueWithoutTypeCheck();
    }

    public IEnumerable GetRelatedObjects (string propertyName)
    {
      return (IEnumerable)Properties[propertyName].GetValueWithoutTypeCheck();
    }

    public DomainObjectCollection GetRelatedObjectsAsDomainObjectCollection (string propertyName)
    {
      return (DomainObjectCollection)Properties[propertyName].GetValueWithoutTypeCheck();
    }

    public IReadOnlyList<DomainObject> GetRelatedObjectsAsVirtualCollection (string propertyName)
    {
      return (IReadOnlyList<DomainObject>)Properties[propertyName].GetValueWithoutTypeCheck();
    }

    public DomainObject GetOriginalRelatedObject (string propertyName)
    {
      return (DomainObject)Properties[propertyName].GetOriginalValueWithoutTypeCheck();
    }

    public DomainObjectCollection GetOriginalRelatedObjectsAsDomainObjectCollection (string propertyName)
    {
      return (DomainObjectCollection)Properties[propertyName].GetOriginalValueWithoutTypeCheck();
    }

    public IReadOnlyList<DomainObject> GetOriginalRelatedObjectsAsVirtualCollection (string propertyName)
    {
      return (IReadOnlyList<DomainObject>)Properties[propertyName].GetOriginalValueWithoutTypeCheck();
    }

    public void SetRelatedObject (string propertyName, DomainObject newRelatedObject)
    {
      Properties[propertyName].SetValueWithoutTypeCheck(newRelatedObject);
    }

    public new void Delete ()
    {
      base.Delete();
    }

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }

    [StorageClassNone]
    public bool NeedsLoadModeDataContainerOnly
    {
      get { return (bool)PrivateInvoke.GetNonPublicField(this, typeof(DomainObject), "_needsLoadModeDataContainerOnly"); }
    }

    protected override void OnReferenceInitializing ()
    {
      base.OnReferenceInitializing();

      OnReferenceInitializingCalled = true;
      OnReferenceInitializingTx = ClientTransaction.Current;
      OnReferenceInitializingActiveTx = ClientTransaction.Current.ActiveTransaction;
      OnReferenceInitializingID = ID;

      if (StaticInitializationHandler != null)
        StaticInitializationHandler(this, EventArgs.Empty);
    }

    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded(loadMode);

      OnLoadedCalled = true;
      OnLoadedTx = ClientTransaction.Current;
      OnLoadedLoadMode = loadMode;
      ++OnLoadedCallCount;

      if (ProtectedLoaded != null)
        ProtectedLoaded(this, EventArgs.Empty);
      if (StaticLoadHandler != null)
        StaticLoadHandler(this, EventArgs.Empty);

      if (_loadEventReceiver != null)
        _loadEventReceiver.OnLoaded(this);
    }

    protected override void OnUnloading ()
    {
      base.OnUnloading();
      OnUnloadingCalled = true;
      OnUnloadingTx = ClientTransaction.Current;

      OnUnloadingDateTime = DateTime.Now;
      while (DateTime.Now == OnUnloadingDateTime)
      {
      }

      UnloadingState = State;
      if (_unloadEventReceiver != null)
        _unloadEventReceiver.OnUnloading(this);
    }

    protected override void OnUnloaded ()
    {
      base.OnUnloading();
      OnUnloadedCalled = true;
      OnUnloadedTx = ClientTransaction.Current;

      OnUnloadedDateTime = DateTime.Now;
      while (DateTime.Now == OnUnloadedDateTime)
      {
      }

      UnloadedState = State;
      if (_unloadEventReceiver != null)
        _unloadEventReceiver.OnUnloaded(this);
    }

    public void SetUnloadEventReceiver (IUnloadEventReceiver unloadEventReceiver)
    {
      _unloadEventReceiver = unloadEventReceiver;
    }

    public void SetLoadEventReceiver (ILoadEventReceiver loadEventReceiver)
    {
      _loadEventReceiver = loadEventReceiver;
    }
  }
}
