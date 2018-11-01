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
using NUnit.Framework;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <summary>
  /// A <see cref="TestCaseFactoryBase"/> which uses the generic test page as test site.
  /// </summary>
  public abstract class GenericPageTestCaseFactoryBase : TestCaseFactoryBase
  {
    /// <summary>
    /// Extends <see cref="TestCaseFactoryBase.TestMethodAttribute"/> for the generic test page.
    /// </summary>
    [AttributeUsage (AttributeTargets.Method, Inherited = false)]
    protected class GenericPageTestMethodAttribute : TestMethodAttribute
    {
      public GenericPageTestMethodAttribute ()
      {
        PageType = GenericTestPageType.Default;
        SearchTimeout = SearchTimeout.UseConfigured;
      }

      /// <summary>
      /// Defines which page sections will be shown when using the generic page.
      /// </summary>
      public GenericTestPageType PageType { get; set; }

      /// <summary>
      /// Defines which SearchTimeout should be used for the Test.
      /// </summary>
      public SearchTimeout SearchTimeout { get; set; }

      /// <inheritdoc />
      public override void Apply (TestCaseData data)
      {
        base.Apply (data);

        data.SetCategory ("GenericTest");
      }
    }
  }
}