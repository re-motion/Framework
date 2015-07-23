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
using System.IO;

namespace Remotion.Data.DomainObjects.DomainImplementation.Transport
{
  /// <summary>
  /// Implements a strategy to export a set of <see cref="DomainObject"/> instances to a <see cref="Stream"/>. The exported objects are
  /// wrapped as <see cref="TransportItem"/> property holders by the <see cref="DomainObjectTransporter"/> class.
  /// </summary>
  /// <remarks>
  /// Supply an implementation of this interface to 
  /// <see cref="DomainObjectTransporter.Export(System.IO.Stream,Remotion.Data.DomainObjects.DomainImplementation.Transport.IExportStrategy)"/>. The strategy
  /// must match the <see cref="IImportStrategy"/> supplied to <see cref="DomainObjectTransporter.LoadTransportData(Stream,IImportStrategy)"/>.
  /// </remarks>
  public interface IExportStrategy
  {
    /// <summary>
    /// Exports the specified transported objects.
    /// </summary>
    /// <param name="transportedObjects">The objects to be exported.</param>
    /// <param name="outputStream">The <see cref="Stream"/> to which the data should be exported.</param>
    /// <returns>A byte array representing the transported objects.</returns>
    /// <exception cref="TransportationException">The objects could not be exported using this strategy.</exception>
    void Export (Stream outputStream, TransportItem[] transportedObjects);
  }
}
