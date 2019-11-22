<%@ Page Title="Settings Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageData.aspx.cs" Inherits="Budgeting._ManageData" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="row justify-content-md-center">
            <h1 id="titleManageData" class="h3 mb-3 font-weight-normal" runat="server">Manage Data</h1>
        </div>

        <%--Main screen - choose action options--%>
        <div id="divRadioOptions" runat="server" visible="false">
            <input id="mainRadioResult" name="mainRadioResult" value="null" runat="server" hidden="hidden" />

            <asp:Repeater ID="rptOptions" runat="server" ItemType="Budgeting.MainRadioButton">
                <ItemTemplate>
                    <div class="row justify-content-md-left w-25 mx-auto">
                        <div class="custom-control custom-radio">
                            <input id="<%# Item.InputID %>" type="radio" class="custom-control-input" name="mainRadioGroup" onclick="setRadioResult('<%# Item.InputID %>')" <%# Item.CustomAttributes %>>
                            <label class="custom-control-label" for="<%# Item.InputID %>"><%# Item.Text %></label>
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



        <%-- Currency amount --%>
        <div id="divCurAmt" class="row justify-content-md-center mt-4 input-group" runat="server" visible="false">
            <input type="text" class="form-control" aria-label="Text input with dropdown button">
            <div class="input-group-append">
                <button class="btn btn-outline-secondary dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Dropdown</button>
                <div class="dropdown-menu">
                    <a class="dropdown-item" href="#">Action</a>
                    <a class="dropdown-item" href="#">Another action</a>
                    <a class="dropdown-item" href="#">Something else here</a>
                    <div role="separator" class="dropdown-divider"></div>
                    <a class="dropdown-item" href="#">Separated link</a>
                </div>
            </div>
        </div>



        <%--Confirm button--%>
        <div class="row justify-content-md-center mt-4 ">
            <asp:Button ID="btnConfirm" CssClass="btn btn-lg btn-primary btn-block " Text="Confirm" OnClick="Confirm_Click" runat="server" />
        </div>



        <%--Error message--%>
        <div class="row justify-content-md-center mt-3">
            <h1 id="labelError" class="h4 font-weight-normal text-danger" runat="server" visible="false">Error</h1>
        </div>
    </div>

    <script type="text/javascript">
        function setRadioResult(inputId) {
            let mainRadioResult = "#MainContent_mainRadioResult";

            $(mainRadioResult).val(inputId);
            console.log("Setting radio box value to " + $(mainRadioResult).val());
        }
    </script>
</asp:Content>
