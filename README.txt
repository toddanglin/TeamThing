Service API

Url								Params												Result

User Methods
POST: /api/user/signin			{"EmailAddress":"jholt456@gmail.com"}				SUCCESS
																						200: {"EmailAddress":"jholt456@gmail.com",
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

								{"EmailAddress":"newUser@test.com"}					FAILURE
																						400: Bad Request
																						Json Error Array:
																						["A user does not exist with this user name."]

POST: /api/user/register		{"EmailAddress":"newUser@test.com"}					SUCCESS
																						{"EmailAddress":"newUser@test.com",
																						 "Id":7,
																						 "PendingTeams":[],
																						 "Teams":[],
																						 "Things":[]
																						 }

								{"EmailAddress":"jholt456@gmail.com"}				FAILURE
																						400: Bad Request
																						Json Error Array:
																						["A user with this email address has already registered!"]

Team Methods

GET: /api/team?$filter=Name ne null and tolower(Name) eq 'asdf'										SUCCESS - 200 Ok
																										{"Administrators":[6],
																										 "Id":9,
																										 "IsPublic":true,
																										 "Name":"asdf",
																										 "OwnerId":6}
																									
GET: /api/team/6																					SUCCESS - 200 Ok
																										{"Id":6,
																										  "IsPublic":true,
																										  "Name":"567 asdfasdf",
																										  "PendingTeamMembers":[],
																										  "TeamMembers":[{"EmailAddress":"jholt456@gmail.com",
																														  "FullName":" ",
																														  "Id":6,
																														  "Role":"Administrator"}]}

																									FAILURE

POST: /api/team		{"name":"asdf","ispublic":true,"createdById":6}									SUCCESS - 201 Created
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
																									FAILURE - 400 Bad Request - When any data is invalid
																										Json Error Array:
																										ex: ["Invalid Creator"]

PUT: /api/team/8	{"id":8,"name":"Test Team2","ispublic":true,"updatedbyid":6}					SUCCESS - 200 Ok
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
																									FAILURE - 400 Bad Request
																										Json Error Array
																										ex: ["A user with this email address has already registered!"]	

DELETE: /api/team/8					{"userId":6}													SUCCESS - 204 No Content
																									FAILURE 
																										400 Bad Request 
																											Json Error Array 
																											If information is missing, or if user does not have permissions to remove team

PUT: /api/team/6/join					{"name":"Test Team","userId":7}									SUCCESS
																									{"Id":8,"IsPublic":false,
																									 "Name":"Test Team",
																									 "PendingTeamMembers":[{"EmailAddress":"newUser@test.com",
																															"FullName":" ",
																															"Id":7,
																															"Role":"Viewer"}],
																									 "TeamMembers":[{"EmailAddress":"jholt456@gmail.com",
																													 "FullName":" ",
																													 "Id":6,
																													 "Role":"Administrator"}]}

																									FAILURE
																									400 Bad Request - When any data is invalid
																						
PUT: /api/team/6/approvemember		{"teamId":8,"userId":7}											SUCCESS - 200 Ok

																									FAILURE
																									404 Not Found - When invalid team or user
																						
PUT: /api/team/6/denymember			{"teamId":8,"userId":7}											SUCCESS - 200 Ok

																									FAILURE
																									404 Not Found - When invalid team or user
																									400 Bad Request - When trying to deny access to team owner
