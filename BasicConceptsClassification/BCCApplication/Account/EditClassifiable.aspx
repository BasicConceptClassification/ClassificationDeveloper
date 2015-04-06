<%@ Page Title="Edit Classifiable" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="True" Inherits="BCCApplication.Account.EditClassifiable" CodeBehind="EditClassifiable.aspx.cs" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <h1><%: Title %></h1>
        <div>
            <asp:Label ID="LabelDescription" runat="server"></asp:Label>
        </div>
        <div id="EditClassifiable1">
            <table style="width: 100%;">
                <tr>
                    <td>
                        <div class="td">
                            <asp:Label runat="server" AssociatedControlID="TextBox_Name" CssClass="control-label">Name</asp:Label>
                            <asp:TextBox ID="TextBox_Name" runat="server"></asp:TextBox>
                            <br />
                            <asp:RequiredFieldValidator ID="ValidatorName" runat="server" ControlToValidate="TextBox_Name" CssClass="text-danger"
                                ErrorMessage="The name of the object is required." />
                        </div>
                    </td>
                    <td>
                        <div class="td">
                            <asp:Label runat="server" AssociatedControlID="TextBox_URL" CssClass="control-label">URL</asp:Label>
                            <asp:TextBox ID="TextBox_URL" runat="server"></asp:TextBox>
                            <br />
                            <asp:RequiredFieldValidator ID="ValidatorURL" runat="server" ControlToValidate="TextBox_URL" CssClass="text-danger"
                                ErrorMessage="The URL of the object is required." />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" AssociatedControlID="EditPerm" CssClass="control-label">Set Permission</asp:Label>
                        <asp:RadioButtonList ID="EditPerm" runat="server" CssClass="radioButtonList" RepeatDirection="Horizontal">
                            <asp:ListItem Value="GLAM" Selected="True" />
                            <asp:ListItem Value="OwnerOnly" />
                        </asp:RadioButtonList>
                    </td>
                    <td>

                        <div class="td">
                            <asp:Label runat="server" AssociatedControlID="TextBox_Concept" CssClass="control-label">Concept String</asp:Label>
                            <asp:TextBox ID="TextBox_Concept" runat="server"></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="td">
                            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                        </div>
                    </td>

                    <td>
                        <div class="td">
                            <asp:Button ID="Edit" runat="server" Text="Edit" OnClick="Edit_Click" CausesValidation="true" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="td">
                            <center>
                         <h2>Classified</h2><br />
                     <asp:ListBox ID="ListBoxClass" runat="server" Height="300px" Width="300px"></asp:ListBox>
                         <br />
                         <asp:Button ID="Update_Class" runat="server" OnClick="Update_Class_Click" Text="Get Information" CausesValidation="false" />
                         </center>
                        </div>
                    </td>
                    <td>
                        <div class="td">
                            <center>
                        <h2> UnClassified</h2><br />
                     <asp:ListBox ID="ListBox2" runat="server" Height="300px" Width="300px"></asp:ListBox>
                         <br />
                         <asp:Button ID="Update_Unclass" runat="server" OnClick="Update_Unclass_Click" Text="Get Information" CausesValidation="false" />
                     </center>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <table class="table-top-aligned" style="width:100%;">
        <tr>
            <td>
                <asp:Label ID="LabelNoticationDataSet" runat="server"></asp:Label>
                <div id="listContainer">
                    <asp:TreeView ID="DataSet" runat="server" ImageSet="Arrows"
                        OnTreeNodePopulate="PopulateNode">
                        <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                        <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                        <ParentNodeStyle Font-Bold="False" />
                        <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
                    </asp:TreeView>
                </div>
                Looking for a term not in the classification? <a href="SuggestTerm.aspx">Click here to suggest a new term.</a>
            </td>
            <td>
                <asp:Button ID="AddTOb" runat="server" Text="Add" />
                <asp:Button ID="RemoveTOb" runat="server" Text="Remove" />
                <p>Double click on the term you want to use from the list or click the Add button.<br /><br />
                    Double click on a term in the Concept String to remove it.<br /><br />
                    Click and drag terms in the Concept String to change the order.
                </p>
            </td>
        </tr>
    </table> 
    </div>
</asp:Content>
