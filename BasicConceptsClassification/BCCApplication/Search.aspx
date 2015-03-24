<%@ Page Title="Search" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="True" Inherits="Search" Codebehind="Search.aspx.cs" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Search Builder</h1></center>
        <p>Quick explanation on how to use with or w/out the buttons. For more information please go <a href="About.aspx" target="_self">about/help page</a>.</p>
        <br />
        <center>
            <asp:Label ID="SearchPrompt" runat="server" CssClass="control-label">Search for:</asp:Label>
            <asp:TextBox ID="TextBox2" runat="server" CssClass="input-xxlarge search-query" placeholder="(Search)(by)(Terms)"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" Text="Jump to Search page" OnClick="Button1_Click" CssClass="btn"/>
        </center>
    </div>
    <br />
    <div id="listContainer">
        <asp:TreeView ID="DataSet" runat="server" ImageSet="Arrows" OnSelectedNodeChanged="DataSet_SelectedNodeChanged">
            <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
            <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
            <ParentNodeStyle Font-Bold="False" />
            <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
         </asp:TreeView>
    </div>
</asp:Content>
