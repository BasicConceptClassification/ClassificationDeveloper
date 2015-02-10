<%@ Page Title="Contact Us" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Contact.aspx.cs" Inherits="Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Contact Us</h1></center>
        <dt>When contacting us, please make the subject clear.</dt>
        <br />
        <dt>Contact John Doe for X reasons, ect.</dt>
        <br />
        Name: John Doe<br />
        Role: Some Role<br />
        Phone: (###)###-####<br />
        <a href="mailto:someone@example.com?subject=BCC:">Email: someone@example.com</a><br />
        <br />
        Name: John Doe<br />
        Role: Some Role<br />
        Phone: (###)###-####<br />
        <a href="mailto:someone@example.com?subject=BCC:">Email: someone@example.com</a><br />
    </div>
</asp:Content>
