var TeamThingData = function(){
	var _uThingsCache = new localStore("uThingsCache"),
		_uTeamsCache = null,
		_lastPollTime = null
		_private = {},
		_isTest = false,
		_endpoint = "http://teamthing.apphb.com/api/",
		_onlinePaths = {
			searchTeam: {path: "team?$filter=substringof('{q}',Name)%20eq%20True", verb: "GET"},
			getTeam: {path: "team/{key}", verb: "GET"},
			createTeam: {path: "team", verb: "POST"},
			updateTeam: {path: "team/{key}", verb: "PUT"},
			deleteTeam: {path: "team/{key}", verb: "DELETE"},
			joinTeam: {path: "team/{key}/join", verb: "PUT"},
			userLogin: {path: "user/signin", verb: "POST"},
			registerUser: {path: "user/register", verb: "POST"},
			getUserThings: {path: "user/{key}/things?teamId={teamId}", verb: "GET"},
			getThing: {path: "thing/{key}", verb: "GET"},
			createThing: {path: "thing", verb: "POST"},
			updateThingStatus: {path: "thing/{key}/updatestatus", verb: "PUT"}
		},
		_testPaths = {
			searchTeam: {path: "teamSearch.json", verb: "GET"},
			getTeam: {path: "team/{key}", verb: "GET"},
			createTeam: {path: "team", verb: "POST"},
			updateTeam: {path: "team/{key}", verb: "PUT"},
			deleteTeam: {path: "team/{key}", verb: "DELETE"},
			joinTeam: {path: "team/{key}/join", verb: "PUT"},
			userLogin: {path: "user/signin", verb: "POST"},
			registerUser: {path: "user/register", verb: "POST"},
			getThing: {path: "thing/{key}", verb: "GET"},
			createThing: {path: "thing", verb: "POST"}
		},
		_paths = _onlinePaths;
		
	_private = {
		load: function(path, verb, options){
			console.log("GETTING");
			
			var _url = (_isTest) ? path : _endpoint + path;
			
			//TODO: Use cached data before re-querying (if fresh)
			return $.ajax({
				type: verb,
				url: _url,
				data: options,
				dataType: "json"
			}).error(function(e,r,m){
				console.log("ERROR", e, r, m);
			});	
		}
	};
		
	return {
		getUserTeamThings: function(uid, tid){
			var dfd = new $.Deferred(),
			    route = _paths.getUserThings;
			
			route.path = route.path.replace(/{key}/g, uid);
			route.path = route.path.replace(/{teamId}/g, tid);
			
			_private.load(route.path, route.verb, {})
				.success(function(data){
					var tmpDate = new Date(),
						_expires = tmpDate.setMinutes(tmpDate.getMinutes() + 10);
						
					_uThingsCache.set({data:data,expires:_expires});
					
					dfd.resolve(data);
				})
				.error(function(err){
					console.log("GET THINGS ERROR", err);
					dfd.resolve(null);
				});
			
			return dfd.promise();
		},		
		getAllTeamThings: function(tid){
		
		},
		getUserTeams: function(uid){
		
		},
		getTeam: function(tid){
			var dfd = new $.Deferred(),
				route = _paths.getTeam;
			
			//TODO: Check for cache before loading
			
			route.path = route.path.replace(/{key}/g, tid);
			
			_private.load(route.path, route.verb, {})
				.success(function(data){
					//TODO: Cache
					console.log("TEAM", data);
					dfd.resolve(data);
				});
			
			return dfd.promise();
		},
		joinTeam: function(teamId, userId){
			var dfd = new $.Deferred(),
				route = _paths.joinTeam;
			
			route.path = route.path.replace(/{key}/g, teamId);
			
			_private.load(route.path, route.verb, {UserId:userId})
				.success(function(data){
					console.log("JOIN TEAM", data);
					dfd.resolve(data);
				})
				.error(function(err){
					console.log("JOIN TEAM ERROR", err);
					dfd.resolve(null);
				});
			
			return dfd.promise();
		},
		getThing: function(thingId){
			var dfd = new $.Deferred(),
				things = _uThingsCache.get();
			
			if(things == null || (new Date(things.expires) <= new Date())) return; //TODO - Populate cache if empty or expired
			
			var q = new kendo.data.Query(things.data),
			    result = q.filter({field:"id",operator:"eq",value:thingId}).toArray()[0];
			    
			dfd.resolve(result);
			
			return dfd.promise();
		},
		createThing: function(teamId, userId, txt){
			var dfd = new $.Deferred(),
				route = _paths.createThing;

			_private.load(route.path, route.verb, {CreatedById:userId, Description:txt, AssignedTo:[userId], teamId:teamId})
				.success(function(data){
					dfd.resolve(data);
				})
				.error(function(err){
					dfd.resolve(null);
				});
			
			return dfd.promise();
		},
		updateThingStatus: function(thingId, uid, newStatus){
			var dfd = new $.Deferred(),
				route = _paths.updateThingStatus;
			
			route.path = route.path.replace(/{key}/g, thingId);
			
			_private.load(route.path, route.verb, {UserId:uid, Status: newStatus})
				.success(function(data){
					dfd.resolve(data);
				})
				.error(function(err){
					dfd.resolve(null);
				});
			
			return dfd.promise();
		},
		searchTeams: function(userId, query){
			var dfd = new $.Deferred(),
				route = _paths.searchTeam;
			
			route.path = route.path.replace(/{q}/g, query);
			
			_private.load(route.path, route.verb, {})
				.success(function(data){
					console.log("TEAM SEARCH", data);
					dfd.resolve(data);
				});
			
			return dfd.promise();
		},
		validateUser: function(email, pass){
			var dfd = new $.Deferred(),
				route = _paths.userLogin;
			
			//DEMO HACKERY
			//var json = (email == "anglin@telerik.com") ? "userLogin.json" : "newUserLogin.json";	
			_private.load(route.path, route.verb, {EmailAddress:email})
				.success(function(data){
					dfd.resolve(data);
				})
				.error(function(err){
					dfd.resolve(null);
				});
		
			return dfd;
		}
	}
};