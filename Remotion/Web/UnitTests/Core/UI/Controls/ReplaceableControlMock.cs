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
using System.Web.UI;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.ControlReplacing;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{
  public class ReplaceableControlMock : ControlMock, INamingContainer, IReplaceableControl
  {
    private readonly LazyInitializationContainer _lazyInitializationContainer = new LazyInitializationContainer();

    protected override void OnInit (EventArgs e)
    {
      Init += delegate { Assertion.IsNotNull(Page, "Page was null."); };

      if (Replacer == null)
      {
        OnInitParameters.Item1.ReplaceAndWrap(this, this, OnInitParameters.Item2);

        if (IsInitialized)
        {
          EnsureLazyInitializationContainer();
          base.OnInit(e);
        }
      }
      else
      {
        EnsureLazyInitializationContainer();
        base.OnInit(e);
      }
    }

    public void EnsureLazyInitializationContainer ()
    {
      _lazyInitializationContainer.Ensure(base.Controls);
    }

    public override ControlCollection Controls
    {
      get { return _lazyInitializationContainer.GetControls(base.Controls); }
    }

    public bool IsInitialized
    {
      get { return _lazyInitializationContainer.IsInitialized; }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ControlReplacer Replacer { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Tuple<ControlReplacer, IStateModificationStrategy> OnInitParameters { get; set; }
  }
}
