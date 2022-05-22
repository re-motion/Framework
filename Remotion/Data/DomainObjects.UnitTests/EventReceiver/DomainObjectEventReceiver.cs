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
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  [Serializable]
  public class DomainObjectEventReceiver : EventReceiverBase
  {
    // types

    // static members and constants

    // member fields

    private readonly DomainObject _domainObject;

    private bool _cancel;
    private readonly ClientTransaction _transactionToVerify;
    private bool _hasChangingEventBeenCalled = false;
    private bool _hasChangedEventBeenCalled = false;
    [NonSerialized]
    private PropertyDefinition _changingPropertyDefinition;
    [NonSerialized]
    private PropertyDefinition _changedPropertyDefinition;
    private object _changingOldValue;
    private object _changingNewValue;
    private object _changedOldValue;
    private object _changedNewValue;
    private bool _hasDeletingEventBeenCalled = false;
    private bool _hasDeletedEventBeenCalled = false;

    private bool _hasRelationChangingEventBeenCalled = false;
    private bool _hasRelationChangedEventBeenCalled = false;
    private string _changingRelationPropertyName;
    private string _changedRelationPropertyName;
    [NonSerialized]
    private IDomainObject _changingOldRelatedObject;
    [NonSerialized]
    private IDomainObject _changingNewRelatedObject;
    [NonSerialized]
    private IDomainObject _changedOldRelatedObject;
    [NonSerialized]
    private IDomainObject _changedNewRelatedObject;

    private bool _hasCommittingEventBeenCalled = false;
    private bool _hasCommittedEventBeenCalled = false;

    private bool _hasRollingBackEventBeenCalled = false;
    private bool _hasRolledBackEventBeenCalled = false;

    // construction and disposing

    public DomainObjectEventReceiver (DomainObject domainObject, bool cancel = false, ClientTransaction transactionToVerify = null)
    {
      _domainObject = domainObject;
      _cancel = cancel;
      _transactionToVerify = transactionToVerify;

      _domainObject.PropertyChanging += DomainObject_PropertyChanging;
      _domainObject.PropertyChanged += DomainObject_PropertyChanged;
      _domainObject.RelationChanging += DomainObject_RelationChanging;
      _domainObject.RelationChanged += DomainObject_RelationChanged;
      _domainObject.Deleting += domainObject_Deleting;
      _domainObject.Deleted += domainObject_Deleted;
      _domainObject.Committing += DomainObject_Committing;
      _domainObject.Committed += DomainObject_Committed;
      _domainObject.RollingBack += DomainObject_RollingBack;
      _domainObject.RolledBack += DomainObject_RolledBack;
    }

    // methods and properties

    public bool Cancel
    {
      get { return _cancel; }
      set { _cancel = value; }
    }

    public bool HasChangingEventBeenCalled
    {
      get { return _hasChangingEventBeenCalled; }
    }

    public bool HasChangedEventBeenCalled
    {
      get { return _hasChangedEventBeenCalled; }
    }

    public PropertyDefinition ChangingPropertyDefinition
    {
      get { return _changingPropertyDefinition; }
    }

    public PropertyDefinition ChangedPropertyDefinition
    {
      get { return _changedPropertyDefinition; }
    }

    public object ChangingOldValue
    {
      get { return _changingOldValue; }
    }

    public object ChangingNewValue
    {
      get { return _changingNewValue; }
    }

    public object ChangedOldValue
    {
      get { return _changedOldValue; }
    }

    public object ChangedNewValue
    {
      get { return _changedNewValue; }
    }

    public bool HasRelationChangingEventBeenCalled
    {
      get { return _hasRelationChangingEventBeenCalled; }
    }

    public bool HasRelationChangedEventBeenCalled
    {
      get { return _hasRelationChangedEventBeenCalled; }
    }

    public string ChangingRelationPropertyName
    {
      get { return _changingRelationPropertyName; }
    }

    public string ChangedRelationPropertyName
    {
      get { return _changedRelationPropertyName; }
    }

    public IDomainObject ChangingOldRelatedObject
    {
      get { return _changingOldRelatedObject; }
    }

    public IDomainObject ChangingNewRelatedObject
    {
      get { return _changingNewRelatedObject; }
    }

    public IDomainObject ChangedOldRelatedObject
    {
      get { return _changedOldRelatedObject; }
    }

    public IDomainObject ChangedNewRelatedObject
    {
      get { return _changedNewRelatedObject; }
    }

    public bool HasDeletingEventBeenCalled
    {
      get { return _hasDeletingEventBeenCalled; }
    }

    public bool HasDeletedEventBeenCalled
    {
      get { return _hasDeletedEventBeenCalled; }
    }

    public bool HasCommittingEventBeenCalled
    {
      get { return _hasCommittingEventBeenCalled; }
    }

    public bool HasCommittedEventBeenCalled
    {
      get { return _hasCommittedEventBeenCalled; }
    }

    public bool HasRollingBackEventBeenCalled
    {
      get { return _hasRollingBackEventBeenCalled; }
    }

    public bool HasRolledBackEventBeenCalled
    {
      get { return _hasRolledBackEventBeenCalled; }
    }

    private void DomainObject_PropertyChanging (object sender, PropertyChangeEventArgs args)
    {
      _hasChangingEventBeenCalled = true;
      _changingPropertyDefinition = args.PropertyDefinition;
      _changingOldValue = args.OldValue;
      _changingNewValue = args.NewValue;

      VerifyTransaction();

      if (_cancel)
        CancelOperation();
    }

    private void DomainObject_PropertyChanged (object sender, PropertyChangeEventArgs args)
    {
      _hasChangedEventBeenCalled = true;
      _changedPropertyDefinition = args.PropertyDefinition;
      _changedOldValue = args.OldValue;
      _changedNewValue = args.NewValue;

      VerifyTransaction();
    }

    protected virtual void DomainObject_RelationChanging (object sender, RelationChangingEventArgs args)
    {
      _hasRelationChangingEventBeenCalled = true;
      _changingRelationPropertyName = args.RelationEndPointDefinition.PropertyName;
      _changingOldRelatedObject = args.OldRelatedObject;
      _changingNewRelatedObject = args.NewRelatedObject;

      VerifyTransaction();

      if (_cancel)
        CancelOperation();
    }

    protected virtual void DomainObject_RelationChanged (object sender, RelationChangedEventArgs args)
    {
      _hasRelationChangedEventBeenCalled = true;
      _changedRelationPropertyName = args.RelationEndPointDefinition.PropertyName;
      _changedOldRelatedObject = args.OldRelatedObject;
      _changedNewRelatedObject = args.NewRelatedObject;

      VerifyTransaction();
    }

    protected virtual void domainObject_Deleting (object sender, EventArgs args)
    {
      if (_cancel)
        CancelOperation();

      VerifyTransaction();

      _hasDeletingEventBeenCalled = true;
    }

    protected virtual void domainObject_Deleted (object sender, EventArgs e)
    {
      _hasDeletedEventBeenCalled = true;

      VerifyTransaction();
    }

    private void DomainObject_Committing (object sender, EventArgs e)
    {
      if (_hasCommittingEventBeenCalled)
        throw CreateApplicationException("Committing event on DomainObject '{0}' has already been called.", _domainObject.ID);

      VerifyTransaction();

      if (_cancel)
        CancelOperation();

      _hasCommittingEventBeenCalled = true;
    }

    private void DomainObject_Committed (object sender, EventArgs e)
    {
      if (_hasCommittedEventBeenCalled)
        throw CreateApplicationException("Committed event on DomainObject '{0}' has already been called.", _domainObject.ID);

      _hasCommittedEventBeenCalled = true;

      VerifyTransaction();
    }

    private void DomainObject_RollingBack (object sender, EventArgs e)
    {
      if (_hasRollingBackEventBeenCalled)
        throw CreateApplicationException("RollingBack event on DomainObject '{0}' has already been called.", _domainObject.ID);

      VerifyTransaction();

      if (_cancel)
        CancelOperation();

      _hasRollingBackEventBeenCalled = true;
    }

    private void DomainObject_RolledBack (object sender, EventArgs e)
    {
      if (_hasRolledBackEventBeenCalled)
        throw CreateApplicationException("RolledBack event on DomainObject '{0}' has already been called.", _domainObject.ID);

      _hasRolledBackEventBeenCalled = true;

      VerifyTransaction();
    }

    private void VerifyTransaction ()
    {
      if (_transactionToVerify != null && ClientTransaction.Current != _transactionToVerify)
        throw CreateApplicationException("The event did not set the right transaction.");
    }

    private ApplicationException CreateApplicationException (string message, params object[] args)
    {
      return new ApplicationException(string.Format(message, args));
    }
  }
}
