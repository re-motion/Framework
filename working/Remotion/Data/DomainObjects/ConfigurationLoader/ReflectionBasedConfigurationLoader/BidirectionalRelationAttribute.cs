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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Declares a relation as bidirectional.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class BidirectionalRelationAttribute: Attribute, IMappingAttribute
  {
    private readonly string _oppositeProperty;
    private string _sortExpression;

    /// <summary>
    /// Initializes a new instance of the <see cref="BidirectionalRelationAttribute"/> class with the name of the oppsite property.
    /// </summary>
    /// <param name="oppositeProperty">The name of the opposite property. Must not be <see langword="null" /> or empty.</param>
    public BidirectionalRelationAttribute (string oppositeProperty)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("oppositeProperty", oppositeProperty);

      _oppositeProperty = oppositeProperty;
    }

    public string OppositeProperty
    {
      get { return _oppositeProperty; }
    }

    /// <summary>
    /// Gets or sets an expression used to sort the relation when it is loaded from the data source. This is only valid on collection-valued properties.
    /// </summary>
    /// <remarks>The <see cref="SortExpression"/> consists of a comma-separated list of property names, each optionally followed by a direction 
    /// specification ("asc" or "desc"; the default is "asc"). The property name should be the ordinary .NET property name. To resolve ambiguities or 
    /// to sort by properties declared by subclasses of the related class, the unique re-store property identifier can be used instead.</remarks>
    /// <example>
    /// <para>
    /// On an OrderItems property: <c>Position</c>>
    /// </para>
    /// <para>
    /// On an OrderItems property: <c>Product asc, Quantity desc</c>
    /// </para>
    /// <para>
    /// On a Persons property, where some Persons are Customers which have a CustomerType property: <c>TestDomain.Customer.CustomerType</c>
    /// </para>
    /// <para>
    /// In the last example, the unique re-store property identifier was used because the CustomerType property is only available on a subclass 
    /// (Customer) of the related class (Person).
    /// </para>
    /// </example>
    public string SortExpression
    {
      get { return _sortExpression; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        value = value.Trim ();
        ArgumentUtility.CheckNotNullOrEmpty ("value", value);
        _sortExpression = StringUtility.EmptyToNull (value);
      }
    }

  }
}
