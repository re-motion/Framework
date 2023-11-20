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
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  public class StubValueColumnDefinition : BocValueColumnDefinition
  {
    protected override IBocColumnRenderer GetRendererInternal (IServiceLocator locator)
    {
      return new StubColumnRenderer(new FakeResourceUrlFactory());
    }

    public override string GetStringValue (IBusinessObject obj)
    {
      throw new NotImplementedException();
    }

    protected override IComparer<BocListRow> CreateCellValueComparer ()
    {
      throw new NotImplementedException();
    }

    protected override IValidationFailureMatcher GetValidationFailureMatcher ()
    {
      throw new NotImplementedException();
    }
  }
}
