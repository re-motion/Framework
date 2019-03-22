using System;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.Web.Services;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Services
{
  /// <summary>
  /// The <see cref="IBocTreeViewWebService"/> interface defines the API for retrieving on-demand information required by <see cref="BocReferenceValueBase"/>.
  /// </summary>
  public interface IBocReferenceValueWebService
  {
    /// <summary>
    /// Retrieves the status for a set of menu items displayed by <see cref="DropDownMenu"/> of the <see cref="BocReferenceValueBase"/>.
    /// </summary>
    /// <param name="controlID">The <see cref="Control.UniqueID"/> of the <see cref="BocReferenceValueBase"/>.</param>
    /// <param name="controlType">The assembly qualified type name of the <see cref="BocReferenceValueBase"/> instance.</param>
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
    /// <param name="arguments">Additional search arguments.</param>
    /// <param name="itemIDs">The IDs for the menu items displayed by the <see cref="DropDownMenu"/> before they are filtered based on the result.</param>
    /// <returns>An array of <see cref="BusinessObjectWithIdentityProxy"/> objects containing the search result.</returns>
    WebMenuItemProxy[] GetMenuItemStatusForOptionsMenu(
        string controlID,
        string controlType,
        string businessObjectClass,
        string businessObjectProperty,
        string businessObject,
        string arguments,
        string[] itemIDs);
  }
}