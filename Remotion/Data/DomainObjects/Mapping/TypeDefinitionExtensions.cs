// // This file is part of the re-motion Core Framework (www.re-motion.org)
// // Copyright (c) rubicon IT GmbH, www.rubicon.eu
// //
// // The re-motion Core Framework is free software; you can redistribute it
// // and/or modify it under the terms of the GNU Lesser General Public License
// // as published by the Free Software Foundation; either version 2.1 of the
// // License, or (at your option) any later version.
// //
// // re-motion is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// // GNU Lesser General Public License for more details.
// //
// // You should have received a copy of the GNU Lesser General Public License
// // along with re-motion; if not, see http://www.gnu.org/licenses.
// //
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Provides extension methods for <see cref="TypeDefinition"/>.
  /// </summary>
  public static class TypeDefinitionExtensions
  {
    /// <summary>
    /// Gets a flag if objects associated with the <see cref="TypeDefinition"/> are persistent or only held within the <see cref="ClientTransaction"/>.
    /// </summary>
    /// <returns><see langword="true" /> if the <see cref="TypeDefinition.StorageEntityDefinition"/> is a <see cref="NonPersistentStorageEntity"/>.</returns>
    internal static bool IsNonPersistent (this TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      return typeDefinition.StorageEntityDefinition is NonPersistentStorageEntity;
    }
  }
}
