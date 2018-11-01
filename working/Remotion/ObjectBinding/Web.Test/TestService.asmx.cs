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
using System.Threading;
using System.Web.Script.Services;
using System.Web.Services;

namespace OBWTest
{
  [WebService (Namespace = "http://tempuri.org/")]
  [WebServiceBinding (ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ToolboxItem (false)]
  [ScriptService]
  public class TestService : WebService, ITestService
  {
    [WebMethod]
    [ScriptMethod (UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public string DoStuff (string stringValue, int intValue)
    {
      Thread.Sleep (3000);
      return stringValue + " " + intValue * 2;
    }
  }

  public interface ITestService
  {
    string DoStuff (string stringValue, int intValue);

  }
}