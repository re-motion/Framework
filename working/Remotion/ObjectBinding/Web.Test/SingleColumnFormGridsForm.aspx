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

<%@ Page language="c#" Codebehind="SingleColumnFormGridsForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleColumnFormGridsForm" %><!doctype HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Singe Column Form Grids</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
    <meta content=C# name=CODE_LANGUAGE>
    <meta content=JavaScript name=vs_defaultClientScript>
    <meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><remotion:htmlheadcontents id=HtmlHeadContents runat="server"></remotion:htmlheadcontents>
    <style>
.singleColumnFormGrid TD.formGridMarkersCell { BORDER-RIGHT: white 0px solid; PADDING-RIGHT: 5px; BORDER-TOP: white 1px solid; PADDING-LEFT: 5px; PADDING-BOTTOM: 0px; BORDER-LEFT: white 0px solid; WIDTH: 100%; PADDING-TOP: 0px; BORDER-BOTTOM: white 0px solid; WHITE-SPACE: nowrap; BACKGROUND-COLOR: #e1ecfc }
.singleColumnFormGrid TD.formGridTopDataRow { BORDER-RIGHT: white 0px solid; BORDER-TOP: white 3px solid; BORDER-LEFT: white 0px solid; BORDER-BOTTOM: white 0px solid }
</style>
</head>
  <body MS_POSITIONING="FlowLayout">
	
    <form id="Form" method="post" runat="server">
<table id="MainFormGrid" style="WIDTH: 100%" runat="server" >
<tr>
<td><remotion:smartlabel id="Smartlabel4" runat="server" forcontrol="BocTextValue1" width="100%"></remotion:SmartLabel></td>
<td >
<table style="width:100%">
<tr>
<td style="width:50%; vertical-align:top">
<table id="LeftFormGrid" style="WIDTH: 100%; vertical-align:top;" runat="server">
  <tr class="singleColumnFormGrid">
    <td style="WIDTH: 0%">
      <remotion:SmartLabel id=SmartLabel1 runat="server" forcontrol="BocTextValue1" style="width:100%; white-space:nowrap;">
      </remotion:SmartLabel></td>
      <td style="DISPLAY:none"></td></tr>
      <tr>
      <td colspan="2"><remotion:BocTextValue id="BocTextValue1" runat="server" style="width:100%;">
<textboxstyle textmode="SingleLine">
</TextBoxStyle></remotion:BocTextValue></td>
      </tr>
  <tr class="singleColumnFormGrid">
    <td style="WIDTH: 0%">
      <remotion:smartlabel id="Smartlabel3" runat="server" forcontrol="BocTextValue1" style="width:100%; white-space:nowrap;">
      </remotion:smartlabel></td>
      <td style="DISPLAY:none"></td></tr>
      <tr>
      <td colspan="2"><remotion:BocList id="BocList1" runat="server">
<fixedcolumns>
<remotion:BocCommandColumnDefinition Text="first" ColumnTitle="First">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCommandColumnDefinition>
<remotion:BocCommandColumnDefinition Text="second" ColumnTitle="Second">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCommandColumnDefinition>
<remotion:BocCommandColumnDefinition Text="thrid" ColumnTitle="Third">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocCommandColumnDefinition>
</FixedColumns>
</remotion:BocList></td>
      </tr>
</table>
</td>
<td style="width:50%;vertical-align:top">
<table id="RightFormGrid" style="WIDTH: 100%;vertical-align:top" runat="server">
  <tr class="singleColumnFormGrid">
    <td style="WIDTH: 0%">
      <remotion:smartlabel id="Smartlabel2" runat="server" forcontrol="BocTextValue2" style="width:100%; white-space:nowrap;">
      </remotion:smartlabel></td>
      <td style="DISPLAY:none"></td></tr>
      <tr>
      <td colspan="2"><remotion:boctextvalue id="Boctextvalue2" runat="server" style="width:100%;">
<textboxstyle textmode="SingleLine">
</TextBoxStyle></remotion:boctextvalue></td>
      </tr>
</table>
</td>
</tr>
</table>
</td>
</tr>
</table>
<remotion:formgridmanager id="FormgridManager" runat="server"></remotion:formgridmanager>
     </form>
	
  </body>
</html>
