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
<%@ Page language="c#" Codebehind="SearchObject.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Data.DomainObjects.Web.Test.SearchObjectPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <HEAD>
    <title>SearchObject</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <remotion:htmlheadcontents id="HtmlHeadContents" runat="server"></remotion:htmlheadcontents>
  </HEAD>
  <body>
    <form id="SearchObjectForm" method="post" runat="server">
      <TABLE id="SearchFormGrid" cellSpacing="0" cellPadding="0" width="300" border="0" runat="server">
        <TR>
          <TD></TD>
          <TD><remotion:boctextvalue id="StringPropertyValue" runat="server" DataSourceControl="CurrentSearchObject"
              PropertyIdentifier="StringProperty">
              <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
            </remotion:boctextvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD><remotion:boctextvalue id="BytePropertyFromTextBox" runat="server" DataSourceControl="CurrentSearchObject"
              PropertyIdentifier="BytePropertyFrom">
              <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
            </remotion:boctextvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD><remotion:boctextvalue id="BytePropertyToTextBox" runat="server" DataSourceControl="CurrentSearchObject"
              PropertyIdentifier="BytePropertyTo">
              <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
            </remotion:boctextvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD><remotion:bocenumvalue id="EnumPropertyValue" runat="server" DataSourceControl="CurrentSearchObject" PropertyIdentifier="EnumProperty">
              <ListControlStyle></ListControlStyle>
            </remotion:bocenumvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD><remotion:bocenumvalue id="ExtensibleEnumPropertyValue" runat="server" DataSourceControl="CurrentSearchObject" PropertyIdentifier="ExtensibleEnumProperty">
              <ListControlStyle></ListControlStyle>
            </remotion:bocenumvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <remotion:BocDateTimeValue id="DatePropertyFromValue" runat="server" PropertyIdentifier="DatePropertyFrom"
              DataSourceControl="CurrentSearchObject"></remotion:BocDateTimeValue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <remotion:BocDateTimeValue id="DatePropertyToValue" runat="server" PropertyIdentifier="DatePropertyTo" DataSourceControl="CurrentSearchObject"></remotion:BocDateTimeValue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <remotion:BocDateTimeValue id="DateTimeFromValue" runat="server" DataSourceControl="CurrentSearchObject" PropertyIdentifier="DateTimePropertyFrom"></remotion:BocDateTimeValue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <remotion:BocDateTimeValue id="BocDateTimeValue2" runat="server" PropertyIdentifier="DateTimePropertyTo" DataSourceControl="CurrentSearchObject"></remotion:BocDateTimeValue></TD>
        </TR>
      </TABLE>
      <asp:button id="SearchButton" runat="server" Text="Suchen"></asp:button><remotion:boclist id="ResultList" runat="server" DataSourceControl="FoundObjects">
<FixedColumns>
<remotion:BocRowEditModeColumnDefinition SaveText="Speichern" CancelText="Abbrechen" EditText="Bearbeiten" ColumnTitle="Aktion"></remotion:BocRowEditModeColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="StringProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="ByteProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="EnumProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="ExtensibleEnumProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DateProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DateTimeProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="BooleanProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="NaBooleanProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
</FixedColumns>
      </remotion:boclist><remotion:formgridmanager id="SearchFormGridManager" runat="server"></remotion:formgridmanager><remotion:BindableObjectDataSourceControl id="FoundObjects" runat="server" Type="Remotion.Data.DomainObjects.Web.Test.Domain.ClassWithAllDataTypes, Remotion.Data.DomainObjects.Web.Test"></remotion:BindableObjectDataSourceControl>
      <remotion:BindableObjectDataSourceControl id="CurrentSearchObject" runat="server" Type="Remotion.Data.DomainObjects.Web.Test.Domain.ClassWithAllDataTypesSearch, Remotion.Data.DomainObjects.Web.Test"></remotion:BindableObjectDataSourceControl></form>
  </body>
</HTML>
