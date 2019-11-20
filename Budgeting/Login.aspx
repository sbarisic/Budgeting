<%@ Page Title="Settings Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Budgeting._Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="row justify-content-md-center">
            <h1 class="h3 mb-3 font-weight-normal">Please sign in</h1>
        </div>

        <div class="row justify-content-md-center">
            <label for="inputUsername" class="sr-only">Username</label>
            <input type="text" id="inputUsername" class="form-control mt-2 mb-2" placeholder="Username" required autofocus runat="server" />
        </div>

        <div class="row justify-content-md-center">
            <label for="inputPassword" class="sr-only">Password</label>
            <input type="password" id="inputPassword" class="form-control mt-2 mb-2" placeholder="Password" required runat="server" />
        </div>

        <div class="row justify-content-md-center">
            <asp:Button ID="btnSignIn" CssClass="btn btn-lg btn-primary btn-block mt-4" Text="Sign in" OnClick="SignIn_Click" runat="server" />
        </div>

        <div class="row justify-content-md-center">
            <h1 id="labelError" class="h4 mt-3 font-weight-normal text-danger" runat="server" visible="false">Error</h1>
        </div>
    </div>
</asp:Content>
