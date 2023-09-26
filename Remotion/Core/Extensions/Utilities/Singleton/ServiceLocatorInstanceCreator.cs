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
using Remotion.ServiceLocation;

namespace Remotion.Utilities.Singleton
{
  /// <summary>
  /// Implements <see cref="IInstanceCreator{T}"/> by delegating to <see cref="ServiceLocator"/> to resolve an instance implementing
  /// <typeparamref name="T"/>.
  /// </summary>
  /// <typeparam name="T">The type to resolve from the <see cref="ServiceLocator"/>.</typeparam>
  /// <remarks>
  /// This class uses the <see cref="SafeServiceLocator"/>, so it will use an instance of <see cref="DefaultServiceLocator"/> if no other
  /// <see cref="IServiceLocator"/> was installed.
  /// </remarks>
  public class ServiceLocatorInstanceCreator<T> : IInstanceCreator<T>
  {
    public T CreateInstance ()
    {
      return SafeServiceLocator.Current.GetInstance<T>();
    }
  }
}
