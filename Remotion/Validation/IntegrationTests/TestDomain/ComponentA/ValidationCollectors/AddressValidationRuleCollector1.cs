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

namespace Remotion.Validation.IntegrationTests.TestDomain.ComponentA.ValidationCollectors
{
  public class AddressValidationRuleCollector1 : ValidationRuleCollectorBase<Address>
  {
    public AddressValidationRuleCollector1 ()
    {
      AddRule(o => o.PostalCode).SetCondition(IsCountryEqualToDeutschland).Matches("^DE");

      AddRule(a => a.City).SetCondition(IsStreetEqualToMariaHilferstrasse145).Matches("Wien");
      AddRule(a => a.PostalCode).SetCondition(IsStreetEqualToMariaHilferstrasse145).Matches("1090");

      AddRule(a => a.PostalCode).NotNull();

      AddRule(a => a.Street).MaxLength(25);

      bool IsCountryEqualToDeutschland (Address o) => o.Country == "Deutschland";

      bool IsStreetEqualToMariaHilferstrasse145 (Address o) => o.Street == "Maria Hilferstrasse 145";
    }
  }
}
