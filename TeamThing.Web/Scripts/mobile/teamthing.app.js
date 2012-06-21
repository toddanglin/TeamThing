var TeamThing = function (kendoApp) {
    var _data = new TeamThingData(),
		_app = kendoApp,
		_currentUser = new localStore("cUser"),
		_currentTeamId = new localStore("teamId"),
        _currentTeamMembers = new localStore("teamMembers"),
		_remember = new localStore("uRemember"),
        _refreshThings = new localStore("refreshThings"),
        _refreshStarList = false;
    _showInstallPrompt = new localStore("installPrompt"),
        _authProvider = new OAuthProvider(),
        _authStatus = null,
        _authInfo = new localStore("authInfo"),
    //ELEMENT CACHE
		_eleThingList = $("#lstThings"),
		_eleNoThingsMsg = $("#msgNoThings"),
    //_eleThingDetail = $("#thingDetail"),
    //TEMPLATE CACHE
		_tmplThingItem = kendo.template($("#tmplThingItem").html()),
    //_tmplThingDetail = kendo.template($("#tmplThingDetail").html()),
    //DATASOURCE CACHE
		_dsThings = null,
        _dsTeamThings = null,
        _dsStarThings = null;

    return {
        init: function () {
            var that = this;
            console.log("INIT", that);

            _refreshThings.set(false);

            if (_showInstallPrompt.get() == null) {
                _showInstallPrompt.set(true);
            }

            _dsThings = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        that.showLoading();
                        $.when(_data.getUserTeamThings(_currentUser.get().id, _currentTeamId.get())).then(function (data) {
                            console.log("THINGS DATA", data);
                            that.hideLoading();
                            if (data.length < 1) _eleNoThingsMsg.show();
                            if (options != null) options.success(data);
                        });
                    }
                },
                group: "status"
            });

            _dsTeamThings = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        that.showLoading();
                        $.when(_data.getAllTeamThings(_currentTeamId.get())).then(function (data) {
                            console.log("TEAM THINGS DATA", data);
                            that.hideLoading();

                            if (options != null) options.success(data);
                        });
                    }
                },
                group: "owner.nickname"
            });

            _dsStarThings = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        that.showLoading();
                        $.when(_data.getStarredTeamThings(_currentTeamId.get(), _currentUser.get().id)).then(function (data) {
                            console.log("STARRED THINGS DATA", data);
                            that.hideLoading();

                            if (options != null) options.success(data);
                        });
                    }
                },
                group: "owner.nickname"
            });
        },
        loadThingsView: function () {
            _eleNoThingsMsg.hide();

            var lv = _eleThingList.data("kendoMobileListView");
            if (lv == undefined) {
                lv = _eleThingList.kendoMobileListView({
                    dataSource: _dsThings,
                    template: _tmplThingItem
                }).data("kendoMobileListView");
            }

            console.log("REFRESH THINGS LIST", _refreshThings.get());
            if (_refreshThings.get()) {
                _dsThings.read();
                _refreshThings.set(false);
            }
        },
        loadDetail: function (thingId, tmpl, ele) {
            $.when(_data.getThing(thingId)).then(function (result) {
                ele.html(tmpl(result));
                console.log("DETAIL", result, ele);

                $(ele).find("a").kendoMobileButton();
            });
        },
        loadTeamView: function(tmpl, ele){
            var lv = ele.data("kendoMobileListView");
            if (lv == undefined) {
                lv = ele.kendoMobileListView({
                    dataSource: _dsTeamThings,
                    template: tmpl
                }).data("kendoMobileListView");
            }

            _dsTeamThings.bind("change", function(){
                //Try to highlight starred things
                $.when(_data.getStarredTeamThings(_currentTeamId.get(), _currentUser.get().id)).then(function(data){
                    if(data == null) return; //No starred things

                    //For each starred thing, find element in ListView, apply CSS
                    for (var i = 0; i < data.length; i++) {
                        var thingId = data[i].id,
                            selector = '*[data-thing-id="'+ thingId +'"]',
                            row = $(ele).find(selector);

                        if(row == []) continue; //Can't find this row

                        //console.log("ROW FOUND", row, row.parent(), selector);

                        row.parent().addClass("starred");
                    };
                });
            });

            //_dsTeamThings.read();
        },
        loadStarView: function(tmpl, ele){
            var lv = ele.data("kendoMobileListView");
            if (lv == undefined) {
                lv = ele.kendoMobileListView({
                    dataSource: _dsStarThings,
                    template: tmpl
                }).data("kendoMobileListView");
            }

            if(_refreshStarList){
                _dsStarThings.read();
                _refreshStarList = false;
            }
        },
        loadTeamMemberView: function (tmpl, ele) {
            $.when(_data.getTeam(this.getCurrentTeam())).then(function (result) {
                ele.html(tmpl(result));

                $(ele).find("#teamScroll").kendoMobileScrollView();
            });
        },
        validateUser: function (email, pass, remember) {
            var dfd = new $.Deferred,
				that = this;
            //TODO: Check TeamThing API to see if email is associated
            //		with TeamThing Team. If yes, log and load team

            $.when(_data.validateUser(email, pass)).then(function (user) {
                if (user == null) {
                    dfd.resolve(null);
                } else {
                    console.log("R", remember);
                    if (remember) {
                        _remember.set(remember);
                    }

                    that.setCurrentUser(user);
                    dfd.resolve(user);
                }
            });

            return dfd;
        },
        validateOauthUser: function (token, provider) {
            var dfd = new $.Deferred,
                userInfo = { provider: provider, authToken: token },
                that = this,
                authInfo = that.getAuthInfo(),
                remember = true; //Always remember OAuth users for now

            //Save OAuth token
            if (authInfo == null || authInfo.authToken != token) {
                console.log("SET AUTH INFO", userInfo);
                that.setAuthInfo(userInfo);
            }

            //This step checks the auth token server-side to ensure it 
            //is still valid with the provider
            //TODO: Maybe skip this check if we're within the expiration window of the token?
            $.when(_data.validateOauthUser(userInfo)).then(function (user) {
                if (user == null) {
                    dfd.resolve(null);
                    console.log("OAuth User Validate Error")
                } else {
                    console.log("R", remember);
                    if (remember) {
                        _remember.set(remember);
                    }

                    that.setCurrentUser(user);
                    dfd.resolve(user);
                }
            });

            return dfd;
        },
        createThing: function (txt) {
            var tid = this.getCurrentTeam(),
				uid = this.getCurrentUser().id,
                that = this;

            return $.when(_data.createThing(tid, uid, txt)).then(function (result) {
                console.log("SAVE THING", result);
                if (result) {
                    that.setThingsRefreshFlag();
                    return true;
                } else {
                    return false;
                }
            });
        },
        updateThingStatus: function (thingId, newStatus) {
            var uid = this.getCurrentUser().id,
                that = this;

            return $.when(_data.updateThingStatus(thingId, uid, newStatus)).then(function (result) {
                if (result) {
                    that.setThingsRefreshFlag();
                    return true;
                } else {
                    return false;
                }
            });
        },
        updateThingStar: function (thingId, shouldStar){
            var userId = _currentUser.get().id;

            if(shouldStar){
                $.when(_data.updateThingStar(thingId, userId)).then(function(){
                    _refreshStarList = true;
                });
            }else{
                $.when(_data.updateThingUnStar(thingId, userId)).then(function(){
                    _refreshStarList = true;
                });
            }
        },
        joinTeam: function (teamId) {
            var dfd = new $.Deferred(),
				userId = this.getCurrentUser().id;


            $.when(_data.joinTeam(teamId, userId)).then(function (result) {
                if (result) {
                    _currentTeamId.set(result.id);
                    dfd.resolve(true);
                } else {
                    dfd.resolve(null);
                }
            });

            return dfd.promise();
        },
        createAndJoinTeam: function(teamName, isPrivate){
            var dfd = new $.Deferred(),
                userId = this.getCurrentUser().id;

            //Make sure any existing team cache is cleared        
            this.clearTeam();

            $.when(_data.createTeam(teamName, isPrivate, userId)).done(function (result) {
                if (result) {
                    _currentTeamId.set(result.id);
                    dfd.resolve(true);
                } else {
                    //Missing team object; something went wrong
                    dfd.reject("Failed to create team");
                }
            }).fail(function(msg){
                //Error creating the team
                dfd.reject(msg);
            });

            return dfd.promise();
        },
        changeTeam: function (newTeamId) {
            //TODO: Check server for ability to change team before changing
            var dfd = new $.Deferred();

            this.clearTeam();
            _currentTeamId.set(newTeamId);
            _eleNoThingsMsg.show();
            dfd.resolve(true);

            return dfd.promise();
        },
        clearUser: function () {
            //Clear all cached user values
            _authStatus = null;
            _authInfo.set(null);
            _currentUser.set(null);
            this.clearTeam();
            _remember.set(null);
        },
        clearTeam: function () {
            _currentTeamId.set(null);
            _currentTeamMembers.set(null);
        },
        showLoading: function (txt) {
            if (txt != "") {
                this.app.loader.find("h1").text(txt);
            }

            try {
                this.app.showLoading();
            } catch (e) {
                //App currently showing loader - don't throw error
            }
        },
        hideLoading: function () {
            try {
                this.app.hideLoading();
                this.app.loader.find("h1").text("Loading...");
            } catch (e) {
                //App not currently showing loader - don't throw error
            }
        },
        getCurrentUser: function () { return _currentUser.get(); },
        setCurrentUser: function (val) { _currentUser.set(val); },
        getCurrentTeam: function () { return _currentTeamId.get(); },
        getRemember: function () { return _remember.get(); },
        getShowInstallPrompt: function () { return _showInstallPrompt.get(); },
        setThingsRefreshFlag: function () { _refreshThings.set(true); return; },
        setStarRefreshFlag: function(){ _refreshStarList = true; return; },
        getAuthProvider: function () { return _authProvider; },
        getAuthStatus: function () { return _authStatus; },
        setAuthStatus: function (val) { _authStatus = val; return; },
        getAuthInfo: function () { return _authInfo.get(); },
        setAuthInfo: function (val) { _authInfo.set(val); return; },
        getTeamMembers: function(){
            var dfd = new $.Deferred();

            if(_currentTeamMembers.get() == null){
                $.when(_data.getTeamMembers(_currentTeamId.get())).then(function(data){
                    _currentTeamMembers.set(data);

                    dfd.resolve(data);
                });
            }else{
                dfd.resolve(_currentTeamMembers.get());
            }

            return dfd.promise();
        },
        app: _app,
        data: _data
    }

};

//UTIL
window.getQueryString = function(q) {
 return (function(a) {
  if (a == "") return {};
  var b = {};
  for (var i = 0; i < a.length; ++i) {
   var p = a[i].split('=');
   if (p.length != 2) continue;
   b[p[0]] = decodeURIComponent(p[1].replace(/\+/g, " "));
  }
  return b;
 })(q.split("&"));
};