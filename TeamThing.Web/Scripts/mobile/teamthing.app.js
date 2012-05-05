var TeamThing = function (kendoApp) {
    var _data = new TeamThingData(),
		_app = kendoApp,
		_currentUser = new localStore("cUser"),
		_currentTeamId = new localStore("teamId"),
		_remember = new localStore("uRemember"),
        _refreshThings = new localStore("refreshThings"),
    _showInstallPrompt = new localStore("installPrompt"),
    //ELEMENT CACHE
		_eleThingList = $("#lstThings"),
		_eleNoThingsMsg = $("#msgNoThings"),
    //_eleThingDetail = $("#thingDetail"),
    //TEMPLATE CACHE
		_tmplThingItem = kendo.template($("#tmplThingItem").html()),
    //_tmplThingDetail = kendo.template($("#tmplThingDetail").html()),
    //DATASOURCE CACHE
		_dsThings = null;

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
                            options.success(data);
                        });
                    }
                },
                group: "status"
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
            }
        },
        loadDetail: function (thingId, tmpl, ele) {
            $.when(_data.getThing(thingId)).then(function (result) {
                ele.html(tmpl(result));
                console.log("DETAIL", result, ele);

                $(ele).find("a").kendoMobileButton();
            });
        },
        loadTeamView: function (tmpl, ele) {
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
        createThing: function (txt) {
            var tid = this.getCurrentTeam(),
				uid = this.getCurrentUser().id;

            return $.when(_data.createThing(tid, uid, txt)).then(function (result) {
                console.log("SAVE THING", result);
                if (result) {
                    return true;
                } else {
                    return false;
                }
            });
        },
        updateThingStatus: function (thingId, newStatus) {
            var uid = this.getCurrentUser().id;
           
            return $.when(_data.updateThingStatus(thingId, uid, newStatus)).then(function (result) {
                if (result) {
                    return true;
                } else {
                    return false;
                }
            });
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
        changeTeam: function (newTeamId) {
            //TODO: Check server for ability to change team before changing
            var dfd = new $.Deferred();

            _currentTeamId.set(newTeamId);
            _eleNoThingsMsg.show();
            dfd.resolve(true);

            return dfd.promise();
        },
        clearUser: function () {
            _currentUser.set(null);
            _currentTeamId.set(null);
            _remember.set(null);
        },
        clearTeam: function () {
            _currentTeamId.set(null);
        },
        showLoading: function (txt) {
            if (txt != "") {
                this.app.loader.find("h1").text(txt);
            }

            this.app.showLoading();
        },
        hideLoading: function () {
            this.app.hideLoading();
            this.app.loader.find("h1").text("Loading...");
        },
        getCurrentUser: function () { return _currentUser.get(); },
        setCurrentUser: function (val) { _currentUser.set(val); },
        getCurrentTeam: function () { return _currentTeamId.get(); },
        getRemember: function () { return _remember.get(); },
        getShowInstallPrompt: function () { return _showInstallPrompt.get(); },
        setThingsRefreshFlag: function () { _refreshThings.set(true); return; },
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