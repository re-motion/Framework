// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Web.UI;
using Remotion.Web.Compilation;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  [FileLevelControlBuilder(typeof(CodeProcessingUserControlBuilder))]
  public partial class ErrorMessageControl : UserControl
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties

    protected override void OnPreRender (EventArgs e)
    {
      ErrorsOnPageLabel.Text = GlobalResourcesHelper.GetString(GlobalResources.ErrorMessage);

      base.OnPreRender(e);
    }

    public void ShowError ()
    {
      ErrorsOnPageLabel.Visible = true;
    }
  }
}
