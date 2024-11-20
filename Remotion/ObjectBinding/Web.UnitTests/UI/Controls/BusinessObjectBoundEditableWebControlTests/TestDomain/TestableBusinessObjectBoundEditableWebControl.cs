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
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BusinessObjectBoundEditableWebControlTests.TestDomain
{
  public class TestableBusinessObjectBoundEditableWebControl : BusinessObjectBoundEditableWebControl
  {
    private object _value;

    public new bool SaveValueToDomainModel ()
    {
      return base.SaveValueToDomainModel();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new object Value
    {
      get { return _value; }
      set { _value = value; }
    }

    public override bool HasValue
    {
      get { return _value != null; }
    }

    protected override sealed object ValueImplementation
    {
      get { return Value; }
      set { Value = value; }
    }

    public override void LoadValue (bool interim)
    {
      throw new NotImplementedException();
    }

    public override bool SaveValue (bool interim)
    {
      throw new NotImplementedException();
    }

    public override string[] GetTrackedClientIDs ()
    {
      throw new NotImplementedException();
    }

    public override bool UseLabel
    {
      get { throw new NotImplementedException(); }
    }

    protected override IEnumerable<BaseValidator> CreateValidators (bool isReadOnly)
    {
      return Enumerable.Empty<BaseValidator>();
    }

    protected override IBusinessObjectConstraintVisitor CreateBusinessObjectConstraintVisitor ()
    {
      return NullBusinessObjectConstraintVisitor.Instance;
    }
  }
}
