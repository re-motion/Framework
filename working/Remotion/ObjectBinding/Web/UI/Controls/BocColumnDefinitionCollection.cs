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
using System.Drawing.Design;
using Remotion.ObjectBinding.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Controls
{

/// <summary> A collection of <see cref="BocColumnDefinition"/> objects. </summary>
[Editor (typeof (BocColumnDefinitionCollectionEditor), typeof (UITypeEditor))]
public class BocColumnDefinitionCollection: BusinessObjectControlItemCollection
{
  public BocColumnDefinitionCollection (IBusinessObjectBoundWebControl ownerControl)
    : base (ownerControl, new Type[] {typeof (BocColumnDefinition)})
  {
  }

  public new BocColumnDefinition[] ToArray()
  {
    return (BocColumnDefinition[]) InnerList.ToArray (typeof (BocColumnDefinition));
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new BocColumnDefinition this[int index]
  {
    get { return (BocColumnDefinition) List[index]; }
    set { List[index] = value; }
  }
}

}
