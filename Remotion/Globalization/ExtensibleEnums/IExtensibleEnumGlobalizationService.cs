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
using Remotion.ExtensibleEnums;
using Remotion.Globalization.ExtensibleEnums.Implementation;

namespace Remotion.Globalization.ExtensibleEnums
{
  /// <summary>
  ///  Defines an interface for retrieving the human-readable localized representation of the extensible-enumeration object.
  /// </summary>
  /// <seealso cref="ResourceManagerBasedExtensibleEnumGlobalizationService"/>
  /// <threadsafety static="true" instance="true" />
  public interface IExtensibleEnumGlobalizationService
  {
    /// <summary>
    /// Tries to get the human-readable extensible-enumeration name of the spefified reflection object.
    /// </summary>
    /// <param name="value">
    /// The <see cref="IExtensibleEnum"/> that defines the name for the resource lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="result">
    /// The human-readable localized representation of the <paramref name="value"/> or <see langword="null" /> if no localization could be found.
    /// </param>
    /// <returns><see langword="true" /> if a resource could be found.</returns>
    bool TryGetExtensibleEnumValueDisplayName ([NotNull] IExtensibleEnum value, out string result);
  }
}