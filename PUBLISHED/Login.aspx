<%@ Page Title="" Language="C#" MasterPageFile="~/MyaFlix.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="MediaLan.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        #header {text-align:center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        
    <asp:ScriptManager ID="script1" runat="server" />
   
    <table class="login" border="0" cellpadding="0" cellspacing="0">
   <tr>
       <td colspan="2">
           <h3>
      Logon Page</h3>
       </td>
   </tr>
        <tr>
        <td>
          Username:</td>
        <td>
          <asp:TextBox ID="UserName" runat="server" /></td>
            </tr>
        <tr>
        <td colspan="2">
          <asp:RequiredFieldValidator ID="RequiredFieldValidator1" 
            ControlToValidate="UserName"
            Display="Dynamic" 
            ErrorMessage="Cannot be empty." 
            runat="server" />
        </td>
      </tr>
      <tr>
        <td>
          Password:</td>
        <td>
          <asp:TextBox ID="UserPass" TextMode="Password" 
             runat="server" />
        </td>
          </tr>
        <tr>
        <td colspan="2">
          <asp:RequiredFieldValidator ID="RequiredFieldValidator2" 
            ControlToValidate="UserPass"
            ErrorMessage="Cannot be empty." 
            runat="server" />
        </td>
      </tr>
      <tr>
        <td>
          Remember me?</td>
        <td>
          <asp:CheckBox ID="Persist" runat="server" /></td>
      </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:Button ID="Submit1" OnClick="Logon_Click" Text="Log On" 
       runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <p>
      <asp:Label ID="Msg" ForeColor="red" runat="server" />
    </p>
            </td>
        </tr>
    </table>
</asp:Content>
