<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <h2>You need to grant access to GIT</h2>
    
        <form action="websites/RequestToken" method="post" >
            <input  type="submit" value="Grant DropBox Access"/>
        </form>
        
        <a href="#" onclick="window.DropBoxOAuth.login()">LOGIN alternate</a>
</asp:Content>
