<%@ Page Title="" Language="C#" MasterPageFile="~/MyaFlix.Master" AutoEventWireup="true" CodeBehind="Library.aspx.cs" Inherits="MediaLan.Library" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentHeader" runat="server">
    <asp:Button ID="Submit1" OnClick="Signout_Click" Text="Sign Out" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:placeholder id="ph1" runat="server" />
</asp:Content>
