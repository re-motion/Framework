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
using System.ComponentModel;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{

public class SingleControlItemCollection
  : ICollection // For Designer Support. (VS2003, VS2005)
{
  private readonly Type[] _supportedTypes;
  private IControlItem? _controlItem;
  private IControl? _ownerControl;

  /// <summary> Creates a new instance. </summary>
  /// <param name="controlItem">The <see cref="IControlItem"/> to be stored in this instance.</param>
  /// <param name="supportedTypes"> Supported types must implement <see cref="IControlItem"/>. </param>
  public SingleControlItemCollection (IControlItem? controlItem, Type[] supportedTypes)
  {
    _supportedTypes = supportedTypes;
    ControlItem = controlItem;
  }

  public SingleControlItemCollection (Type[] supportedTypes)
    : this(null, supportedTypes)
  {
  }

  public IControlItem? ControlItem
  {
    get { return _controlItem; }
    set
    {
      if (value != null && ! IsSupportedType(value))
        throw ArgumentUtility.CreateArgumentTypeException("value", value.GetType(), null);
      _controlItem = value;
      if (_controlItem != null)
        _controlItem.OwnerControl = _ownerControl;
    }
  }

  public IControl? OwnerControl
  {
    get { return _ownerControl; }
    set
    {
      _ownerControl = value;
      if (_controlItem != null)
        _controlItem.OwnerControl = _ownerControl;
    }
  }

  /// <summary>Tests whether the specified control item's type is supported by the collection. </summary>
  private bool IsSupportedType (IControlItem controlItem)
  {
    Type controlItemType = controlItem.GetType();

    foreach (Type type in _supportedTypes)
    {
      if (type.IsAssignableFrom(controlItemType))
        return true;
    }

    return false;
  }

  IEnumerator IEnumerable.GetEnumerator ()
  {
     return new SingleControlItemCollectionEnumerator(_controlItem);
  }

  void ICollection.CopyTo (Array array, int index)
  {
    throw new NotSupportedException();
  }

  int ICollection.Count
  {
    get { return 1; }
  }

  bool ICollection.IsSynchronized
  {
    get { return true; }
  }

  object ICollection.SyncRoot
  {
    get { return this; }
  }

  /// <summary> For Designer Support. (VS2003, VS2005) </summary>
  /// <exclude/>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public IControlItem? this[int index]
  {
	  get
	  {
      if (index > 0) throw new NotSupportedException("Getting an element above index 0 is not implemented.");
      return ControlItem;
	  }
	  set
	  {
      if (index > 0) throw new NotSupportedException("Setting an element above index 0 is not implemented.");
      ControlItem = value;
	  }
  }

  /// <summary> For Designer Support. (VS2003, VS2005) </summary>
  /// <exclude/>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public int Add (IControlItem value)
  {
    ControlItem = value;
    return 1;
  }
}

public class SingleControlItemCollectionEnumerator: IEnumerator
{
  private readonly IControlItem? _controlItem;
  bool _isMoved;
  bool _isEnd;

  internal SingleControlItemCollectionEnumerator (IControlItem? controlItem)
  {
    _controlItem = controlItem;
    _isMoved = false;
    _isEnd = false;
  }

  public void Reset ()
  {
    _isMoved = false;
    _isEnd = false;
  }

  public object? Current
  {
    get
    {
      if (! _isMoved) throw new InvalidOperationException("The enumerator is positioned before the first element.");
      if (_isEnd) throw new InvalidOperationException("The enumerator is positioned after the last element.");
      return _controlItem;
    }
  }

  public bool MoveNext ()
  {
    if (_isMoved)
      _isEnd = true;
    _isMoved = true;
    if (_controlItem == null)
      _isEnd = true;
    return ! _isEnd;
  }
}

}
