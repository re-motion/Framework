using System;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation
{
  public class BocAutoCompleteReferenceValueConstraintVisitor
      : IBusinessObjectConstraintVisitor<BusinessObjectPropertyValueRequiredConstraint>
  {
    public BocAutoCompleteReferenceValue Control { get; }

    public BocAutoCompleteReferenceValueConstraintVisitor (BocAutoCompleteReferenceValue control)
    {
      ArgumentUtility.CheckNotNull(nameof(control), control);

      Control = control;
    }

    public void Visit (BusinessObjectPropertyValueRequiredConstraint constraint)
    {
      ArgumentUtility.CheckNotNull(nameof(constraint), constraint);

      ((IBusinessObjectBoundEditableWebControl)Control).RequiredByPropertyConstraint = constraint.IsRequired;
    }
  }
}
