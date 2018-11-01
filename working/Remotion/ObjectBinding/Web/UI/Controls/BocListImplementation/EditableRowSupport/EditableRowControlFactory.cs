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
using System.Linq;
using Remotion.Mixins;
using Remotion.TypePipe;
using Remotion.Utilities;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport
{
  public class EditableRowControlFactory
  {
    public static EditableRowControlFactory CreateEditableRowControlFactory ()
    {
      return ObjectFactory.Create<EditableRowControlFactory> (true, ParamList.Empty);
    }

    protected EditableRowControlFactory ()
    {
    }

    public virtual IBusinessObjectBoundEditableWebControl Create (BocSimpleColumnDefinition column, int columnIndex)
    {
      ArgumentUtility.CheckNotNull ("column", column);
      if (columnIndex < 0)
        throw new ArgumentOutOfRangeException ("columnIndex");

      IBusinessObjectBoundEditableWebControl control = column.CreateEditModeControl();

      if (control == null)
        control = CreateFromPropertyPath (column.GetPropertyPath());

      return control;
    }

    protected virtual IBusinessObjectBoundEditableWebControl CreateFromPropertyPath (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

      return (IBusinessObjectBoundEditableWebControl) ControlFactory.CreateControl (propertyPath.Properties.Last(), ControlFactory.EditMode.InlineEdit);
    }

    public virtual void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      var bocBooleanValue = new Controls.BocBooleanValue();
      bocBooleanValue.RegisterHtmlHeadContents (htmlHeadAppender);

      var bocDateTimeValue = new Controls.BocDateTimeValue();
      bocDateTimeValue.RegisterHtmlHeadContents (htmlHeadAppender);

      var bocEnumValue = new BocEnumValue();
      bocEnumValue.RegisterHtmlHeadContents (htmlHeadAppender);

      var bocMultilineTextValue = new BocMultilineTextValue();
      bocMultilineTextValue.RegisterHtmlHeadContents (htmlHeadAppender);

      var bocReferenceValue = new BocReferenceValue();
      bocReferenceValue.RegisterHtmlHeadContents (htmlHeadAppender);

      var bocTextValue = new BocTextValue();
      bocTextValue.RegisterHtmlHeadContents (htmlHeadAppender);
    }
  }
}