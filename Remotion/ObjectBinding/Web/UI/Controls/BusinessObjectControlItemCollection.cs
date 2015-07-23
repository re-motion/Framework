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
using System.ComponentModel;
using Remotion.Globalization;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> A collection of <see cref="BusinessObjectControlItem"/> objects. </summary>
  public abstract class BusinessObjectControlItemCollection : ControlItemCollection
  {
    protected BusinessObjectControlItemCollection (IBusinessObjectBoundWebControl ownerControl, Type[] supportedTypes)
        : base (ownerControl, supportedTypes)
    {
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public new IBusinessObjectBoundWebControl OwnerControl
    {
      get { return (IBusinessObjectBoundWebControl) base.OwnerControl; }
      set { base.OwnerControl = value; }
    }
  }

  /// <summary>
  ///   Base class for non-UI items of business object controls. 
  /// </summary>
  /// <remarks>
  ///   Derived classes: <see cref="BocColumnDefinition"/>, <see cref="BocListView"/>, <see cref="PropertyPathBinding"/>.
  /// </remarks>
  public abstract class BusinessObjectControlItem : IControlItem
  {
    private IBusinessObjectBoundWebControl _ownerControl;

    /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
    protected virtual void OnOwnerControlChanged ()
    {
    }

    /// <summary> Gets or sets the <see cref="IBusinessObjectBoundWebControl"/> to which this item belongs. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public IBusinessObjectBoundWebControl OwnerControl
    {
      get { return _ownerControl; }
      set
      {
        if (_ownerControl != value)
        {
          _ownerControl = value;
          OnOwnerControlChanged();
        }
      }
    }

    IControl IControlItem.OwnerControl
    {
      get { return _ownerControl; }
      set { OwnerControl = (IBusinessObjectBoundWebControl) value; }
    }

    /// <summary> Not supported by base implementation. </summary>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public virtual string ItemID
    {
      get { return null; }
      set { throw new NotSupportedException ("Implement ItemID in a specialized class, if the class supports IDs."); }
    }

    public virtual void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
    }
  }
}
