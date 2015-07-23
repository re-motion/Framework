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
      ArgumentUtility.CheckNotNull ("table", table);

      var providers = GetFormGridRowProvider();
      
      var stringCollection = new StringCollection ();
      stringCollection.AddRange (providers.SelectMany (p => p.GetHiddenRows ((TSelf) this, table, GetFormGridManager())).ToArray());

      return stringCollection;
    }

    FormGridRowInfoCollection IFormGridRowProvider.GetAdditionalRows (HtmlTable table)
    {
      ArgumentUtility.CheckNotNull ("table", table);

      var providers = GetFormGridRowProvider();
      return new FormGridRowInfoCollection (providers.SelectMany (p => p.GetAdditionalRows ((TSelf) this, table, GetFormGridManager())).ToArray());
    }

    private IEnumerable<IOrganizationalStructureEditControlFormGridRowProvider<TSelf>> GetFormGridRowProvider ()
    {
      return ServiceLocator.GetAllInstances<IOrganizationalStructureEditControlFormGridRowProvider<TSelf>>();
    }
  }
}