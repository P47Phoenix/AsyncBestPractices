﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WeatherSyncToCustomContext.aspx.cs" Inherits="WebApplication.WeatherSyncToCustomContext"  MasterPageFile="~/Site.Master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <asp:DataGrid runat="server" ID="m_datagrid_weather" ClientIDMode="Static">
    </asp:DataGrid>
</asp:Content>
