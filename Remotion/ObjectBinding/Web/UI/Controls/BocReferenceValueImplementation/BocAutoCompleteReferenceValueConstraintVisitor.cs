using System;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation
{
  public class BocAutoCompleteReferenceValueConstraintVisitor
      : IBusinessObjectConstraintVisitor<BusinessObjectPropertyValueRequiredConstraint>
  {
    public BocAutoCompleteReferenceValue Control { get; }

    public BocAutoCompleteReferenceValueConstraintVisitor (BocAutoCompleteReferenceValue control)
    {
      ArgumentNullException.ThrowIfNull(control);

      Control = control;
    }

    public void Visit (BusinessObjectPropertyValueRequiredConstraint constraint)
    {
      ArgumentNullException.ThrowIfNull(constraint);

      ((IBusinessObjectBoundEditableWebControl)Control).RequiredByPropertyConstraint = constraint.IsRequired;
    }
  }
}
