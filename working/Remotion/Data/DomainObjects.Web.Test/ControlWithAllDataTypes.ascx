<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ControlWithAllDataTypes.ascx.cs" Inherits="Remotion.Data.DomainObjects.Web.Test.ControlWithAllDataTypes" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>

<remotion:formgridmanager id="FormGridManager" runat="server" visible="true"></remotion:formgridmanager>
<remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.Data.DomainObjects.Web.Test.Domain.ClassWithAllDataTypes, Remotion.Data.DomainObjects.Web.Test"></remotion:BindableObjectDataSourceControl>
<P><STRONG><FONT color="#ff3333">Achtung: Auf dieser Seite befinden sich Controls, 
die mehrfach auf die gleiche Porperty gebunden sind. Dadurch überschreiben sich 
diese gegenseitig beim Zurückspeichern der Werte. Dies bitte bei Tests 
beachten!</FONT></STRONG></P>
<TABLE id="FormGrid" cellSpacing="0" cellPadding="0" border="0" runat="server">
  <tr>
    <td></td>
    <td><h2>Standard Types</h2>
    </td>
  </tr>
  <TR>
    <TD></TD>
    <TD><remotion:bocbooleanvalue id="BocBooleanValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="BooleanProperty"></remotion:bocbooleanvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:bocenumvalue id="BocEnumValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="BooleanProperty">
        <ListControlStyle></ListControlStyle>
      </remotion:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="BocTextValue10" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ByteProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD style="HEIGHT: 25px"></TD>
    <TD style="HEIGHT: 25px"><remotion:boctextvalue id="Boctextvalue12" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:bocdatetimevalue id="Bocdatetimevalue4" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateProperty"></remotion:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD style="HEIGHT: 25px"></TD>
    <TD style="HEIGHT: 25px"><remotion:boctextvalue id="Boctextvalue7" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateTimeProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:bocdatetimevalue id="BocDateTimeValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateTimeProperty"></remotion:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:bocdatetimevalue id="BocDateTimeValue7" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ReadOnlyDateTimeProperty"></remotion:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="BocTextValue16" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DecimalProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="BocTextValue15" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DoubleProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:bocenumvalue id="BocEnumValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="EnumProperty">
        <ListControlStyle></ListControlStyle>
      </remotion:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD>
      <remotion:bocenumvalue id="Bocenumvalue5" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="EnumProperty"
        ReadOnly="True">
        <ListControlStyle></ListControlStyle>
      </remotion:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:bocenumvalue id="BocEnumValue6" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ExtensibleEnumProperty" Width="30em">
        <ListControlStyle></ListControlStyle>
      </remotion:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="BocTextValue19" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="GuidProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue11" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Int16Property">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="BocTextValue4" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Int32Property">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue20" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="SingleProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <td></td>
    <TD><remotion:boctextvalue id="BocTextValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="StringProperty">
        <TextBoxStyle TextMode="MultiLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <td></td>
    <TD><remotion:boctextvalue id="BocTextValue30" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="StringPropertyWithoutMaxLength">
        <TextBoxStyle TextMode="MultiLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:BocMultilineTextValue id="BocMultilineTextValue1" runat="server" PropertyIdentifier="StringArray" DataSourceControl="CurrentObject">
<TextBoxStyle TextMode="MultiLine">
</TextBoxStyle>
</remotion:BocMultilineTextValue></TD></TR>
  <tr>
    <td></td>
    <td><br>
      <h2>Nullable Types</h2>
    </td>
  </tr>
  <TR>
    <TD></TD>
    <TD><remotion:bocbooleanvalue id="BocBooleanValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaBooleanProperty"
        TrueDescription="Ja" FalseDescription="Nein" NullDescription="Undefiniert"></remotion:bocbooleanvalue></TD>
  </TR>
  <TR>
    <TD style="HEIGHT: 12px"></TD>
    <TD style="HEIGHT: 12px"><remotion:bocenumvalue id="BocEnumValue3" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaBooleanProperty">
        <ListControlStyle></ListControlStyle>
      </remotion:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue3" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaByteProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue13" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:bocdatetimevalue id="Bocdatetimevalue5" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateProperty"></remotion:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue8" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateTimeProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:bocdatetimevalue id="BocDateTimeValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateTimeProperty"></remotion:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue24" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDecimalProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue25" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDoubleProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
