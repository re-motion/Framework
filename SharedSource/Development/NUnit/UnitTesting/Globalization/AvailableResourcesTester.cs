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
using System.Linq;
using System.Reflection;
using NUnit.Framework;

// ReSharper disable once CheckNamespace
namespace Remotion.Development.NUnit.UnitTesting.Globalization
{
  public static class AvailableResourcesTester
  {
    /// <summary>
    /// Tests that the available resources declared by <typeparamref name="TAttribute"/> on <paramref name="targetAssembly"/>
    /// match the resource files in the output directory.
    /// </summary>
    public static void CheckAvailableResources<TAttribute> (Assembly targetAssembly)
      where TAttribute : Attribute
    {
      // We don't have a reference to globalization so the call has to provide the right attribute
      // Not ideal but better than hardcoding a type string here that causes the test to fail in case the attribute is renamed or moved
      var attribute = targetAssembly.GetCustomAttribute<TAttribute>();
      var availableResourcesByAssembly = ((string[])typeof(TAttribute).GetProperty("CultureNames")!.GetValue(attribute)!)
          .Distinct()
          .Select(name => name.ToLower())
          .ToArray();

      var resourceFileName = $"{targetAssembly.GetName().Name}.resources.dll";
      var assemblyDirectory = Path.GetDirectoryName(targetAssembly.Location)!;
      var availableResourcesByOutputFolder = Directory.EnumerateDirectories(assemblyDirectory)
          .Where(e => File.Exists(Path.Combine(e, resourceFileName)))
          .Select(Path.GetFileName)
          .Select(name => name?.ToLower())
          .Concat(new [] { "" }) // Assume that the "invariant" culture is always supported as it is embedded in the target assembly and not stored in a directory
          .ToArray();

      Assert.That(
          availableResourcesByOutputFolder,
          Is.EquivalentTo(availableResourcesByAssembly),
          $"The available resources declared by the assembly '{targetAssembly.GetName().Name}' does not match the available resources in the output directory.");
    }
  }
}
