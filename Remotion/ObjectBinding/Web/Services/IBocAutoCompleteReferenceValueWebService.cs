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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Services
{
  /// <summary>
  /// The <see cref="IBocAutoCompleteReferenceValueWebService"/> interface defines the API for retrieving on-demand information required by <see cref="BocAutoCompleteReferenceValue"/>.
  /// </summary>
  public interface IBocAutoCompleteReferenceValueWebService : IBocReferenceValueBaseWebService
  {
    /// <summary>
    /// Retrieves a list of objects based on the <paramref name="searchString"/> and the search context (i.e. the <paramref name="businessObjectClass"/> etc).
    /// </summary>
    /// <param name="searchString">The <see cref="string"/> all returned values must match.</param>
    /// <param name="completionSetOffset">
    ///   The number of items to skip before returning results or zero if no offset should be used and results should be returned from the beginning.
    /// </param>
    /// <param name="completionSetCount">
    ///   The maximum number of items to be returned or <see langword="null" /> if the search service implementation can define the result set size.
    /// </param>
    /// <param name="context">
    ///   Arbitrary context information that will be sent back to the server as part of the next pagination request.
    /// </param>
    /// <param name="businessObjectClass">
    ///   The <see cref="IBusinessObjectClass.Identifier"/> of the <see cref="IBusinessObjectClass"/> the control is bound to or <see langword="null" />.
    ///   This value is either the <see cref="IBusinessObject.BusinessObjectClass"/> of the bound <see cref="IBusinessObject"/> or the
    ///   <see cref="IBusinessObjectDataSource.BusinessObjectClass"/> of the <see cref="IBusinessObjectDataSource"/>.
    /// </param>
    /// <param name="businessObjectProperty">
    ///   The <see cref="IBusinessObjectProperty.Identifier"/> of the bound <see cref="IBusinessObjectProperty"/> or <see langword="null" />.
    /// </param>
    /// <param name="businessObject">
    ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the bound <see cref="IBusinessObjectWithIdentity"/> or <see langword="null" />
    ///   if the bound object only implements the <see cref="IBusinessObject"/> interface.
    /// </param>
    /// <param name="args">Additional search arguments or <see langword="null" />.</param>
    /// <returns>A <see cref="BocAutoCompleteReferenceValueSearchResult"/> instance.</returns>
    /// <remarks>
    ///   This method can be implemented by delegating the search to <see cref="ISearchAvailableObjectsService"/>.<see cref="ISearchAvailableObjectsService.Search"/>.
    /// </remarks>
    BocAutoCompleteReferenceValueSearchResult Search (
        string searchString,
        int completionSetOffset,
        int? completionSetCount,
        string? context,
        string? businessObjectClass,
        string? businessObjectProperty,
        string? businessObject,
        string? args);

    /// <summary>
    /// Retrieves a single object that exactly matches the <paramref name="searchString"/> and the search context (i.e. the <paramref name="businessObjectClass"/> etc).
    /// </summary>
    /// <param name="searchString">
    /// The <see cref="string"/> all returned values must match.</param>
    /// <param name="businessObjectClass">
    /// The <see cref="IBusinessObjectClass.Identifier"/> of the <see cref="IBusinessObjectClass"/> the control is bound to or <see langword="null" />.
    /// This value is either the <see cref="IBusinessObject.BusinessObjectClass"/> of the bound <see cref="IBusinessObject"/> or the 
    /// <see cref="IBusinessObjectDataSource.BusinessObjectClass"/> of the <see cref="IBusinessObjectDataSource"/>.
    /// </param>
    /// <param name="businessObjectProperty">
    ///   The <see cref="IBusinessObjectProperty.Identifier"/> of the bound <see cref="IBusinessObjectProperty"/> or <see langword="null" />.
    /// </param>
    /// <param name="businessObject">
    ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the bound <see cref="IBusinessObjectWithIdentity"/> or <see langword="null" />
    ///   if the bound object only implements the <see cref="IBusinessObject"/> interface.
    /// </param>
    /// <param name="args">Additional search arguments or <see langword="null" />.</param>
    /// <returns>A <see cref="BusinessObjectWithIdentityProxy"/> object if an exact match (i.e. a single match) is found or <see langword="null" />.</returns>
    /// <remarks>
    ///   This method can be implemented by delegating the search to <see cref="ISearchAvailableObjectsService"/>.<see cref="ISearchAvailableObjectsService.Search"/>
    ///   and selecting the first result.
    /// </remarks>
    BusinessObjectWithIdentityProxy? SearchExact (
        string searchString,
        string? businessObjectClass,
        string? businessObjectProperty,
        string? businessObject,
        string? args);
  }
}
