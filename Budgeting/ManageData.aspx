<%@ Page Title="Settings Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageData.aspx.cs" Inherits="Budgeting._ManageData" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="row justify-content-md-center">
            <h1 class="h3 mb-3 font-weight-normal">Insert Data</h1>
        </div>

        <%--Main screen - choose action options--%>
        <div id="listMainOptions" runat="server" >
            <asp:Repeater ID="rptOptions" runat="server" ItemType="Budgeting.MainRadioButton">
                <ItemTemplate>
                    <div class="row justify-content-md-left w-25 mx-auto">
                        <div class="custom-control custom-radio">
                            <input type="radio" class="custom-control-input" id=<%# Item.InputID %> name="groupOfDefaultRadios">
                            <label class="custom-control-label" for=<%# Item.InputID %>><%# Item.Text %></label>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <%--Begin/end date picker--%>
        <div class="row justify-content-md-center">
            <div id="divDateFrom" class="form-group mr-3 ml-3" runat="server" visible="false">
                <label id="lblDateFrom" runat="server">Start Date</label>
                <input id="dateBegin" type="date" name="bday" max="3000-12-31" min="1000-01-01" class="form-control" runat="server">
            </div>
            <div id="divDateTo" class="form-group mr-3 ml-3" runat="server" visible="false">
                <label id="lblDateTo" runat="server">End Date</label>
                <input id="dateEnd" type="date" name="bday" min="1000-01-01" max="3000-12-31" class="form-control" runat="server">
            </div>
        </div>

        <%--Confirm button--%>
        <div class="row justify-content-md-center">
            <asp:Button ID="btnConfirm" CssClass="btn btn-lg btn-primary btn-block mt-4" Text="Confirm" OnClick="Confirm_Click" runat="server" />
        </div>

         <%--Error message--%>
        <div class="row justify-content-md-center">
            <h1 id="labelError" class="h4 mt-3 font-weight-normal text-danger" runat="server" visible="false">Error</h1>
        </div>
    </div>
</asp:Content>
