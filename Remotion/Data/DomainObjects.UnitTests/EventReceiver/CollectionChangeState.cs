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
  public class CollectionChangeState : ChangeState
  {
    // types

    // static members and constants

    // member fields

    private DomainObject _domainObject;

    // construction and disposing

    public CollectionChangeState (object sender, DomainObject domainObject)
      : this(sender, domainObject, null)
    {
    }

    public CollectionChangeState (object sender, DomainObject domainObject, string message)
      : base(sender, message)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      _domainObject = domainObject;
    }

    // methods and properties

    public DomainObject DomainObject
    {
      get { return _domainObject; }
    }

    public override void Check (ChangeState expectedState)
    {
      base.Check(expectedState);

      CollectionChangeState collectionChangeState = (CollectionChangeState)expectedState;

      if (!ReferenceEquals(_domainObject, collectionChangeState.DomainObject))
      {
        throw CreateApplicationException(
            "Affected actual DomainObject '{0}' and expected DomainObject '{1}' do not match.",
            _domainObject.ID,
            collectionChangeState.DomainObject.ID);
      }
    }
  }
}
