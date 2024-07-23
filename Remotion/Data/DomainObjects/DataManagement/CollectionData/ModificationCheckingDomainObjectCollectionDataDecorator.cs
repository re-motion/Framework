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

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// Implements a decorator for <see cref="IDomainObjectCollectionData"/> that performs semantic checks on the arguments passed to 
  /// <see cref="Insert"/>, <see cref="Replace"/>, and <see cref="Remove(Remotion.Data.DomainObjects.DomainObject)"/>. Those checks are
  /// performed in a decorator so that they lead to exceptions before any events are raised by <see cref="EventRaisingDomainObjectCollectionDataDecorator"/>.
  /// </summary>
  public class ModificationCheckingDomainObjectCollectionDataDecorator : DomainObjectCollectionDataDecoratorBase
  {
    private readonly Type? _requiredItemType;

    public ModificationCheckingDomainObjectCollectionDataDecorator (Type? requiredItemType, IDomainObjectCollectionData wrappedData)
      : base(wrappedData)
    {
      _requiredItemType = requiredItemType;
    }

    public override Type? RequiredItemType
    {
      get { return _requiredItemType; }
    }

    public override void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      if (index < 0 || index > Count)
      {
        throw new ArgumentOutOfRangeException(
            "index",
            index,
            "Index is out of range. Must be non-negative and less than or equal to the size of the collection.");
      }

      if (ContainsObjectID(domainObject.ID))
        throw new ArgumentException(string.Format("The collection already contains an object with ID '{0}'.", domainObject.ID), "domainObject");

      CheckItemType(domainObject, "domainObject");

      base.Insert(index, domainObject);
    }

    public override bool Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      var existingObject = GetObject(domainObject.ID);
      if (existingObject != null && existingObject != domainObject)
      {
        var message = "The object to be removed has the same ID as an object in this collection, but is a different object reference.";
        throw new ArgumentException(message, "domainObject");
      }

      return base.Remove(domainObject);
    }

    public override void Replace (int index, DomainObject value)
    {
      ArgumentUtility.CheckNotNull("value", value);

      if (index < 0 || index >= Count)
      {
        throw new ArgumentOutOfRangeException(
            "index",
            index,
            "Index is out of range. Must be non-negative and less than the size of the collection.");
      }

      if (ContainsObjectID(value.ID) && !ReferenceEquals(GetObject(index), value))
      {
        var message = string.Format("The collection already contains an object with ID '{0}'.", value.ID);
        throw new InvalidOperationException(message);
      }

      CheckItemType(value, "value");

      base.Replace(index, value);
    }

    private void CheckItemType (DomainObject domainObject, string argumentName)
    {
      if (_requiredItemType != null && !_requiredItemType.IsInstanceOfType(domainObject))
      {
        string message = string.Format(
            "Values of type '{0}' cannot be added to this collection. Values must be of type '{1}' or derived from '{1}'.",
            domainObject.GetPublicDomainObjectType(),
            _requiredItemType);
        throw new ArgumentException(message, argumentName);
      }
    }
  }
}
