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
<%@ Page language="c#" Codebehind="UndefinedEnumTest.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Data.DomainObjects.Web.Test.UndefinedEnumTestPage" %>

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
<h2>UndefinedEnumValueTest</h2>
      <TABLE id="SearchFormGrid" cellSpacing="0" cellPadding="0" width="300" border="0" runat="server">
        <TR>
          <TD style="WIDTH: 214px">Neues Objekt (1):</TD>
          <TD style="WIDTH: 403px"><remotion:bocenumvalue id="NewObjectEnumProperty" runat="server" DataSourceControl="NewObjectWithUndefinedEnumDataSource" PropertyIdentifier="UndefinedEnum" Required="true">
<ListControlStyle>
</ListControlStyle>
            </remotion:bocenumvalue></TD>
        </TR>
        <TR>
          <TD style="WIDTH: 214px"> Bestehendes Objekt (2):</TD>
          <TD style="WIDTH: 403px"><remotion:bocenumvalue id="ExistingObjectEnumProperty" runat="server" DataSourceControl="ExistingObjectWithUndefinedEnumDataSource" PropertyIdentifier="UndefinedEnum" Required="true">
<ListControlStyle>
</ListControlStyle>
            </remotion:bocenumvalue></TD>
        </TR>
        <TR>
          <TD style="WIDTH: 214px">
      <P>Search Objekt (3):</P></TD>
          <TD style="WIDTH: 403px"><remotion:bocenumvalue id="SearchObjectEnumProperty" runat="server" DataSourceControl="SearchObjectWithUndefinedEnumDataSource" PropertyIdentifier="UndefinedEnum">
<ListControlStyle>
</ListControlStyle>
            </remotion:bocenumvalue></TD>
        </TR>
      </TABLE>
<P>Visuelle Checks des gerenderten BocEnumValue 
Controls:</P>
<UL>
  <LI>"Value1" muss überall auszuwählen sein. 
  <LI>"Undefined" darf nirgends zur Auswahl stehen.
  <LI>Bei (1) muss ein Stern zu sehen und initial eine 
  leere Zeile ausgewält sein. <BR>Es muss ein Validator anspringen, 
  solange&nbsp;nicht "Value1"&nbsp;ausgewählt ist. 
  <LI>Bei (2) muss ein Stern zu sehen sein und&nbsp;es darf keine leere Zeile 
  auswählbar sein.&nbsp; 
  <LI>Bei (3)&nbsp;darf kein Stern zu sehen sein&nbsp;und 
  eine leere Zeile muss zur Auswahl stehen.</LI></UL>
<P>Für den&nbsp;Abschluss des Tests muss "Value1" bei (1) ausgewählt werden und 
beim Klicken auf "Test fortsetzen" darf keine Exception kommen.</P>
<P>
      <asp:button id="TestButton" runat="server" Text="Test fortsetzten"></asp:button><remotion:formgridmanager id="FormGridManager" runat="server"></remotion:formgridmanager><remotion:BindableObjectDataSourceControl id="ExistingObjectWithUndefinedEnumDataSource" runat="server" Type="Remotion.Data.DomainObjects.Web.Test.Domain.ClassWithUndefinedEnum, Remotion.Data.DomainObjects.Web.Test"></remotion:BindableObjectDataSourceControl><remotion:BindableObjectDataSourceControl id="NewObjectWithUndefinedEnumDataSource" runat="server" Type="Remotion.Data.DomainObjects.Web.Test.Domain.ClassWithUndefinedEnum, Remotion.Data.DomainObjects.Web.Test"></remotion:BindableObjectDataSourceControl>
      <remotion:BindableObjectDataSourceControl id="SearchObjectWithUndefinedEnumDataSource" runat="server" Type="Remotion.Data.DomainObjects.Web.Test.Domain.SearchObjectWithUndefinedEnum, Remotion.Data.DomainObjects.Web.Test"></remotion:BindableObjectDataSourceControl></P></form>
  </body>
</HTML>
