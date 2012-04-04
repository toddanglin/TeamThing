#TeamThing Service API

##User Methods
**Search Users**  
Most standard odata search conventions can be used to search for users.
			
<table>
  <tr>
   <th>Request Type</th><th>Url</th><th>Result</th>
  </tr>
  <tr>
    <td>GET</td>
    <td>/api/team<br/>
    	Example:
    	<br/>/api/team?$filter=EmailAddress ne null and tolower(EmailAddress) eq 'jholt456@gmail.com'
    </td>
    <td>SUCCESS - 200 Ok  
    <pre>[{"EmailAddress":"jholt456@gmail.com", Id":6}]</pre>
    	FAILURE - 200 Ok  
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
	returns JSON error array
		<pre>["A user does not exist with this user name."]</pre>
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
["A user with this email address has already registered!"]
</pre>
    </td>
  </tr> 
</table>


##Team Methods

**Search Teams**
		Notes
			Most standard odata search conventions can be used to search for teams.
		URL
			GET: /api/team?$filter=Name ne null and tolower(Name) eq 'closed team'
		Result	
			SUCCESS - 200 Ok
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
				}]

			FAILURE - 200 Ok
				Returns empty array for no results

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
    	Failure - 400 Bad Request - When any data is invalid
	Returns JSON error array
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
{"id":8,"name":
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
    	Failure - 400 Bad Request - When any data is invalid
	Returns JSON error array
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
    <td>SUCCESS - 204 No Content  
    
    	Failure - 400 Bad Request - When any data is invalid  
    	Returns JSON error array
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
    	Failure - 400 Bad Request - When any data is invalid  
    	Returns JSON error array
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
	 <td>SUCCESS - 200 OK      
    	
    	FAILURE
    	    	
    	404 Not Found - When invalid team or user
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
	 <td>SUCCESS - 200 OK  
    
    	FAILURE  
    	
    	404 Not Found - When invalid team or user
	400 Bad Request - When trying to deny access to team owner
	<pre>["Invalid Team"]</pre>
    </td>
  </tr> 
</table>

##Thing Methods</h2>
**Search Things**
		Notes
			Most standard odata search conventions can be used to search for things.
		URL
			GET: /api/thing?$filter=Description ne null and indexof(Description, 'd') ge 1
		Result	
			SUCCESS - 200 Ok
				[{"Description":"asdfasdf",
					"Id":3,
					"Status":"InProgress"},
				  {"Description":"vcdfasdfasdf",
				   "Id":4,
				   "Status":"InProgress"},
				  {"Description":"a sdfasdf ",
				  "Id":6,
				  "Status":"InProgress"}]
			FAILURE - 200 Ok
				Returns empty array for no results

**Get a Thing**	
		URL
			GET: /api/thing/6

		Result 
			SUCCESS - 200 Ok
				{"Description":"a sdfasdf ",
				 "Id":6,
				 "Status":"InProgress"}

			FAILURE
				- 400 Bad Request
				returns JSON error array
				Example: ["Invalid Thing"]

**Create a new Thing**
		URL
			POST: /api/thing
		Params
			{"CreatedById":5,"Description":"My New Thing","AssignedTo":[5,6], "teamId":10}
		Result
			SUCCESS - 201 Created
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
					"OwnerId":7}
			}
			FAILURE - 400 Bad Request - When any data is invalid
				returns JSON error array
				Example: ["A thing must be assigned to 1 or more people"]

**Update a Thing**
		URL
			PUT: /api/thing/8
		Params	
			
		Result
			SUCCESS - 200 Ok
				
			FAILURE - 400 Bad Request - When any data is invalid
				returns JSON error array
				Example: ["A thing must be assigned to 1 or more people"]

**Delete a Thing**
		URL
			DELETE: /api/thing/8
		Params
			{"DeletedById":6}
		Results
			SUCCESS - 204 No Content
			FAILURE 
				400 Bad Request 
					returns error message string if information is missing, or if user does not have permissions to remove thing(only thing owners can delete things)

**Complete a Thing**
		URL
			PUT: /api/thing/8/complete
		Params
			{"UserId":10}
		Results
			SUCCESS - 200 Ok
				{"Description":"a sdfasdf ",
				 "Id":8,
				 "Status":"Complete"}

			FAILURE
				- 400 Bad Request
				returns JSON error array
				Example: ["Invalid Thing"]
				
**Update a Things Status**
		URL
			PUT: /api/thing/8/complete
		Params
			{"UserId":10, "Status":"Completed"}
		Results
			SUCCESS - 200 Ok
				{"Description":"a sdfasdf ",
				 "Id":8,
				 "Status":"Completed"}

			FAILURE
				- 400 Bad Request

**Complete a Thing**
		URL
			PUT: /api/thing/8/complete
		Params
			{"UserId":10}
		Results
			SUCCESS - 200 Ok
				{"Description":"a sdfasdf ",
				 "Id":8,
				 "Status":"Completed"}

			FAILURE
				- 400 Bad Request
				returns JSON error array
				Example: ["Invalid Status"]