<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Budgeting._Default" %>

<%@ Register Src="~/Controls/ListControl.ascx" TagName="List" TagPrefix="Budgeting" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Table ID="DataTable" runat="server" CssClass="table borderless">
    </asp:Table>
</asp:Content>
