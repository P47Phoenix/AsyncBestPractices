<%@ Page Title="Weather" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WeatherSyncToAsync.aspx.cs" Inherits="WebApplication.WeatherSyncToAsync" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <asp:DataGrid runat="server" ID="m_datagrid_weather" ClientIDMode="Static">
    </asp:DataGrid>
</asp:Content>
