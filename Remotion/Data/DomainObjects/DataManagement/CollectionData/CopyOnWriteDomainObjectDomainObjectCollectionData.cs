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
  /// Represents a copy of another collection. The data is only copied if either this or the other collection changes.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class by default delegates to the copied collection, until it is either instructed to make a copy (<see cref="CopyOnWrite"/>), its contents
  /// changes, or the copied collection's contents changes. In order to be able to detect changes to the copied collection, that collection must
  /// be an <see cref="ObservableDomainObjectCollectionDataDecorator"/>. Only changes made through the <see cref="ObservableDomainObjectCollectionDataDecorator"/> will lead
  /// to a copy operation; changes made to the underlying data store of the copied collection (eg. by keeping the data store passed into the copied 
  /// collection when created) may lead 
  /// to copy operations being missed and must therfore be performed very carefully.
  /// </para>
  /// <para>
  /// Retrieval of this collection's underlying data store will always lead to a copy operation in order to avoid invalid behavior.
  /// </para>
  /// </remarks>
  [Serializable]
  public class CopyOnWriteDomainObjectDomainObjectCollectionData : ObservableDomainObjectCollectionDataDecoratorBase
  {
    private readonly ObservableDomainObjectCollectionDataDecorator _copiedData;

    public CopyOnWriteDomainObjectDomainObjectCollectionData (ObservableDomainObjectCollectionDataDecorator copiedData)
      : base(ArgumentUtility.CheckNotNull("copiedData", copiedData))
    {
      _copiedData = copiedData;
      _copiedData.CollectionChanging += delegate { CopyOnWrite(); };
    }

    public bool IsContentsCopied
    {
      get { return WrappedData != _copiedData; }
    }

    public void CopyOnWrite ()
    {
      if (!IsContentsCopied)
        WrappedData = new DomainObjectCollectionData(WrappedData);
    }

    public void RevertToCopiedData ()
    {
      WrappedData = _copiedData;
    }

    protected override void OnDataChanging (OperationKind operation, IDomainObject? affectedObject, int index)
    {
      CopyOnWrite();
    }

    protected override void OnDataChanged (OperationKind operation, IDomainObject? affectedObject, int index)
    {
      // nothing to do here
    }
  }
}
