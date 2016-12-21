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

namespace Remotion.Data.DomainObjects.DomainImplementation.Transport
{
  /// <summary>
  /// Indicates a problem when exporting or importing <see cref="DomainObject"/> instances using <see cref="DomainObjectTransporter"/>. Usually,
  /// the data or objects either don't match the <see cref="IImportStrategy"/> or <see cref="IExportStrategy"/> being used, or the data has become
  /// corrupted.
  /// </summary>
  public class TransportationException : DomainObjectException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TransportationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public TransportationException (string message)
        : base (message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransportationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public TransportationException (string message, Exception innerException)
      : base (message, innerException)
    {
    }

  }
}