<%--  <TR>
    <TD></TD>
    <TD><remotion:bocenumvalue id="BocEnumValue6" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaEnumProperty">
        <ListControlStyle></ListControlStyle>
      </remotion:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD>
      <remotion:bocenumvalue id="Bocenumvalue7" runat="server" PropertyIdentifier="NaEnumProperty" DataSourceControl="CurrentObject"
        ReadOnly="True">
        <ListControlStyle></ListControlStyle>
      </remotion:bocenumvalue></TD>
  </TR>
--%>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue27" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaGuidProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue22" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaInt16Property">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="BocTextValue5" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaInt32Property">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue28" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaSingleProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <tr>
    <td></td>
    <td><br>
      <h2>Nullable Types with null values</h2>
    </td>
  </tr>
  <TR>
    <TD></TD>
    <TD><remotion:bocbooleanvalue id="BocBooleanValue3" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaBooleanWithNullValueProperty"
        TrueDescription="Ja" FalseDescription="Nein" NullDescription="Undefiniert"></remotion:bocbooleanvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:bocenumvalue id="BocEnumValue4" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaBooleanWithNullValueProperty">
        <ListControlStyle></ListControlStyle>
      </remotion:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue21" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaByteWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue14" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateWithNullValueProperty"
        ValueType="Date">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:bocdatetimevalue id="Bocdatetimevalue6" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateWithNullValueProperty"></remotion:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue9" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateTimeWithNullValueProperty"
        ValueType="Date">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:bocdatetimevalue id="BocDateTimeValue3" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateTimeWithNullValueProperty"></remotion:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue26" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDecimalWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="BocTextValue18" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDoubleWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
<%--  <TR>
    <TD></TD>
    <TD><remotion:bocenumvalue id="BocEnumValue8" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaEnumPropertyWithNullValueProperty">
        <ListControlStyle></ListControlStyle>
      </remotion:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD>
      <remotion:bocenumvalue id="Bocenumvalue9" runat="server" PropertyIdentifier="NaEnumPropertyWithNullValueProperty" DataSourceControl="CurrentObject"
        ReadOnly="True">
        <ListControlStyle></ListControlStyle>
      </remotion:bocenumvalue></TD>
  </TR>
--%>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue17" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaGuidWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue23" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaInt16WithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="BocTextValue6" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaInt32WithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="Boctextvalue29" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaSingleWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boctextvalue id="BocTextValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="StringWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </remotion:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:bocenumvalue id="BocEnumValue7" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ExtensibleEnumWithNullValueProperty" Width="30em">
        <ListControlStyle></ListControlStyle>
      </remotion:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:BocMultilineTextValue id="BocMultilineTextValue2" runat="server" PropertyIdentifier="NullStringArray" DataSourceControl="CurrentObject">
<TextBoxStyle TextMode="MultiLine">
</TextBoxStyle>
</remotion:BocMultilineTextValue></TD></TR>
  <tr>
    <td></td>
    <td><br>
      <h2>Reference Types</h2>
    </td>
  </tr>
  <TR>
    <TD></TD>
    <TD><remotion:bocreferencevalue id="BocReferenceValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ClassForRelationTestMandatory"
        Select="GetAllRelatedObjects">
        <PersistedCommand>
          <remotion:BocCommand Type="None"></remotion:BocCommand>
        </PersistedCommand>
      </remotion:bocreferencevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:bocreferencevalue id="BocReferenceValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ClassForRelationTestOptional"
        Select="GetAllRelatedObjects">
        <PersistedCommand>
          <remotion:BocCommand Type="None"></remotion:BocCommand>
        </PersistedCommand>
      </remotion:bocreferencevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boclist id="BocList1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ClassesForRelationTestMandatoryNavigateOnly">
        <FixedColumns>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="EnumProperty">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
        </FixedColumns>
      </remotion:boclist></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><remotion:boclist id="BocList2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ClassesForRelationTestOptionalNavigateOnly">
        <FixedColumns>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="EnumProperty">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
        </FixedColumns>
      </remotion:boclist></TD>
  </TR>
</TABLE>
<P><br>
  <asp:button id="SaveButton" Text="Speichern" Runat="server"></asp:button></P>

