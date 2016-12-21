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
using System.ComponentModel;
using System.Drawing.Design;
using Remotion.ObjectBinding.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
 
/// <summary> A collection of <see cref="PropertyPathBinding"/> objects. </summary>
[Editor (typeof (PropertyPathBindingCollectionEditor), typeof (UITypeEditor))]
public class PropertyPathBindingCollection : BusinessObjectControlItemCollection
{
  public PropertyPathBindingCollection (IBusinessObjectBoundWebControl ownerControl)
    : base (ownerControl, new Type[] {typeof (PropertyPathBinding)})
  {
  }

  public new PropertyPathBinding[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (PropertyPathBinding[]) arrayList.ToArray (typeof (PropertyPathBinding));
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new PropertyPathBinding this[int index]
  {
    get { return (PropertyPathBinding) List[index]; }
    set { List[index] = value; }
  }
}

}
