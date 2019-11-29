<%@ Page Title="Settings Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ExchangeRates.aspx.cs" Inherits="Budgeting._ExchangeRates" %>

<%@ Register Src="~/Controls/ListControl.ascx" TagName="List" TagPrefix="Budgeting" %>
<%@ Register Src="~/Controls/DropdownButton.ascx" TagName="DropdownButton" TagPrefix="Budgeting" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="row justify-content-md-center">
            <h1 id="titleManageData" class="h3 mb-3 font-weight-normal" runat="server">Exchange Rates</h1>
        </div>



        <%-- Selected currency --%>
        <div id="divSelVal" class="row justify-content-md-center mt-4 input-group" runat="server">
            <input id="inSelVal" type="text" class="form-control" aria-label="Text input with dropdown button" readonly="readonly" value="EUR" runat="server">
            <div class="input-group-append">
                <Budgeting:DropdownButton ID="btnDropdown" runat="server" Value="Base currency" />
            </div>
        </div>

        <%-- Data table --%>
        <div class="row justify-content-md-center">
            <asp:Table ID="DataTable" runat="server" CssClass="table borderless w-25">
            </asp:Table>
        </div>

    </div>
</asp:Content>
