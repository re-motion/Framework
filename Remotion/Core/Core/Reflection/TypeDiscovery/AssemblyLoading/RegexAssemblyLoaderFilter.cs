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
using System.Reflection;
using System.Text.RegularExpressions;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyLoading
{
  /// <summary>
  /// Filters the assemblies loaded during type discovery by name, excluding those whose names match a given regular expression. Whether to
  /// match the simple name or the full name can be specified.
  /// </summary>
  public class RegexAssemblyLoaderFilter : IAssemblyLoaderFilter
  {
    public enum MatchTargetKind { FullName, SimpleName };

    private readonly Regex _matchExpression;
    private readonly MatchTargetKind _matchTarget;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexAssemblyLoaderFilter"/> class.
    /// </summary>
    /// <param name="matchExpression">The expression to match against the assembly name.</param>
    /// <param name="matchTarget">Specifies whether to match the full names or the simple names of assemblies against the 
    /// <paramref name="matchExpression"/>.</param>
    public RegexAssemblyLoaderFilter (Regex matchExpression, MatchTargetKind matchTarget)
    {
      ArgumentUtility.CheckNotNull("matchExpression", matchExpression);
      ArgumentUtility.CheckValidEnumValue("matchTarget", matchTarget);

      _matchExpression = matchExpression;
      _matchTarget = matchTarget;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexAssemblyLoaderFilter"/> class.
    /// </summary>
    /// <param name="matchExpression">The expression to match against the assembly name. This is converted into a compiled, culture-invariant
    /// regular expression.</param>
    /// <param name="matchTarget">Specifies whether to match the full names or the simple names of assemblies against the 
    /// <paramref name="matchExpression"/>.</param>
    public RegexAssemblyLoaderFilter (string matchExpression, MatchTargetKind matchTarget)
        : this(new Regex(
            ArgumentUtility.CheckNotNull("matchExpression", matchExpression),
            // Do not use RegexOptions.Compiled because it takes several 100ms to compile long RegEx which is not offset by the calls made after cache lookups.
            // This is an issue in .NET up to at least version 4.5.1 in x64 mode.
            RegexOptions.CultureInvariant | RegexOptions.Singleline),
            matchTarget)
    {
    }

    /// <summary>
    /// Gets a string representation of the regular expression the assemblies are matched against.
    /// </summary>
    /// <value>The match expression string.</value>
    public string MatchExpressionString
    {
      get { return _matchExpression.ToString(); }
    }

    /// <summary>
    /// Determines whether the assembly of the given name should be considered for inclusion by the <see cref="FilteringAssemblyLoader"/>.
    /// Assemblies are considered only if the assembly name does not match the <see cref="MatchExpressionString"/>.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to be checked.</param>
    /// <returns>
    /// <see langword="true" /> unless the <paramref name="assemblyName"/> matches <see cref="MatchExpressionString"/>.
    /// </returns>
    public bool ShouldConsiderAssembly (AssemblyName assemblyName)
    {
      switch (_matchTarget)
      {
        case MatchTargetKind.SimpleName:
          return _matchExpression.IsMatch(assemblyName.GetNameChecked());
        default:
          return _matchExpression.IsMatch(assemblyName.FullName);
      }
    }

    /// <summary>
    /// Always returns <see langword="true" />; all filtering is performed by <see cref="ShouldConsiderAssembly"/>.
    /// </summary>
    public bool ShouldIncludeAssembly (Assembly assembly)
    {
      return true;
    }
  }
}
