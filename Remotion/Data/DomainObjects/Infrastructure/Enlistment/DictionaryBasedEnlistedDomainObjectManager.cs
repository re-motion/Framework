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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Enlistment
{
  /// <summary>
  /// Manages the enlisted objects via a <see cref="Dictionary{TKey,TValue}"/>.
  /// </summary>
  [Serializable]
  public class DictionaryBasedEnlistedDomainObjectManager : IEnlistedDomainObjectManager
  {
    private readonly Dictionary<ObjectID, int> _enlistedObjects = new Dictionary<ObjectID, int>();
    private readonly List<IDomainObject?> _enlistedObjectsList = new List<IDomainObject?>();

    public int EnlistedDomainObjectCount
    {
      get { return _enlistedObjects.Count; }
    }

    public IEnumerable<IDomainObject> GetEnlistedDomainObjects ()
    {
      for (int i = 0; i < _enlistedObjects.Count; i++)
      {
        var enlistedDomainObject = _enlistedObjectsList[i];
        if (enlistedDomainObject != null)
          yield return enlistedDomainObject;
      }
    }

    public IDomainObject? GetEnlistedDomainObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      int index;
      if (_enlistedObjects.TryGetValue(objectID, out index))
        return _enlistedObjectsList[index];
      return null;
    }

    public bool IsEnlisted (IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      return GetEnlistedDomainObject(domainObject.ID) == domainObject;
    }

    public void EnlistDomainObject (IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      IDomainObject? alreadyEnlistedObject = GetEnlistedDomainObject(domainObject.ID);
      if (alreadyEnlistedObject != null && alreadyEnlistedObject != domainObject)
      {
        string message = string.Format("A domain object instance for object '{0}' already exists in this transaction.", domainObject.ID);
        throw new InvalidOperationException(message);
      }

      if (alreadyEnlistedObject == null)
      {
        _enlistedObjects.Add(domainObject.ID, _enlistedObjectsList.Count);
        _enlistedObjectsList.Add(domainObject);
      }
    }

    public void DisenlistDomainObject (IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      int index;
      if (!_enlistedObjects.TryGetValue(domainObject.ID, out index))
        throw new InvalidOperationException(string.Format("Object '{0}' is not enlisted.", domainObject.ID));

      if (_enlistedObjectsList[index] != domainObject)
        throw new InvalidOperationException(string.Format("Object '{0}' is not enlisted.", domainObject.ID));

      _enlistedObjects.Remove(domainObject.ID);
      _enlistedObjectsList[index] = null;

      // Note: The ever growing list of enlisted objects cannot be easily compacted because the iteration in GetEnlistedDomainObjects will not take the
      // updated index into account. Since it is possible to have multiple active iterators, the correct update of the lists is a non-trivial problem.
    }
  }
}
