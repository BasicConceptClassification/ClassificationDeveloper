
<%@ Page Title="EditClassifiable" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="True" Inherits="BCCApplication.Account.EditClassifiable" CodeBehind="EditClassifiable.aspx.cs" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Edit Classifiable</h1></center>
        <div id="EditClassifiable1" runat="server">
        <table style="width:100%;">
         <tr>
             <td>
                 <div class ="td">
                     
                     Name:<asp:TextBox ID="TextBox_Name" runat="server"></asp:TextBox>
                 </div>
             </td>
             
             <td>
                 <div class ="td">
                     
                     URL:<asp:TextBox ID="TextBox_URL" runat="server"></asp:TextBox>
                 </div>
             </td>
         </tr>
            <tr>
             <td>
                 <div class ="td">
                     
                     Concept:<asp:TextBox ID="TextBox_Concept" runat="server"></asp:TextBox>
                 </div>
             </td>
             
             <td>
                 <div class ="td">
                    
                    
                    
                 </div>
             </td>
         </tr>

         <tr>
             <td>
                    <asp:RadioButtonList ID="EditPerm" runat="server" Font-Size="X-Small" RepeatColumns="2">
                        <asp:ListItem Value="GLAM" Selected="True"/>
                        <asp:ListItem Value="OwnerOnly"/>
                    </asp:RadioButtonList>
             </td>
             
             <td>
                 <div class ="td">
                    
                     <asp:Button ID="Edit" runat="server" Text="Edit" OnClick="Edit_Click" />
                    
                 </div>
             </td>
         </tr>
        <tr>
             <td>
                 <div class ="td">
                     <center>
                         <h2>Classified</h2><br />
                     <asp:ListBox ID="ListBoxClass" runat="server" Height="300px" Width="300px"></asp:ListBox>
                         <br />
                         <asp:Button ID="Update_Class" runat="server" OnClick="Update_Class_Click" Text="Update" />
                         </center>
                 </div>
             </td>
             
             <td>
                 <div class ="td">
                     <center>
                        <h2> UnClassified</h2><br />
                     <asp:ListBox ID="ListBox2" runat="server" Height="300px" Width="300px"></asp:ListBox>
                         <br />
                         <asp:Button ID="Update_Unclass" runat="server" OnClick="Update_Unclass_Click" Text="Update" />
                     </center>
                     
                 </div>
             </td>
         </tr>
         <tr>
             <div class ="td">
                 </div>
         </tr>
        </div>
    </div>
</asp:Content>
