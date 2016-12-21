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

namespace Remotion.ObjectBinding
{
  /// <summary> 
  ///   The <b>IBusinessObjectReferenceProperty</b> interface is used for accessing references to other 
  ///   <see cref="IBusinessObject"/> instances.
  /// </summary>
  public interface IBusinessObjectReferenceProperty : IBusinessObjectProperty
  {
    /// <summary> Gets the class information for elements of this property. </summary>
    /// <value> 
    ///   The <see cref="IBusinessObjectClass"/> of the <see cref="IBusinessObject"/> accessed through this property. 
    ///   Must not return <see langword="null" />.
    /// </value>
    IBusinessObjectClass ReferenceClass { get; }

    /// <summary>Gets a flag indicating whether it is possible to get a list of the objects that can be assigned to this property.</summary>
    /// <returns> <see langword="true"/> if it is possible to get the available objects from the object model. </returns>
    /// <remarks>Use the <see cref="SearchAvailableObjects"/> method (or an object model specific overload) to get the list of objects.</remarks>
    bool SupportsSearchAvailableObjects { get; }

    /// <summary>Searches the object model for the <see cref="IBusinessObject"/> instances that can be assigned to this property.</summary>
    /// <param name="referencingObject"> The business object for which to search for the possible objects to be referenced. Can be <see langword="null"/>.</param>
    /// <param name="searchArguments">A parameter-object containing additional information for executing the search. Can be <see langword="null"/>.</param>
    /// <returns>A list of the <see cref="IBusinessObject"/> instances available. Must not return <see langword="null"/>.</returns>
    /// <exception cref="NotSupportedException">
    ///   Thrown if <see cref="SupportsSearchAvailableObjects"/> evaluated <see langword="false"/> but this method
    ///   has been called anyways.
    /// </exception>
    /// <remarks> 
    ///   This method is used if the seach statement is entered via the Visual Studio .NET designer, for instance in
    ///   the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValue"/> control.
    ///   <note type="inotes">
    ///     If your object model cannot evaluate a search string, but allows search through a less generic method,
    ///     provide an overload, and document that getting the list of available objects is only possible during runtime.
    ///   </note>
    /// </remarks>
    IBusinessObject[] SearchAvailableObjects (IBusinessObject referencingObject, ISearchAvailableObjectsArguments searchArguments);

    /// <summary>
    ///   Gets a flag indicating if <see cref="CreateDefaultValue"/> and <see cref="IsDefaultValue"/> may be called
    ///   to implicitly create a new <see cref="IBusinessObject"/> instance for editing in case the object reference is <see langword="null" />.
    /// </summary>
    [Obsolete ("The default value feature is not supported. (Version 1.13.142)")]
    bool SupportsDefaultValue { get; }

    /// <summary>
    ///   If <see cref="SupportsDefaultValue"/> is <see langword="true"/>, this method can be used to create a new <see cref="IBusinessObject"/> instance.
    /// </summary>
    /// <param name="referencingObject"> 
    ///   The <see cref="IBusinessObject"/> instance containing the object reference whose value will be assigned the newly created object. Can be <see langword="null"/>.
    /// </param>
    /// <exception cref="NotSupportedException"> 
    ///   Thrown if this method is called although <see cref="SupportsDefaultValue"/> evaluated <see langword="false"/>. 
    /// </exception>
    /// <remarks>
    ///   A use case for the <see cref="CreateDefaultValue"/> method is the instantiation of an business object without identity,
    ///   e.g. a <b>value object</b>. The reference to the value object can be <see langword="null"/> until one of its values
    ///   is set in the user interface.
    /// </remarks>
    [Obsolete ("The default value feature is not supported. (Version 1.13.142)")]
    IBusinessObject CreateDefaultValue (IBusinessObject referencingObject);

    /// <summary>
    ///   If <see cref="SupportsDefaultValue"/> is <see langword="true"/>, this method can be used evaluate if the <paramref name="value"/>
    ///   is equivalent to the <see cref="IBusinessObject"/> instance created when calling <see cref="CreateDefaultValue"/>.
    /// </summary>
    /// <param name="referencingObject"> 
    ///   The <see cref="IBusinessObject"/> instance containing the object reference the <paramref name="value"/> is assigned to. Can be <see langword="null"/>.
    /// </param>
    /// <param name="value">
    ///   The <see cref="IBusinessObject"/> instance to be evaluated. Must not be <see langword="null" />.
    /// </param>
    /// <param name="emptyProperties">
    ///   The list of properties that will be assigned <see langword="null"/> when the data is written back into the <paramref name="value"/>.
    ///   The properties belong to the <see cref="IBusinessObject.BusinessObjectClass"/> of the <paramref name="value"/>.
    ///   Must not be <see langword="null" />.
    /// </param>
    /// <exception cref="NotSupportedException"> 
    ///   Thrown if this method is called although <see cref="SupportsDefaultValue"/> evaluated <see langword="false"/>. 
    /// </exception>
    /// <remarks>
    ///   <para>
    ///     A use case for the <see cref="IsDefaultValue"/> method is the save operation of the <see cref="T:Remotion.ObjectBinding.BusinessObjectReferenceDataSource"/>.
    ///     If the property supports both the default value behavior and object deletion behavior, and <see cref="IsDefaultValue"/> evaluates <see langword="true" />,
    ///     the data source can delete the <paramref name="value"/> and write back <see langword="null" /> into the object reference.
    ///   </para>
    ///   <para>
    ///     In order to properly evaluate the default value semantics even before the data from the bound controls are written back into the <paramref name="value"/>,
    ///     the <paramref name="emptyProperties"/> can be evaluated by business object layer to determine if the <paramref name="value"/> would retain 
    ///     any relevant information after the data had been written back.
    ///   </para>
    /// </remarks>
    [Obsolete ("The default value feature is not supported. (Version 1.13.142)")]
    bool IsDefaultValue (IBusinessObject referencingObject, IBusinessObject value, IBusinessObjectProperty[] emptyProperties);

    /// <summary>
    ///   Gets a flag indicating if <see cref="Delete"/> may be called to automatically delete the current value of this object reference.
    /// </summary>
    [Obsolete ("The delete-object feature is not supported. (Version 1.13.142)")]
    bool SupportsDelete { get; }

    /// <summary>
    ///   If <see cref="SupportsDelete"/> is <see langword="true"/>, this method can be used to delete the current value of this object reference.
    /// </summary>
    /// <param name="referencingObject"> 
    ///   The <see cref="IBusinessObject"/> instance containing the object reference whose <paramref name="value"/> will be deleted. Can be <see langword="null"/>.
    /// </param>
    /// <param name="value">
    ///   The <see cref="IBusinessObject"/> instance to be deleted. Must not be <see langword="null" />.
    /// </param>
    /// <exception cref="NotSupportedException"> 
    ///   Thrown if this method is called although <see cref="SupportsDefaultValue"/> evaluated <see langword="false"/>. 
    /// </exception>
    [Obsolete ("The delete-object feature is not supported. (Version 1.13.142)")]
    void Delete (IBusinessObject referencingObject, IBusinessObject value);
  }
}
