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
    private readonly int _count;
    private readonly string _id;

    private bool _applied;

    protected GenericTestPageParameterBase ([NotNull] string id, int count)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      if (count < 0)
        count = 0;

      _id = id;
      _count = count;
    }

    /// <summary>
    /// <see langword="true" /> if <see cref="Apply"/> has been called, otherwise <see langword="false" />.
    /// </summary>
    public bool Applied
    {
      get { return _applied; }
    }

    /// <inheritdoc />
    public int Count
    {
      get { return _count; }
    }

    /// <inheritdoc />
    public string Name
    {
      get { return _id; }
    }

    /// <inheritdoc />
    public virtual void Apply (GenericTestPageParameter data)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("data", data);

      Assertion.IsTrue (data.ArgumentCount == _count);
      _applied = true;
    }
  }
}