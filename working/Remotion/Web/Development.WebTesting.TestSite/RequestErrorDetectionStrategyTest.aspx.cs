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
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class RequestErrorDetectionStrategyTest : WxePage
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
    }

    protected void AsyncPostback_OnClick (object sender, EventArgs e)
    {
      throw new Exception ("AsyncPostbackError");
    }

    protected void SyncPostbackError_OnClick (object sender, EventArgs e)
    {
      throw new Exception ("SyncPostbackError");
    }

    protected void SyncPostbackWithoutError_OnClick (object sender, EventArgs e)
    {
      //Do nothing
    }

    protected void SyncPostbackWithSpecialCharactersInErrorMessage_OnClick (object sender, EventArgs e)
    {
      throw new Exception ("ä&<\r\n'\"");
    }

    private static void IfYouWannTestInnerExceptions ()
    {
      try
      {
        FirstMethod();
      }
      catch (Exception ex)
      {
        throw new Exception ("Exception from ClickHandler.", ex);
      }
    }

    private static void SecondMethod ()
    {
      throw new Exception ("Exception from\r\nSecond Method.");
    }

    private static void FirstMethod ()
    {
      try
      {
        SecondMethod();
      }
      catch (Exception ex)
      {
        throw new Exception ("Exception from FirstMethod.", ex);
      }
    }
  }
}