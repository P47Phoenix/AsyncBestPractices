<%@ Page Title="Weather" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Weather.aspx.cs" Inherits="WebApplication.Weather" Async="false" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <asp:DataGrid runat="server" ID="m_datagrid_weather" ClientIDMode="Static">
    </asp:DataGrid>
</asp:Content>
