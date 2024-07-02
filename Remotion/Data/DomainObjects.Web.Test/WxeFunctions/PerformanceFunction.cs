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
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  public class PerformanceFunction : WxeFunction
  {
    public PerformanceFunction ()
      : base(WxeTransactionMode.CreateRootWithAutoCommit)
    {
      ReturnUrl = "default.aspx";
    }

    [WxeParameter(0, true)]
    public int ItemCount
    {
      get { return (int)Variables["ItemCount"]; }
      set { Variables["ItemCount"] = value; }
    }

    public ClassForRelationTest[] Items
    {
      get { return (ClassForRelationTest[])Variables["Items"]; }
      set { Variables["Items"] = value; }
    }

    private void Step1 ()
    {
      var items = new List<ClassForRelationTest>();
      for (int i = 0; i < ItemCount; i++)
      {
        var item = ClassForRelationTest.NewObject();
        item.Name = "Item " + i;
        item.ClassWithAllDataTypesMandatory = AttachObjectWithAllDataTypes(i, item);

        items.Add(item);
      }

      Items = items.ToArray();
    }

    private WxePageStep Step2 = new WxePageStep("Performance/Form.aspx");

    private ClassWithAllDataTypes AttachObjectWithAllDataTypes (int i, ClassForRelationTest parent)
    {
      var item = ClassWithAllDataTypes.NewObject();
      item.Int32Property = i;
      item.DateProperty = DateTime.Today;
      item.StringProperty = "Child " + i;
      item.ClassForRelationTestMandatory = parent;
      return item;
    }
  }
}
