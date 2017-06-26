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

namespace Remotion.Web.Development.WebTesting.TestSite.GenericTestPageInfrastructure
{
  public class SimplePageSetup : IPageSetup
  {
    public static readonly TestParameter[] DefaultParameters =
    {
        TestParameter.HtmlID,
        TestParameter.Index,
        TestParameter.LocalID,
        TestParameter.First,
        TestParameter.Single,
        TestParameter.TextContent,
        TestParameter.Title
    };

    private readonly Func<TestOptions, int, IControlSetup> _setup;

    private int _index;
    private readonly List<TestParameter> _parameters;

    public SimplePageSetup (Func<TestOptions, int, IControlSetup> setup)
        : this (setup, true)
    {
    }

    public SimplePageSetup (Func<TestOptions, int, IControlSetup> setup, bool addDefaultParameters, params TestParameter[] parameters)
    {
      _setup = setup;
      _parameters = new List<TestParameter> (parameters);
      if (addDefaultParameters)
        _parameters.AddRange (DefaultParameters);
    }

    public TestParameter[] Parameters
    {
      get { return _parameters.ToArray(); }
    }

    public IControlSetup CreateControlSetup (TestOptions options)
    {
      var setup = _setup (options, _index);
      _index++;
      return setup;
    }
  }
}