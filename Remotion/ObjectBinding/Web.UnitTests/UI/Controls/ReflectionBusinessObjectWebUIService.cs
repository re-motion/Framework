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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  public class ReflectionBusinessObjectWebUIService : IBusinessObjectWebUIService
  {
    public IconInfo GetIcon (IBusinessObject obj)
    {
      if (obj == null)
      {
        string url = "~/Images/NullIcon.gif";
        return new IconInfo(url, Unit.Pixel(16), Unit.Pixel(16));
      }
      else
      {
        string url = "~/Images/" + ((BindableObjectClass)obj.BusinessObjectClass).TargetType.FullName + ".gif";
        return new IconInfo(url, Unit.Pixel(16), Unit.Pixel(16));
      }
    }

    public string GetToolTip (IBusinessObject obj)
    {
      if (obj == null)
        return "No ToolTip";
      else
        return "ToolTip: " + ((BindableObjectClass)obj.BusinessObjectClass).TargetType.FullName;
    }

    public HelpInfo GetHelpInfo (
        IBusinessObjectBoundWebControl control,
        IBusinessObjectClass businessObjectClass,
        IBusinessObjectProperty businessObjectProperty,
        IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull("control", control);
      ArgumentUtility.CheckNotNull("businessObjectClass", businessObjectClass);

      return new HelpInfo(
          "fakeFallbackUrl",
          null,
          string.Format(
              "{0}\r\n{1}\r\n{2}\r\n{3}",
              control.ID,
              businessObjectClass.Identifier,
              (businessObjectProperty != null ? businessObjectProperty.Identifier : "prop"),
              (businessObject is IBusinessObjectWithIdentity ? ((IBusinessObjectWithIdentity)businessObject).DisplayName : "obj")),
          "return false;");
    }
  }
}
