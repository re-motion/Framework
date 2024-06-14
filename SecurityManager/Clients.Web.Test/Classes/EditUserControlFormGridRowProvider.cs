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
using System.Linq;
using System.Web.UI.HtmlControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.Test.Classes
{
  public class EditUserControlFormGridRowProvider : IOrganizationalStructureEditControlFormGridRowProvider<EditUserControl>
  {
    public IEnumerable<String> GetHiddenRows (EditUserControl dataEditControl, HtmlTable formGrid, FormGridManager formGridManager)
    {
      return Enumerable.Empty<string>();
    }

    public IEnumerable<FormGridRowInfo> GetAdditionalRows (EditUserControl dataEditControl, HtmlTable formGrid, FormGridManager formGridManager)
    {
      yield return new FormGridRowInfo(
          new BocTextValue { ID = "ReadOnlyUserNameField", ReadOnly = true, PropertyIdentifier = "UserName", DataSource = dataEditControl.DataSource },
          FormGridRowInfo.RowType.ControlInRowWithLabel,
          "UserNameField",
          FormGridRowInfo.RowPosition.BeforeRowWithID);
    }
  }
}
