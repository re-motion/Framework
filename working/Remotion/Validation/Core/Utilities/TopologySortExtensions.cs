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
using System.Collections.Generic;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Validation.Utilities
{
  /// <summary>
  /// Provides extension methods to sort directed acyclic graphs using a topology sort algorithm.
  /// </summary>
  /// <remarks>
  /// Topology sorting can be used to sort assemblies according to their references, 
  /// or types according to their inheritance structure.
  /// </remarks>
  //TODO RM-5906: Review
  public static class TopologySortExtensions
  {
    private class Node<T>
    {
      private readonly Func<T, IEnumerable<T>> _getDependencies;
      private HashSet<Node<T>> _dependencies;

      public Node (T content, Func<T, IEnumerable<T>> getDependencies, bool included)
      {
        ArgumentUtility.CheckNotNull ("content", content);
        ArgumentUtility.CheckNotNull ("getDependencies", getDependencies);
        Content = content;
        _getDependencies = getDependencies;
        Included = included;
      }

      public override string ToString ()
      {
        return string.Concat ("Node<T>(", ReferrerCount, ") of ", Content);
      }

      public Node<T> DropDependencies ()
      {
        foreach (var dependency in _dependencies)
          dependency.ReferrerCount--;
        
        return this;
      }

      public bool Included { get; private set; }

      public void CalculateDependencies (Dictionary<object, Node<T>> nodes, TopologySortMissingDependencyBehavior missingDependencies)
      {
        _dependencies = new HashSet<Node<T>>();
        foreach (T dependency in _getDependencies (Content))
        {
          Node<T> node;
          if (!nodes.TryGetValue (dependency, out node))
          {
            switch (missingDependencies)
            {
              case TopologySortMissingDependencyBehavior.Ignore:
                node = null;
                break;
              case TopologySortMissingDependencyBehavior.Respect:
                node = new Node<T> (dependency, _getDependencies, false);
                break;
              case TopologySortMissingDependencyBehavior.Include:
                node = new Node<T> (dependency, _getDependencies, true);
                break;
              default:
                throw new ArgumentOutOfRangeException ("missingDependencies");
            }
            if (node != null)
            {
              nodes.Add (node.Content, node);
              node.CalculateDependencies (nodes, missingDependencies);
            }
          }
          if (node != null)
            AddDependency (node);
        }
      }

      private void AddDependency (Node<T> node)
      {
        if (this == node)
          return;
        if (node.Included)
        {
          if (_dependencies.Contains (node))
            return;

          _dependencies.Add (node);
          if (Included)
            node.ReferrerCount++;
        }
        else
        {
          foreach (Node<T> dependencyNode in node._dependencies)
            AddDependency (dependencyNode);
        }
      }

      public T Content { get; private set; }
      public int ReferrerCount { get; private set; }
    }

    /// <summary>
    /// Sorts the objects given in <paramref name="source"/> using a topology-sort algorithm.
    /// The graph structure is defined by the <paramref name="getDependencies"/> delegate which should supply the 
    /// dependencies for each object to sort, i.e. the edges of the graph.
    /// The order of objects whose sort-order cannot be determined by the graph structure
    /// is not deterministic.
    /// Missing dependencies (not included in <paramref name="source"/>) will be ignored.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="getDependencies"></param>
    /// <returns>Returns the sorted items of the topology grouped by their hierarchy level starting with the leave-nodes.</returns>
    public static IEnumerable<IEnumerable<T>> TopologySort<T> (this IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
    {
      return TopologySort (source, getDependencies, null);
    }

    /// <summary>
    /// Sorts the objects given in <paramref name="source"/> using a topology-sort algorithm.
    /// The graph structure is defined by the <paramref name="getDependencies"/> delegate which should supply the 
    /// dependencies for each object to sort, i.e. the edges of the graph.
    /// The order of objects whose sort-order cannot be determined by the graph structure
    /// is not deterministic.
    /// Missing dependencies (not included in <paramref name="source"/>) will be ignored.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="getDependencies"></param>
    /// <returns>Returns the sorted items of the topology grouped by their hierarchy level starting with the leave-nodes.</returns>
    public static IEnumerable<IEnumerable<T>> TopologySortDesc<T> (this IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
    {
      return TopologySort (source, getDependencies, null).Reverse();
    }

    /// <summary>
    /// Sorts the objects given in <paramref name="source"/> using a topology-sort algorithm.
    /// The graph structure is defined by the <paramref name="getDependencies"/> delegate which should supply the 
    /// dependencies for each object to sort, i.e. the edges of the graph.
    /// The <paramref name="subSort"/> delegate, if supplied, specifies how objects will be sorted whose sort-order cannot 
    /// be determined by the graph structure.
    /// Missing dependencies (not included in <paramref name="source"/>) will be ignored.
    /// </summary>
    /// <returns>Returns the sorted items of the topology grouped by their hierarchy level starting with the leave-nodes.</returns>
    public static IEnumerable<IEnumerable<T>> TopologySort<T> (
        this IEnumerable<T> source,
        Func<T, IEnumerable<T>> getDependencies,
        Func<IEnumerable<T>, IEnumerable<T>> subSort
        )
    {
      return TopologySort (source, getDependencies, subSort, TopologySortMissingDependencyBehavior.Ignore);
    }

    /// <summary>
    /// Sorts the objects given in <paramref name="source"/> using a topology-sort algorithm.
    /// The graph structure is defined by the <paramref name="getDependencies"/> delegate which should supply the 
    /// dependencies for each object to sort, i.e. the edges of the graph.
    /// The <paramref name="subSort"/> delegate, if supplied, specifies how objects will be sorted whose sort-order cannot 
    /// be determined by the graph structure.
    /// Missing dependencies (not included in <paramref name="source"/>) will be ignored.
    /// </summary>
    /// <returns>Returns the sorted items of the topology grouped by their hierarchy level starting with the leave-nodes.</returns>
    public static IEnumerable<IEnumerable<T>> TopologySortDesc<T> (
        this IEnumerable<T> source,
        Func<T, IEnumerable<T>> getDependencies,
        Func<IEnumerable<T>, IEnumerable<T>> subSort
        )
    {
      return TopologySort (source, getDependencies, subSort, TopologySortMissingDependencyBehavior.Ignore).Reverse();
    }

    /// <summary>
    /// Sorts the objects given in <paramref name="source"/> using a topology-sort algorithm.
    /// The graph structure is defined by the <paramref name="getDependencies"/> delegate which should supply the 
    /// dependencies for each object to sort, i.e. the edges of the graph.
    /// The <paramref name="subSort"/> delegate, if supplied, specifies how objects will be sorted whose sort-order cannot 
    /// be determined by the graph structure.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="getDependencies"></param>
    /// <param name="subSort"></param>
    /// <param name="missingDependencies">
    /// Controls how to process dependencies which were not included in the original <paramref name="source"/>.
    /// See <see cref="TopologySortMissingDependencyBehavior"/>.
    /// </param>
    /// <returns>Returns the sorted items of the topology grouped by their hierarchy level starting with the leave-nodes.</returns>
    public static IEnumerable<IEnumerable<T>> TopologySort<T> (
        this IEnumerable<T> source,
        Func<T, IEnumerable<T>> getDependencies,
        Func<IEnumerable<T>, IEnumerable<T>> subSort,
        TopologySortMissingDependencyBehavior missingDependencies
        )
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("getDependencies", getDependencies);

      var unsorted = source.Select (content => new Node<T> (content, getDependencies, true)).ToList();
      Dictionary<object, Node<T>> nodes;
      try
      {
        nodes = unsorted.ToDictionary (node => (object) node.Content);
      }
      catch (ArgumentException e)
      {
        throw new InvalidOperationException ("elements to topology-sort are not unique or some element is null.", e);
      }
      unsorted.ForEach (node => node.CalculateDependencies (nodes, missingDependencies));

      if (missingDependencies == TopologySortMissingDependencyBehavior.Include)
        unsorted = nodes.Values.Where (node => node.Included).ToList();

      var sorted = new List<IEnumerable<T>>();
      while (unsorted.Count > 0)
      {
        var unreferred = new List<Node<T>>();
        var toBeSortedNext = new List<Node<T>>();
        foreach (var node in unsorted)
        {
          if (node.ReferrerCount == 0)
            unreferred.Add (node);
          else
            toBeSortedNext.Add (node);
        }
        if (unreferred.Count == 0)
          throw new InvalidOperationException ("Cyclic dependency detected - cannot perform topology sort");

        unsorted = toBeSortedNext;

        var unreferredContents = unreferred.Select (node => node.DropDependencies().Content);
        if (subSort != null)
          unreferredContents = subSort (unreferredContents);
        sorted.Add (unreferredContents.ToArray());
      }

      return sorted;
    }

    public static IEnumerable<IEnumerable<T>> TopologySortDesc<T> (
        this IEnumerable<T> source,
        Func<T, IEnumerable<T>> getDependencies,
        Func<IEnumerable<T>, IEnumerable<T>> subSort,
        TopologySortMissingDependencyBehavior missingDependencies
        )
    {
      return TopologySort (source, getDependencies, subSort, missingDependencies).Reverse ();
    }

  }
}