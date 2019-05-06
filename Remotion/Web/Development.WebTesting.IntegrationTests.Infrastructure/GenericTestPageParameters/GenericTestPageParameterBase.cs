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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.GenericTestPageParameters
{
  /// <summary>
  /// Simplifies the implementation of <see cref="IGenericTestPageParameter"/> by providing a common implementation.
  /// </summary>
  public abstract class GenericTestPageParameterBase : IGenericTestPageParameter
  {
    /// <inheritdoc />
    public int Count { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <summary>
    /// <see langword="true" /> if <see cref="Apply"/> has been called, otherwise <see langword="false" />.
    /// </summary>
    public bool Applied { get; private set; }

    private static readonly Lazy<TestConstants> s_testConstantsLazy = new Lazy<TestConstants>(() => new TestConstants());
    protected static TestConstants TestConstants => s_testConstantsLazy.Value;

    protected GenericTestPageParameterBase ([NotNull] string id, int count)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      if (count < 0)
        count = 0;

      Name = id;
      Count = count;
    }

    /// <inheritdoc />
    public virtual void Apply (GenericTestPageParameter data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      Assertion.IsTrue (data.Arguments.Count == Count);

      Applied = true;
    }
  }
}