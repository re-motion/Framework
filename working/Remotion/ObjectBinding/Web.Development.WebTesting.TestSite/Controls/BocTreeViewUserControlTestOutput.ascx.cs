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
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocTreeViewUserControlTestOutput : UserControl
  {
    public void SetNormalSelectedNodeLabel (string value)
    {
      NormalSelectedNodeLabel.Text = value;
    }

    public void SetNoTopLevelExpanderSelectedNodeLabel (string value)
    {
      NoTopLevelExpanderSelectedNodeLabel.Text = value;
    }

    public void SetNoLookAheadEvaluationSelectedNodeLabel (string value)
    {
      NoLookAheadEvaluationSelectedNodeLabel.Text = value;
    }

    public void SetNoPropertyIdentifierSelectedNodeLabel (string value)
    {
      NoPropertyIdentifierSelectedNodeLabel.Text = value;
    }

    public void SetActionPerformed (string bocTreeViewId, string action, string parameter)
    {
      ActionPerformedSenderLabel.Text = bocTreeViewId;
      ActionPerformedLabel.Text = action;
      ActionPerformedParameterLabel.Text = parameter;
    }
  }
}