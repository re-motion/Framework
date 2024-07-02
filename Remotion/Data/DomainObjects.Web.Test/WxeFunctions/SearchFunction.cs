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
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Mixins;
using Remotion.TypePipe;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  public class SearchFunction : WxeFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SearchFunction ()
      : base(WxeTransactionMode.CreateRootWithAutoCommit)
    {
      ReturnUrl = "default.aspx";
    }

    // methods and properties

    public ClassWithAllDataTypesSearch SearchObject
    {
      get { return (ClassWithAllDataTypesSearch)Variables["SearchObject"]; }
      set { Variables["SearchObject"] = value; }
    }

    public IList Result
    {
      get { return (IList)Variables["Result"]; }
      set { Variables["Result"] = value; }
    }

    public void Requery ()
    {
      Result = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection(SearchObject.CreateQuery()).ToCustomCollection();
    }

    private void Step1 ()
    {
      SearchObject = ObjectFactory.Create<ClassWithAllDataTypesSearch>(ParamList.Empty);
      Requery();
    }

    private WxePageStep Step2 = new WxePageStep("SearchObject.aspx");
  }
}
