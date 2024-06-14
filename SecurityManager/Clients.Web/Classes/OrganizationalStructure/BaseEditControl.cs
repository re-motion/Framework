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
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.HtmlControls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure
{
  public abstract class BaseEditControl<TSelf> : BaseControl, IFormGridRowProvider
      where TSelf: BaseEditControl<TSelf>
  {
    protected abstract FormGridManager GetFormGridManager ();

    StringCollection IFormGridRowProvider.GetHiddenRows (HtmlTable table)
    {
      ArgumentUtility.CheckNotNull("table", table);

      var providers = GetFormGridRowProvider();

      var stringCollection = new StringCollection();
      stringCollection.AddRange(providers.SelectMany(p => p.GetHiddenRows((TSelf)this, table, GetFormGridManager())).ToArray());

      return stringCollection;
    }

    FormGridRowInfoCollection IFormGridRowProvider.GetAdditionalRows (HtmlTable table)
    {
      ArgumentUtility.CheckNotNull("table", table);

      var providers = GetFormGridRowProvider();
      return new FormGridRowInfoCollection(providers.SelectMany(p => p.GetAdditionalRows((TSelf)this, table, GetFormGridManager())).ToArray());
    }

    private IEnumerable<IOrganizationalStructureEditControlFormGridRowProvider<TSelf>> GetFormGridRowProvider ()
    {
      return ServiceLocator.GetAllInstances<IOrganizationalStructureEditControlFormGridRowProvider<TSelf>>();
    }
  }
}
