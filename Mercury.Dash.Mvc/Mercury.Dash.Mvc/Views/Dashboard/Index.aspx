<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Mercury.Dash.Mvc.Controllers.DashboardViewContext>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightContent" runat="server">
<%if (Model.MissingPeople.Count() > 0)
  { %>
<div class="borderbox">
<h2 class="etch">M.I.A.</h2>
  <ul>
  <%foreach (var p in Model.MissingPeople)
    {%>
    <li><%= p.FirstName %><br /></li>      
    <%} %>
  </ul>
  </div>
<%} %>
<div class="sidebox">
<h2 class="etch">The Team</h2>
<ul class="teamlist">
<%foreach (var p in Model.Team)
    {%>
    <li>
    <div class="profileBox" style='background-color:<%= p.ProfileColor %>;'>
        <% var pTasks = Model.Tasks.Where(t => t.TeamMember == p).Select(t => t).ToList();
           if (pTasks != null && pTasks.Count > 0)
               Response.Write(pTasks.Count);
           else
               Response.Write("?");
           %>
    </div>
    <b><%= p.FirstName +' '+ p.LastName %></b><br /><i><%= p.Email %></i>
    </li>      
    <%} %>
  </ul>
</div>
<div class="sidebox">
<h2 class="etch">Team Stats</h2>
<ul class="statlist">
    <li><span><%= Model.Tasks.Count %></span> things today</li>
    <li><span><%= Model.Tasks.Where(t => t.Status == Mercury.Model.TaskStatus.Done).Count() %></span> things done</li>
    <li><span><%= Model.Obstacles.Count %></span> obstacles</li>
    <li><span><%= Model.Tasks.Where(t => t.DateCreated.Date == DateTime.Now.AddDays(-1).Date && t.Status == Mercury.Model.TaskStatus.Done).Count() %>/<%= Model.Tasks.Where(t => t.DateCreated.Date == DateTime.Now.AddDays(-1).Date).Count() %></span> done yesterday</li>
</ul>
</div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Today</h2>
    <ul class="tasklist">
    <% var todayTasks = Model.Tasks.Where(t => t.DateCreated.Date == DateTime.Now.Date).Select(t => t).OrderBy(t => t.TeamMember.FirstName).OrderBy(t => t.DateCreated).ToList();
       if (todayTasks.Count <= 0)
       {%>
           <p style="text-align:center;font-style:italic;">
           Nobody is working today. At least, not yet.
           </p>
       <%}
       else
       {%>
       <% Html.RenderPartial("TaskListItem", todayTasks); %>
      <% var yesterdayTasks = Model.Tasks.Where(t => t.DateCreated.Date == DateTime.Now.AddDays(-1).Date).Select(t => t).OrderBy(t => t.TeamMember.FirstName).OrderBy(t => t.DateCreated).ToList();
         if (yesterdayTasks.Count() > 0)
         {%>
            <h2>Yesterday</h2>
             <%Html.RenderPartial("TaskListItem", yesterdayTasks);%>
         <%}
       } %>
   </ul>
</asp:Content>

