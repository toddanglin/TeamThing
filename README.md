#TeamThing&trade; Demo App

The TeamThing demo app is designed to show-off the capabilities of [Kendo UI Mobile](http://www.kendoui.com/mobile), a HTML5 mobile app framework that helps developers create apps that automatically adapt to the look-and-feel of different devices.

**NOTE: TeamThing is not an official Kendo UI product. It is an unsupported demo, meant to be used as a learning tool. Use at your own risk.**

##What is TeamThing?
TeamThing is an app that helps teams easily keep track of what everyone is doing. It's like a task list for teams.

Users can do the following things in the TeamThing app:

* Join a team
* Create new Things
* Update Thing status (InProgress/Completed/Delayed/Deleted)
* Browse personal Things for current team
* View other team members

##How do you use TeamThing?
To use TeamThing, simply browse to the [TeamThing app](http://teamthing.apphb.com/mobile/index.html) from any supported mobile device. In the current version, iOS, Android, and BlackBerry are supported.

To use the demo, you can log-in with the following credentials:

* **User:** demo@demo.com
* **Pass:** 1234

TeamThing will automatically adapt to the look-and-feel of your device, providing a "native" experience wherever you use it. No need for separate apps or code for each device.

##How is TeamThing built?
TeamThing consists of two primary pieces:

1. Kendo UI Mobile powered HTML/JavaScript front-end app
2. JSON REST API (using Web API)

The demo app is deployed to AppHarbor. An additional browser-based admin interface is also being developed.

###Kendo UI Mobile App
You can find the files specific the Kendo UI Mobile app in:

(Root) > TeamThing.Web > Mobile

This folder contains the HTML, JavaScript, and CSS needed for the mobile app.

The bulk of the app's logic is contained in two files:

* teamthing.app.js
* teamthing.data.js

The starting point for the app, which includes the Kendo UI Mobile Application initialization is contained in: 

* index.html

###REST API
The TeamThing RESTful API is built using ASP.NET MVC 4's Web API and [Telerik's free OpenAccess ORM](http://www.telerik.com/orm) communicating with a cloud-hosted SQL Server instance. The primary app endpoints can be found in:

(Root) > TeamThing.Web > Controllers > Thing/Team/UserController.cs

#Improving the Demo
TeamThing is a living demo, far from finished or perfect. You can help make the demo better! Take a look at the existing Issues on GitHub and make your own suggestions for improvement.

[TeamThing Issues on GitHub](https://github.com/toddanglin/TeamThing/issues)

There are a number of features planned but not yet implemented, including things like data security and federated log-in.


#TeamThing Service API
Documentation of the TeamThing RESTful API

##User Methods
**Search Users**  
Most standard odata search conventions can be used to search for users.
			
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Result</th>
  </tr>
  <tr>
    <td>GET</td>
    <td>/api/user<br/>
    	Example:
    	<br/>/api/user?$filter=EmailAddress ne null and tolower(EmailAddress) eq 'jholt456@gmail.com'
    </td>
    <td>SUCCESS - 200 Ok  
    <pre>[{"EmailAddress":"jholt456@gmail.com", 
    	    Id":6}]</pre>
    	FAILURE - 200 OK<br/>
    	Returns empty array for no results</td>
  </tr> 
</table>

**Sign in an Existing User**
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>POST</td>
    <td>
    	/api/user/signin
    </td>
    <td>
    	{"EmailAddress":"jholt456@gmail.com"}
    </td>
    <td>SUCCESS - 200 Ok  
    	<pre>
{"EmailAddress":"jholt456@gmail.com",
"Id":6,
"PendingTeams":[],
"Teams":[{"Administrators":[6],
			"Id":6,
			"IsPublic":true,
			"Name":"567 asdfasdf",
			"OwnerId":6},
		{"Administrators":[6],
			"Id":8,
			"IsPublic":false,
			"Name":"Test Team",
			"OwnerId":6}],
"Things":[{"Description":"Test thing",
		"Id":5,"Status":
		"InProgress"}]
}
    	</pre>
    	Failure 400: Bad Request
	Returns JSON error array
<pre>["A user does not exist 	
with this user name."]</pre>
    </td>
  </tr> 
</table>

**Register a New User**
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>POST</td>
    <td>
    	/api/user/register
    </td>
    <td>
    	{"EmailAddress":"newUser@test.com"}
    </td>
    <td>SUCCESS - 200 Ok  
    	<pre>
{"EmailAddress":"newUser@test.com",
	"Id":7,
	"PendingTeams":[],
	"Teams":[],
	"Things":[]
	}
    	</pre>
    	Failure 400: Bad Request
	Returns JSON error array
	<pre>
["A user with this email 
address has already registered!"]
</pre>
    </td>
  </tr> 
</table>


##Team Methods

**Search Teams**<br/>
Most standard odata search conventions can be used to search for teams.
			
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Result</th>
  </tr>
  <tr>
    <td>GET</td>
    <td>/api/team<br/>
    	Example:
    	<br/>/api/team?$filter=Name ne null and tolower(Name) eq 'closed team'
    </td>
    <td>SUCCESS - 200 Ok  
    <pre>
[{"Id":4,
	"IsPublic":false,
	"Name":"Closed Team",
	"PendingTeamMembers":[{"EmailAddress":"holt@telerik.com",
							"FullName":" ",
							"Id":5,
							"Role":"Viewer"}],
	"TeamMembers":[{"EmailAddress":"jholt456@gmail.com",
					"FullName":" ",
					"Id":6,
					"Role":"Administrator"}]
}]</pre>
    	FAILURE - 200 Ok  <br/>
    	Returns empty array for no results</td>
  </tr> 
</table>

**Get a Team**	
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>GET</td>
    <td>
    	/api/team/6
    </td>
    <td>    	
    </td>
    <td>SUCCESS - 200 Ok  
    	<pre>
{"Id":6,
"IsPublic":true,
"Name":"567 asdfasdf",
"PendingTeamMembers":[],
"TeamMembers":[{"EmailAddress":"jholt456@gmail.com",
				"FullName":" ",
				"Id":6,
				"Role":"Administrator"}]}
    	</pre>
    	Failure 400: Bad Request
	Returns JSON error array
	<pre>
["Invalid Team"]
</pre>
    </td>
  </tr> 
</table>


**Create a Team**
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>POST</td>
    <td>
    	/api/team
    </td>
    <td> 
    	<pre>
{"name":"asdf",
"ispublic":true,
"createdById":6}
    	</pre>
    </td>
    <td>SUCCESS - 201 Created  
    	<pre>
{"Administrators":[6],
 "Id":19,
 "IsPublic":false,
 "Name":"My new team",
 "OwnerId":6}
    	</pre>
    	Failure - 400 Bad Request<br/>
    	When any data is invalid returns JSON error array
	<pre>["A team must have a name"]</pre>
    </td>
  </tr> 
</table>
	
**Update a Team**
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>PUT</td>
    <td>
    	/api/team/8
    </td>
    <td> 
    	<pre>
{"name":
"Test Team2",
"ispublic":true,
"updatedbyid":6}
    	</pre>
    </td>
    <td>SUCCESS - 201 Created  
    	<pre>
{"Id":8,
"IsPublic":true,
"Name":"Test Team2",
"PendingTeamMembers":[],
"TeamMembers":[{"EmailAddress":"jholt456@gmail.com",
				"FullName":" ",
				"Id":6,
				"Role":"Administrator"},
				{"EmailAddress":"newUser@test.com",
				"FullName":" ",
				"Id":7,
				"Role":"Viewer"}]}
    	</pre>
    	Failure - 400 Bad Request<br/>
    	When any data is invalid returns JSON error array
	<pre>["Team name already in use"]</pre>
    </td>
  </tr> 
</table>

**Delete a Team**
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>DELETE</td>
    <td>
    	/api/team/8
    </td>
    <td> 
    	<pre>{"userId":6}</pre>
    </td>
    <td>SUCCESS - 204 No Content<br/>    
    	FAILURE - 400 Bad Request<br/>
    	When any data is invalid returns JSON error array
	<pre>["Invalid Team"]</pre>
    </td>
  </tr> 
</table>

**Add a User to Team**
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>PUT</td>
    <td>
    	/api/team/6/join
    </td>
    <td> 
    	<pre>{"userId":6}</pre>
    </td>
    <td>SUCCESS - 200 Ok 
    	<pre>
{"Id":6,"IsPublic":false,
"Name":"Test Team",
"PendingTeamMembers":[{"EmailAddress":"newUser@test.com",
					"FullName":" ",
					"Id":7,
					"Role":"Viewer"}],
"TeamMembers":[{"EmailAddress":"jholt456@gmail.com",
				"FullName":" ",
				"Id":6,
				"Role":"Administrator"}]}
    	</pre>
    	Failure - 400 Bad Request<br/>
    	When any data is invalid returns JSON error array
	<pre>["Invalid Team"]</pre>
    </td>
  </tr> 
</table>

**Approve Pending Team Member**
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>PUT</td>
    <td>
    	/api/team/6/approvemember
    </td>
    <td> 
    	<pre>{"userId":7}</pre>
    	</td>
	 <td>SUCCESS - 200 OK <br/>     
    	
    	FAILURE<br/>
    	    	
    	404 Not Found - When invalid team or user<br/>
    	Returns JSON error array
	<pre>["Invalid Team"]</pre>
    </td>
  </tr> 
</table>

**Deny User**
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>PUT</td>
    <td>
    	/api/team/6/denymember
    </td>
    <td> 
    	<pre>{"userId":7}</pre>
    	</td>
	 <td>SUCCESS - 200 OK  <br/>
    
    	FAILURE  <br/>
    	
    	404 Not Found - When invalid team or user<br/>
	400 Bad Request - When trying to deny access to team owner
	<pre>["Invalid Team"]</pre>
    </td>
  </tr> 
</table>

##Thing Methods</h2>
**Search Things**<br/>
Most standard odata search conventions can be used to search for things.
			
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Result</th>
  </tr>
  <tr>
    <td>GET</td>
    <td>/api/thing<br/>
    	Example:
    	<br/>/api/thing?$filter=Description ne null and indexof(Description, 'd') ge 1
    </td>
    <td>SUCCESS - 200 Ok  
    <pre>
[{"Description":"asdfasdf",
	"Id":3,
	"Status":"InProgress"},
  {"Description":"vcdfasdfasdf",
   "Id":4,
   "Status":"InProgress"},
  {"Description":"a sdfasdf ",
  "Id":6,
  "Status":"InProgress"}]</pre>
    	FAILURE - 200 Ok<br/>
    	Returns empty array for no results</td>
  </tr> 
</table>

**Get a Thing**	
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>GET</td>
    <td>
    	/api/thing/8
    </td>
    <td> 
    	<pre></pre>
    </td>
    <td>SUCCESS - 200 OK
    <pre>
{"Description":"a sdfasdf ",
 "Id":6,
 "Status":"InProgress"}
    </pre>
    
    	Failure - 400 Bad Request<br/>
    	When any data is invalid returns JSON error array
	<pre>["Invalid Thing"]</pre>
    </td>
  </tr> 
</table>

**Create a new Thing**
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>POST</td>
    <td>
    	/api/thing
    </td>
    <td> 
<pre>{"CreatedById":5,
"Description":"My New Thing",
"AssignedTo":[5,6], 
"teamId":10}</pre>
    </td>
    <td>SUCCESS - 200 Ok 
    	<pre>
{"AssignedTo":[{"EmailAddress":"newUser@test.com",
				"Id":7,
				"ImagePath":"\/images\/GenericUserImage.gif"}],
"DateCreated":"\/Date(1333463568863-0500)\/",
"Description":"test",
"Id":52,
"Owner":{"EmailAddress":"jholt456@gmail.com",
	"Id":11,
	"ImagePath":"\/images\/GenericUserImage.gif"},
"Status":"InProgress",
"Team":{"Administrators":[7],
	"Id":20,
	"ImagePath":
	"\/images\/GenericUserImage.gif",
	"IsPublic":false,
	"Name":"A Sweet Team",
	"OwnerId":7}}
    	</pre>
    	Failure - 400 Bad Request<br/>
    	When any data is invalid returns JSON error array
	<pre>["A thing must be assigned to 1 or more people"]</pre>
    </td>
  </tr> 
</table>

**Update a Thing**
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>PUT</td>
    <td>
    	/api/thing/8
    </td>
    <td> 
    	<pre></pre>
    </td>
    <td>SUCCESS - 200 OK  <br/>
    
    	Failure - 400 Bad Request<br/>
    	When any data is invalid returns JSON error array
	<pre>["A thing must be assigned to 1 or more people"]</pre>
    </td>
  </tr> 
</table>

**Delete a Thing**
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>DELETE</td>
    <td>
    	/api/thing/8
    </td>
    <td> 
    	<pre>{"DeletedById":6}</pre>
    </td>
    <td>SUCCESS - 204 No Content  <br/>
    
    	Failure - 400 Bad Request<br/>
    	When any data is invalid returns JSON error array
	<pre>["Invalid Thing"]</pre>
    </td>
  </tr> 
</table>

**Complete a Thing**
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>PUT</td>
    <td>
    	/api/thing/8/complete
    </td>
    <td> 
    	<pre>{"UserId":10}</pre>
    </td>
    <td>SUCCESS - 200 OK
    <pre>
{"Description":"a sdfasdf ",
 "Id":8,
 "Status":"Complete"}
    </pre>
    
    	Failure - 400 Bad Request<br/>
    	When any data is invalid returns JSON error array
	<pre>["Invalid Thing"]</pre>
    </td>
  </tr> 
</table>
				
**Update a Things Status**
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Params</th><th>Result</th>
  </tr>
  <tr>
    <td>PUT</td>
    <td>
    	/api/thing/8/updatestatus
    </td>
    <td> 
    	<pre>{"UserId":10, "Status":"Completed"}</pre>
    </td>
    <td>SUCCESS - 200 OK
    <pre>
{"Description":"a sdfasdf ",
 "Id":8,
 "Status":"Completed"}
    </pre>
    
    	Failure - 400 Bad Request <br/>
    	When any data is invalid returns JSON error array
	<pre>["Invalid Thing"]</pre>
    </td>
  </tr> 
</table>
