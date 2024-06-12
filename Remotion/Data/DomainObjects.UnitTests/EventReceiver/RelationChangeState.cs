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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public class RelationChangeState : ChangeState
  {
    // types

    // static members and constants

    // member fields

    private string _propertyName;
    private DomainObject _oldDomainObject;
    private DomainObject _newDomainObject;

    // construction and disposing

    public RelationChangeState (
        object sender,
        string propertyName,
        DomainObject oldDomainObject,
        DomainObject newDomainObject)
      : this(sender, propertyName, oldDomainObject, newDomainObject, null)
    {
    }

    public RelationChangeState (
        object sender,
        string propertyName,
        DomainObject oldDomainObject,
        DomainObject newDomainObject,
        string message)
      : base(sender, message)
    {
      ArgumentUtility.CheckNotNull("propertyName", propertyName);

      _propertyName = propertyName;
      _oldDomainObject = oldDomainObject;
      _newDomainObject = newDomainObject;
    }

    // methods and properties

    public string PropertyName
    {
      get { return _propertyName; }
    }

    public DomainObject OldDomainObject
    {
      get { return _oldDomainObject; }
    }

    public DomainObject NewDomainObject
    {
      get { return _newDomainObject; }
    }

    public override void Check (ChangeState expectedState)
    {
      base.Check(expectedState);

      RelationChangeState relationChangeState = (RelationChangeState)expectedState;

      if (_propertyName != relationChangeState.PropertyName)
      {
        throw CreateApplicationException(
            "Actual PropertyName '{0}' and expected PropertyName '{1}' do not match.",
            _propertyName,
            relationChangeState.PropertyName);
      }

      if (!ReferenceEquals(_oldDomainObject, relationChangeState.OldDomainObject))
      {
        throw CreateApplicationException(
            "Actual old related DomainObject '{0}' and expected old related DomainObject '{1}' do not match.",
            GetObjectIDAsText(_oldDomainObject),
            GetObjectIDAsText(relationChangeState.OldDomainObject));
      }

      if (!ReferenceEquals(_newDomainObject, relationChangeState.NewDomainObject))
      {
        throw CreateApplicationException(
            "Actual new related DomainObject '{0}' and expected new related DomainObject '{1}' do not match.",
            GetObjectIDAsText(_newDomainObject),
            GetObjectIDAsText(relationChangeState.NewDomainObject));
      }
    }

    private string GetObjectIDAsText (DomainObject domainObject)
    {
      if (domainObject != null)
        return domainObject.ID.ToString();
      else
        return "null";
    }
  }
}
