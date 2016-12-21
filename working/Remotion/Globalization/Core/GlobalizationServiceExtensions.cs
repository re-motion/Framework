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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  /// <summary>
  /// Defines extension methods for the <see cref="IGlobalizationService"/> interface.
  /// </summary>
  public static class GlobalizationServiceExtensions
  {
    /// <summary>
    /// Gets the <see cref="IResourceManager"/> for the specified <see cref="Type"/>.
    /// </summary>
    [NotNull]
    public static IResourceManager GetResourceManager ([NotNull] this IGlobalizationService globalizationService, [NotNull] Type type)
    {
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);
      ArgumentUtility.CheckNotNull ("type", type);

      return globalizationService.GetResourceManager (TypeAdapter.Create (type));
    }
  }
}