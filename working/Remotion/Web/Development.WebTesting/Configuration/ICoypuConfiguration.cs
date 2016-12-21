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
using Coypu;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Provides all the necessary information to initialize Coypu <see cref="Options"/>.
  /// </summary>
  public interface ICoypuConfiguration
  {
    /// <summary>
    /// Specifies how long the Coypu engine should maximally search for a web element or try to interact with a web element before it fails.
    /// </summary>
    TimeSpan SearchTimeout { get; }

    /// <summary>
    /// Whenever the element to be interacted with is not ready, visible or otherwise not present, the Coypu engine automatically retries the action
    /// after the given <see cref="RetryInterval"/> until the <see cref="SearchTimeout"/> has been reached.
    /// </summary>
    TimeSpan RetryInterval { get; }
  }
}