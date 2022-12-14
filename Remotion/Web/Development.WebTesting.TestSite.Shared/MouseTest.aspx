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
<%@ Page Title="" Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="MouseTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.MouseTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <table id="table">
    <tr>
      <td>
        <div id="clickDiv" onContextMenu="return false;"></div>
        <div id="focusDiv" tabindex="1"></div>
        <div id="hoverDiv"></div>
      </td>
      <td>
        <div id="tooltipDiv" title="&NegativeMediumSpace;"></div>
      </td>
      <td>
        <select id="items">
          <option> </option>
          <option> </option>
        </select>
      </td>
    </tr>
  </table>

  <style>
    #table
    {
      margin-top: 100px;
      margin-left: 10px;
    }
    
    #clickDiv, #focusDiv, #hoverDiv, #tooltipDiv
    {
      margin-left: 5px;
      margin-top: 5px;
      width: 15px;
      height: 15px;
      background-color: black;
    }

    #tooltipDiv 
    {
      margin-left: 20px;
      margin-right: 20px; 
    }

    #focusDiv:focus
    {
      background-color: orange;
    }

    .hover, .click
    {
      background-color: red !important;
    }

    .doubleClick
    {
      background-color: green !important;
    }

    .rightclick
    {
      background-color: blue !important;
    }

  </style>
  <script>
    /* wire hover div */
    var hoverDiv = document.getElementById("hoverDiv");
    hoverDiv.onmouseenter = function ()
    {
      hoverDiv.classList.add ("hover");
    }
    hoverDiv.onmouseleave = function ()
    {
      hoverDiv.classList.remove ("hover");
    }

    /* wire click */
    var clickDiv = document.getElementById("clickDiv");
    clickDiv.onmouseup = function (event)
    {
      clickDiv.classList.remove ("click");
      clickDiv.classList.remove ("rightclick");
      clickDiv.classList.remove ("doubleClick");

      if (event.button === 0)
        clickDiv.classList.add ("click");
      else if (event.button === 2)
        clickDiv.classList.add ("rightclick");
    }

    /* wire double click */
    clickDiv.ondblclick = function (event)
    {
      clickDiv.classList.remove ("click");
      clickDiv.classList.remove ("rightclick");
      clickDiv.classList.add ("doubleClick");
    }

  </script>
</asp:Content>
