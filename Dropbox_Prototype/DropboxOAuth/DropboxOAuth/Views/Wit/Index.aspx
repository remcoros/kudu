<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    WIT
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="search-container">
    <div class="search-input">
        <label for="txtSearch">Work item Id:</label> 
        <input class='search-id' type="text" />
        <button id="btnSearch">Search</button>
     </div>
    <div class="search-result"></div>
</div>
<div class="WebHookUrl-container">
    <div class="WebHookUrl-input">
        <label for="txtWebHookUrl">WebHookUrl:</label> 
        <input class='WebHookUrl-id' type="text" />
        <button id="btnWebHookUrl">Set</button>
     </div>
    <div class="WebHookUrl-result"></div>
</div>
</asp:Content>

<asp:Content ContentPlaceHolderID="EndDocument" runat="server">
 <script type="text/javascript">
     var t = new SearchControl();
 </script>
</asp:Content>