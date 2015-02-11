<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Account_Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <form role="form">
        <div class="form-group">
            <label for="UserName">UserName:</label>
            <input type="text" class="form-control" id="UserNameInput" />
        </div>
        <div class="form-group">
            <label for="Password">Password:</label>
            <input type="password" class="form-control" id="PasswordInput" />
        </div>
        <div>
            <a href="#">Forget your UserName or Password?</a>
        </div>
        <button type="submit" class="btn btn-default">Submit</button>
    </form>
</asp:Content>