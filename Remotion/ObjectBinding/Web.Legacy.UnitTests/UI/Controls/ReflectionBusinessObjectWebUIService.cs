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
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Legacy.UnitTests.UI.Controls
{
  public class ReflectionBusinessObjectWebUIService : IBusinessObjectWebUIService
  {
    public IconInfo GetIcon (IBusinessObject obj)
    {
      string url;
      if (obj == null)
        url = "~/Images/NullIcon.gif";
      else
        url = "~/Images/" + ((BindableObjectClass) obj.BusinessObjectClass).TargetType.FullName + ".gif";
      return new IconInfo (url, Unit.Pixel (16), Unit.Pixel (16));
    }

    public string GetToolTip (IBusinessObject obj)
    {
      throw new NotImplementedException();
    }

    public HelpInfo GetHelpInfo (
        IBusinessObjectBoundWebControl control,
        IBusinessObjectClass businessObjectClass,
        IBusinessObjectProperty businessObjectProperty,
        IBusinessObject businessObject)
    {
      throw new NotImplementedException();
    }
  }
}