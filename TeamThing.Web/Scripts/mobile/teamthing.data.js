var TeamThingData = function () {
    var _uThingsCache = new localStore("uThingsCache"),
        _uTeamThingsCache = new localStore("uTeamThingsCache"),
		_uTeamsCache = null,
		_lastPollTime = null
    _private = {},
		_isTest = false,
		_endpoint = "http://teamthing.net/api/",
		_onlinePaths = {
		    searchTeam: { path: "team?$filter=substringof('{q}',Name)%20eq%20True", verb: "GET", token: false },
		    getTeam: { path: "team/{key}", verb: "GET", token: false },
		    createTeam: { path: "team", verb: "POST", token: false },
		    updateTeam: { path: "team/{key}", verb: "PUT", token: false },
		    deleteTeam: { path: "team/{key}", verb: "DELETE", token: false },
		    joinTeam: { path: "team/{key}/join", verb: "PUT", token: false },
		    userLogin: { path: "user/signin", verb: "POST", token: false },
		    userOauth: { path: "user/oauth", verb: "POST", token: true },
		    registerUser: { path: "user/register", verb: "POST", token: false },
		    getUserThings: { path: "user/{key}/things?$filter=TeamId eq {teamId}", verb: "GET", token: false },
		    getThing: { path: "thing/{key}", verb: "GET", token: false },
            getTeamThings: { path: "team/{key}/things", verb: "GET", token: false },
            getStarredThings: { path: "user/{key}/starred", verb:"GET", token:false},
            getTeamMembers: { path: "team/{key}/members", verb: "GET", token: false },
		    createThing: { path: "thing", verb: "POST", token: false },
		    updateThingStatus: { path: "thing/{key}/updatestatus", verb: "PUT", token: false },
            updateThingStar: {path: "thing/{key}/star", verb: "PUT", token: false},
            updateThingUnStar: {path: "thing/{key}/unstar", verb: "PUT", token: false},
		    deleteThing: { path: "thing/{key}", verb: "DELETE", token: false }
		},
		_paths = _onlinePaths,
		_authInfo = new localStore("authInfo");

    _private = {
        load: function (route, options) {            
            var path = route.path,
            	verb = route.verb,
            	requiresToken = route.token,
            	url = (_isTest) ? path : _endpoint + path,
            	info = _authInfo.get(),
                dfd = new $.Deferred();


            if(info.authToken != null){
                $.ajaxSetup({
                    'beforeSend': function(xhr){
                        xhr.setRequestHeader("X-AuthProvider", info.provider);
                        xhr.setRequestHeader("X-AuthToken", info.authToken);
                    }
                })
            }

            console.log("GETTING", url, verb, options);

            //TODO: Use cached data before re-querying (if fresh)
            if (!dfd.isRejected()) {
                $.ajax({
                    type: verb,
                    url: url,
                    data: options,
                    dataType: "json"
                }).success(function (data, code, xhr) {
                    dfd.resolve(data, code, xhr);
                }).error(function (e, r, m) {
                    console.log("ERROR", e, r, m);
                    dfd.reject(m);
                });
            }

            return dfd.promise();
        }
    };

    return {
        getUserTeamThings: function (uid, tid) {
            var dfd = new $.Deferred(),
			    route = $.extend({}, _paths.getUserThings);

            route.path = route.path.replace(/{key}/g, uid);
            route.path = route.path.replace(/{teamId}/g, tid);

            _private.load(route, {})
				.done(function (data) {
				    var tmpDate = new Date(),
						_expires = tmpDate.setMinutes(tmpDate.getMinutes() + 10);

				    _uThingsCache.set({ data: data, expires: _expires });

				    dfd.resolve(data);
				})
				.fail(function (err) {
				    console.log("GET THINGS ERROR", err);
				    dfd.resolve(null);
				});

            return dfd.promise();
        },
        getAllTeamThings: function (tid) {
            var dfd = new $.Deferred(),
                route = $.extend({}, _paths.getTeamThings);

            route.path = route.path.replace(/{key}/g, tid);

            _private.load(route, {})
                .done(function (data) {
                    var tmpDate = new Date(),
                        _expires = tmpDate.setMinutes(tmpDate.getMinutes() + 10);

                    _uTeamThingsCache.set({ data: data, expires: _expires });

                    dfd.resolve(data);
                })
                .fail(function (err) {
                    console.log("GET TEAM THINGS ERROR", err);
                    dfd.resolve(null);
                });

            return dfd.promise();
        },
        getStarredTeamThings: function (tid, userId) {
            var dfd = new $.Deferred(),
                route = $.extend({}, _paths.getStarredThings);

            route.path = route.path.replace(/{key}/g, userId);

            _private.load(route, {teamId: tid})
                .done(function (data) {
                    dfd.resolve(data);
                })
                .fail(function (err) {
                    console.log("GET STARRED THINGS ERROR", err);
                    dfd.resolve(null);
                });

            return dfd.promise();
        },
        getUserTeams: function (uid) {

        },
        getTeam: function (tid) {
            var dfd = new $.Deferred(),
				route = $.extend({}, _paths.getTeam);

            //TODO: Check for cache before loading

            route.path = route.path.replace(/{key}/g, tid);

            _private.load(route, {})
				.done(function (data) {
				    //TODO: Cache
				    console.log("TEAM", data);
				    dfd.resolve(data);
				});

            return dfd.promise();
        },
        getTeamMembers: function(tid){
            var dfd = new $.Deferred(),
                route = $.extend({}, _paths.getTeamMembers);

            //TODO: Check for cache before loading

            route.path = route.path.replace(/{key}/g, tid);

            _private.load(route, {})
                .done(function (data) {
                    //TODO: Cache
                    console.log("TEAM MEMBERS", data);
                    dfd.resolve(data);
                }).fail(function (err) {
                    console.log("GET TEAM MEMBERS ERROR", err);
                    dfd.resolve(null);
                });

            return dfd.promise();
        },
        joinTeam: function (teamId, userId) {
            var dfd = new $.Deferred(),
				route = $.extend({}, _paths.joinTeam);

            route.path = route.path.replace(/{key}/g, teamId);

            _private.load(route, { UserId: userId })
				.done(function (data) {
				    console.log("JOIN TEAM", data);
				    dfd.resolve(data);
				})
				.fail(function (err) {
				    console.log("JOIN TEAM ERROR", err);
				    dfd.resolve(null);
				});

            return dfd.promise();
        },
        createTeam: function(teamName, isPrivate, userId){
            var dfd = new $.Deferred(),
                route = $.extend({}, _paths.createTeam);


            _private.load(route, { createdById: userId, name: teamName, isPublic: !isPrivate })
                .done(function (data) {
                    console.log("CREATE TEAM", data);
                    dfd.resolve(data);
                })
                .fail(function (err) {
                    console.log("CREATE TEAM ERROR", err);
                    dfd.reject(err);
                });

            return dfd.promise();
        },
        getThing: function (thingId) {
            var dfd = new $.Deferred(),
				things = _uThingsCache.get();

            if (things == null || (new Date(things.expires) <= new Date())) return; //TODO - Populate cache if empty or expired

            var q = new kendo.data.Query(things.data),
			    result = q.filter({ field: "id", operator: "eq", value: thingId }).toArray()[0];

            dfd.resolve(result);

            return dfd.promise();
        },
        createThing: function (teamId, userId, txt) {
            var dfd = new $.Deferred(),
				route = $.extend({}, _paths.createThing);

            _private.load(route, { CreatedById: userId, Description: txt, AssignedTo: [userId], teamId: teamId })
				.done(function (data) {
				    dfd.resolve(data);
				})
				.fail(function (err) {
				    dfd.resolve(null);
				});

            return dfd.promise();
        },
        deleteThing: function (thingId, uid) {
            var dfd = new $.Deferred(),
				route = $.extend({}, _paths.deleteThing);

            route.path = route.path.replace(/{key}/g, thingId);

            _private.load(route, { DeletedById: uid })
				.done(function (data) {
				    dfd.resolve(data);
				})
				.fail(function (err) {
				    dfd.resolve(null);
				});

            return dfd.promise();
        },
        updateThingStatus: function (thingId, uid, newStatus) {
            var dfd = new $.Deferred(),
				route = $.extend({}, _paths.updateThingStatus);

            //Handle Delete uniquely
            if (newStatus == "Delete") return this.deleteThing(thingId, uid);

            route.path = route.path.replace(/{key}/g, thingId);

            _private.load(route, { UserId: uid, Status: newStatus })
				.done(function (data) {
				    dfd.resolve(data);
				})
				.fail(function (err) {
				    dfd.resolve(null);
				});

            return dfd.promise();
        },
        updateThingStar: function(thingId, userId){
            var dfd = new $.Deferred(),
                route = $.extend({}, _paths.updateThingStar);

            route.path = route.path.replace(/{key}/g, thingId);

            _private.load(route, {userId: userId})
                .done(function (data) {
                    dfd.resolve(data);
                })
                .fail(function (err) {
                    dfd.resolve(null);
                });

            return dfd.promise();
        },
        updateThingUnStar: function(thingId, userId){
            var dfd = new $.Deferred(),
                route = $.extend({}, _paths.updateThingUnStar);

            route.path = route.path.replace(/{key}/g, thingId);

            _private.load(route, {userId: userId})
                .done(function (data) {
                    dfd.resolve(data);
                })
                .fail(function (err) {
                    dfd.resolve(null);
                });

            return dfd.promise();
        },
        searchTeams: function (userId, query) {
            var dfd = new $.Deferred(),
				route = $.extend({}, _paths.searchTeam);

            route.path = route.path.replace(/{q}/g, query);

            _private.load(route, {})
				.done(function (data) {
				    console.log("TEAM SEARCH", data);
				    dfd.resolve(data);
				});

            return dfd.promise();
        },
        validateUser: function (email, pass) {
            var dfd = new $.Deferred(),
				route = $.extend({}, _paths.userLogin);

            //DEMO HACKERY
            //var json = (email == "anglin@telerik.com") ? "userLogin.json" : "newUserLogin.json";	
            _private.load(route, { EmailAddress: email })
				.done(function (data) {
				    dfd.resolve(data);
				})
				.fail(function (err) {
				    dfd.resolve(null);
				});

            return dfd;
        },
        validateOauthUser: function (userInfo) {
            var dfd = new $.Deferred(),
				route = $.extend({}, _paths.userOauth);

            //Send userInfo object to server (token, provider)
            //to get back the TeamThing user object
            _private.load(route, userInfo)
				.done(function (data) {
				    dfd.resolve(data);
				})
				.fail(function (err) {
				    dfd.resolve(null);
				});

            return dfd;
        }
    }
};