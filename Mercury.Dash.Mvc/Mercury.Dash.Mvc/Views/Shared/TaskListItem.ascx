<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ICollection<Mercury.Model.Task>>" %>
           <%foreach (var t in Model)
           {%>
        <li class="<%= t.Status.ToString().ToLower() %>">
        <div class="teamProfile fontborder" style='background-color:<%= t.TeamMember.ProfileColor %>;'>
        <%= t.TeamMember.FirstName.Substring(0, 1) + t.TeamMember.LastName.Substring(0, 1)%>
        </div>
        <% if (t.Status != Mercury.Model.TaskStatus.InProgress)
           { %>
           <div class="status"><%= t.Status.ToString()%></div>
        <%} %>
        <span class="tasktext"><%= t.Description%></span>
        </li>        
      <% } %>