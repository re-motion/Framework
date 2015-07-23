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
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public interface IBocMenuItemContainer
  {
    bool IsReadOnly { get; }
    bool IsSelectionEnabled { get; }
    IBusinessObject[] GetSelectedBusinessObjects ();
    void RemoveBusinessObjects (IBusinessObject[] businessObjects);
    void InsertBusinessObjects (IBusinessObject[] businessObjects);
  }

  /// <remarks>
  ///   May only be added to an <see cref="IBusinessObjectBoundWebControl"/>.
  /// </remarks>
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class BocMenuItem : WebMenuItem
  {
    public BocMenuItem (
        string id,
        string category,
        string text,
        IconInfo icon,
        IconInfo disabledIcon,
        RequiredSelection requiredSelection,
        bool isDisabled,
        BocMenuItemCommand command)
        : this (id, category, text, icon, disabledIcon, WebMenuItemStyle.IconAndText, requiredSelection, isDisabled, command)
    {
    }

    public BocMenuItem (
        string id,
        string category,
        string text,
        IconInfo icon,
        IconInfo disabledIcon,
        WebMenuItemStyle style,
        RequiredSelection requiredSelection,
        bool isDisabled,
        BocMenuItemCommand command)
        : base (id, category, text, icon, disabledIcon, style, requiredSelection, isDisabled, command)
    {
    }

    public BocMenuItem ()
        : this (
            null,
            null,
            null,
            new IconInfo(),
            new IconInfo(),
            WebMenuItemStyle.IconAndText,
            RequiredSelection.Any,
            false,
            new BocMenuItemCommand())
    {
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "BocMenuItem"; }
    }

    public override Command Command
    {
      get { return base.Command; }
      set { base.Command = (BocCommand) value; }
    }

    /// <summary> Gets or sets the <see cref="IBusinessObjectBoundWebControl"/> to which this object belongs. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public new IBusinessObjectBoundWebControl OwnerControl
    {
      get { return (IBusinessObjectBoundWebControl) base.OwnerControlImplementation; }
      set { base.OwnerControlImplementation = value; }
    }

    protected override IControl OwnerControlImplementation
    {
      get { return OwnerControl; }
      set { OwnerControl = (IBusinessObjectBoundWebControl) value; }
    }

    protected override void OnOwnerControlChanged ()
    {
      base.OnOwnerControlChanged();
      ArgumentUtility.CheckNotNullAndType<IBocMenuItemContainer> ("OwnerControl", OwnerControl);
    }

    protected IBocMenuItemContainer BocMenuItemContainer
    {
      get { return (IBocMenuItemContainer) OwnerControl; }
    }

    public override bool EvaluateVisible ()
    {
      if (! base.EvaluateVisible())
        return false;

      bool isReadOnly = BocMenuItemContainer.IsReadOnly;
      bool isSelectionEnabled = BocMenuItemContainer.IsSelectionEnabled;

      if (Command != null)
      {
        if (! isReadOnly && Command.Show == CommandShow.ReadOnly)
          return false;
        if (isReadOnly && Command.Show == CommandShow.EditMode)
          return false;
      }
      bool isSelectionRequired = RequiredSelection == RequiredSelection.ExactlyOne
                                 || RequiredSelection == RequiredSelection.OneOrMore;
      if (!isSelectionEnabled && isSelectionRequired)
        return false;

      return true;
    }
  }
}
