<%@ Page Title="Basic Concepts Classification" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BCCApplication._Default" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <h2><%: Title %></h2>
        <div>
            <asp:Label ID="LabelDescription" runat="server"></asp:Label>
        </div>
        <center>
            <asp:Label ID="SearchPrompt" runat="server" CssClass="control-label">Search for:</asp:Label>
            <asp:TextBox ID="TextBox2" runat="server" CssClass="input-xxlarge search-query" placeholder="(Search)(by)(Terms)"></asp:TextBox>
            <asp:Button ID="SearchBtn" runat="server" Text="Jump to Search page" OnClick="SearchBtn_Click" CssClass="btn"/>
        </center>
        <br />
        
        <div id="listContainer">
            <asp:TreeView ID="DataSet" runat="server" ImageSet="Arrows" 
                OnSelectedNodeChanged="DataSet_SelectedNodeChanged"
                OnTreeNodePopulate="PopulateNode">
                <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                <ParentNodeStyle Font-Bold="False" />
                <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
            </asp:TreeView>
        </div>
    </div>
</asp:Content>