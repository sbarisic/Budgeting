<%@ Page Title="Settings Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageData.aspx.cs" Inherits="Budgeting._ManageData" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="row justify-content-md-center">
            <h1 class="h3 mb-3 font-weight-normal">Insert Data</h1>
        </div>

        <div id="divDateFromTo" class="row justify-content-md-center" runat="server" visible="false">
            <div class="form-group mr-3 ml-3">
                <label>Start Date</label>
                <input id="dateBegin" type="date" name="bday" max="3000-12-31" min="1000-01-01" class="form-control" runat="server">
            </div>
            <div class="form-group mr-3 ml-3">
                <label>End Date</label>
                <input id="dateEnd" type="date" name="bday" min="1000-01-01" max="3000-12-31" class="form-control" runat="server">
            </div>
        </div>

        <div class="row justify-content-md-center">
            <asp:Button ID="btnConfirm" CssClass="btn btn-lg btn-primary btn-block mt-4" Text="Confirm" OnClick="Confirm_Click" runat="server" />
        </div>

        <div class="row justify-content-md-center">
            <h1 id="labelError" class="h4 mt-3 font-weight-normal text-danger" runat="server" visible="false">Error</h1>
        </div>
    </div>
</asp:Content>
