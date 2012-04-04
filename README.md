#Service API

#User Methods
	Search Users
		Notes
			Most standard odata search conventions can be used to search for users.
		URL
			GET: /api/team?$filter=EmailAddress ne null and tolower(EmailAddress) eq 'jholt456@gmail.com'
		Result	
			SUCCESS - 200 Ok
				[{"EmailAddress":"jholt456@gmail.com",
				"Id":6}]
			FAILURE - 200 Ok
				Returns empty array for no results

	-Sign in an Existing User
		Url
			POST: /api/user/signin			

		Params
			{"EmailAddress":"jholt456@gmail.com"}

		Result				
			SUCCESS
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
			FAILURE
				400: Bad Request
				returns JSON error array
				Example: ["A user does not exist with this user name."]

	-Register a New User
		Url
			POST: /api/user/register

		Params
			{"EmailAddress":"newUser@test.com"}

		Result
			SUCCESS
				{"EmailAddress":"newUser@test.com",
					"Id":7,
					"PendingTeams":[],
					"Teams":[],
					"Things":[]
					}

			FAILURE
					400: Bad Request
					returns JSON error array
					Example: ["A user with this email address has already registered!"]

<h2>Team Methods</h2>

	-Search Teams
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

	-Get a Team	
		URL
			GET: /api/team/6

		Result 
			SUCCESS - 200 Ok
				{"Id":6,
					"IsPublic":true,
					"Name":"567 asdfasdf",
					"PendingTeamMembers":[],
					"TeamMembers":[{"EmailAddress":"jholt456@gmail.com",
									"FullName":" ",
									"Id":6,
									"Role":"Administrator"}]}

			FAILURE
				- 400 Bad Requestd
				returns JSON error array
				Example: ["Invalid Team"]

	-Create a Team
		URL
			POST: /api/team		
		Params
			{"name":"asdf","ispublic":true,"createdById":6}
		Result
			SUCCESS - 201 Created
				{"Administrators":[6],
				 "Id":19,
				 "IsPublic":false,
				 "Name":"My new team",
				 "OwnerId":6}
			FAILURE - 400 Bad Request - When any data is invalid
				returns JSON error array
				Example: ["A team must have a name"]
	
	-Update a Team
		URL
			PUT: /api/team/8
		Params	
			{"id":8,"name":"Test Team2","ispublic":true,"updatedbyid":6}
		Result
			SUCCESS - 200 Ok
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
				returns JSON error array
				ex: ["Team name already in use"]

	-Delete a Team
		URL
			DELETE: /api/team/8
		Params
			{"userId":6}
		Results
			SUCCESS - 204 No Content
			FAILURE 
				400 Bad Request 
					returns error message string if information is missing, or if user does not have permissions to remove team

	-Add a User to Team
		URL
			PUT: /api/team/6/join
		Params 
			{"name":"Test Team","userId":7}
		Result
			SUCCESS
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
				returns JSON error array
				Example: ["A team must have a name"]
	-Approve Pending Team Member
		URL
			PUT: /api/team/6/approvemember		
		Params
			{"teamId":8,"userId":7}
		Result
			SUCCESS - 200 Ok
			FAILURE
				404 Not Found - When invalid team or user

	-Deny User
		URL
			PUT: /api/team/6/denymember
		Params
			{"teamId":8,"userId":7}
		Result
			SUCCESS - 200 Ok
			FAILURE
				404 Not Found - When invalid team or user
				400 Bad Request - When trying to deny access to team owner
<h2>Thing Methods</h2>
	-Search Things
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

	-Get a Thing	
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

	-Create a new Thing
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

	-Update a Thing
		URL
			PUT: /api/thing/8
		Params	
			
		Result
			SUCCESS - 200 Ok
				
			FAILURE - 400 Bad Request - When any data is invalid
				returns JSON error array
				Example: ["A thing must be assigned to 1 or more people"]

	-Delete a Thing
		URL
			DELETE: /api/thing/8
		Params
			{"DeletedById":6}
		Results
			SUCCESS - 204 No Content
			FAILURE 
				400 Bad Request 
					returns error message string if information is missing, or if user does not have permissions to remove thing(only thing owners can delete things)

	-Complete a Thing
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

	-Complete a Thing
		URL
			PUT: /api/thing/8/complete
		Params
			{"UserId":10, "Status":"Complete"}
		Results
			SUCCESS - 200 Ok
				{"Description":"a sdfasdf ",
				 "Id":8,
				 "Status":"Complete"}

			FAILURE
				- 400 Bad Request
				returns JSON error array
				Example: ["Invalid Status"]