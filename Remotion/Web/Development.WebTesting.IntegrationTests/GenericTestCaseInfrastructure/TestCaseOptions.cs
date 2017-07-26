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
using System.Reflection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure
{
  /// <summary>
  /// Represents options used by <see cref="TestCaseFactoryBase"/>.
  /// </summary>
  public class TestCaseOptions
  {
    public TestCaseOptions ()
    {
    }

    /// <summary>
    /// The test method that will be invoked when the test starts.
    /// </summary>
    public MethodInfo TargetMethod { get; set; }

    /// <summary>
    /// The page that will be passed to the generic page.
    /// </summary>
    public GenericPageTypes PageType { get; set; }
  }
}