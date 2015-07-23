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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Sample
{
  public class PersonTreeView : BocTreeView
  {
    public PersonTreeView ()
    {
    }

    protected override BusinessObjectPropertyTreeNodeInfo[] GetPropertyNodes (
        BusinessObjectTreeNode parentNode, IBusinessObjectWithIdentity businessObject)
    {
      BusinessObjectPropertyTreeNodeInfo[] nodeInfos;
      if (businessObject is Person)
      {
        nodeInfos = new BusinessObjectPropertyTreeNodeInfo[2];
        nodeInfos[0] = new BusinessObjectPropertyTreeNodeInfo (
            "Children",
            "ToolTip: Children",
            new IconInfo (null, Unit.Empty, Unit.Empty),
            (IBusinessObjectReferenceProperty) businessObject.BusinessObjectClass.GetPropertyDefinition ("Children"));
        nodeInfos[1] = new BusinessObjectPropertyTreeNodeInfo (
            "Jobs",
            "ToolTip: Jobs",
            new IconInfo (null, Unit.Empty, Unit.Empty),
            (IBusinessObjectReferenceProperty) businessObject.BusinessObjectClass.GetPropertyDefinition ("Jobs"));
      }
      else
        nodeInfos = new BusinessObjectPropertyTreeNodeInfo[0];

      return nodeInfos;
    }

    protected override IBusinessObjectWithIdentity[] GetBusinessObjects (
        BocTreeNode parentNode, IBusinessObjectWithIdentity parent, IBusinessObjectReferenceProperty property)
    {
      if (parent.UniqueIdentifier == new Guid (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1).ToString())
      {
        IList children = (IList) parent.GetProperty (property);
        ArrayList childrenList = new ArrayList();
        for (int i = 0; i < children.Count; i++)
        {
          if (i != 1)
            childrenList.Add (children[i]);
        }
        return (IBusinessObjectWithIdentity[]) childrenList.ToArray (typeof (IBusinessObjectWithIdentity));
      }
      return base.GetBusinessObjects (parentNode, parent, property);
    }
  }
}