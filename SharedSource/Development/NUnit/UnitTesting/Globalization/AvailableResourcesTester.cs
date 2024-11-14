// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
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
