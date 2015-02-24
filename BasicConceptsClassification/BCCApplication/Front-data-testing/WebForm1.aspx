<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="BCCApplication.Neo4j_text.WebForm1" %>


<form id="form1" runat="server">
    Value test :&nbsp;
    <asp:TextBox ID="Input_text" runat="server"></asp:TextBox>
    <asp:Button ID="Cleck_Button" runat="server" OnClick="Cleck_Button_Click" Text="Cleck" />
    &nbsp; works<br />
    Cleck add then pass string to textbox&nbsp; :
    <asp:Label ID="Term1" runat="server" Text="Terms"></asp:Label>
    &nbsp;&nbsp;
    <asp:Button ID="Add_term" runat="server" OnClick="Add_term_Click" Text="ADD" />
    &nbsp; works<br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
&nbsp;&nbsp;
    <asp:Button ID="add2" runat="server" OnClick="add2_Click" Text="ADD" />
    <br />
    <asp:Button ID="Search" runat="server" OnClick="Search_Click" Text="Search" />
    <br />
    <br />
    <asp:ListBox ID="ListBox1" runat="server" Height="202px" Width="302px"></asp:ListBox>
&nbsp;&nbsp;&nbsp; WORKS<br />
    <br />
    <asp:Button ID="Sort_id" runat="server" OnClick="Sort_id_Click" Text="Sort_relevent" Width="113px" />
&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="sort_11" runat="server" OnClick="sort_11_Click" Text="Sort_name" />
    <br />
    <br />
    <asp:ListBox ID="ListBox2" runat="server" Height="245px" Width="300px"></asp:ListBox>
    <br />
    <br />

    <asp:TextBox ID="in_text" runat="server"></asp:TextBox>
    <asp:Button ID="get_button" runat="server" OnClick="get_button_Click" Text="get" />

    <br />
    <br />
    <asp:ListBox ID="ListBox3" runat="server" Height="297px" Width="295px"></asp:ListBox>
    <br />
    <br />
</form>



