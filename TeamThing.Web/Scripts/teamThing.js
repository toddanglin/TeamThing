(function (application, $, kendo, undefined) {
    "use strict";

    function TeamDashboard(_teamThing, team) {
        var that = this;
        var teamThing = _teamThing;

        this.team = team;

        this.pendingTeamMembers = $.map(team.PendingTeamMembers, function (member) {
            return new PendingMemberListItemViewModel(member, that);
        });

        this.teamMembers = $.map(team.TeamMembers, function (member) {
            return new TeamMemberListItemViewModel(member, that);
        });

        this.things = team.Things

        function thingAdded(thing) {
            refresh();
        };

        function refresh() {
            //TODO: Memory leak?!?
            teamThing.showTeam(that.team);
        }

        this.editMember = function (user) {
            alert("edit" + user.Id);
        };

        this.viewMember = function (user) {
            alert("view" + user.Id);
        };

        this.denyMember = function (user) {

            var teamId = that.team.Id;
            var approvalInfo = { teamId: teamId, userId: user.Id };

            //ToDO: will this cause a memory leak?!
            teamThing.dataProvider.updateResource("/api/team/" + teamId + "/denymember", approvalInfo, this.refresh);
        };

        this.approveMember = function (user) {
            var teamId = that.team.Id;
            var approvalInfo = { teamId: teamId, userId: user.Id };

            //ToDO: will this cause a memory leak?!
            teamThing.dataProvider.updateResource("/api/team/" + teamId + "/approvemember", approvalInfo, this.refresh);
        };

        this.addThing = function () {

            var newThingModel = {
                description: "",
                availableTeamMembers: that.teamMembers,
                selectedTeamMembers: [],
                save: function (e) {

                    //retrieve the ids for the selected assignments
                    var assignedTo = $.map(this.selectedTeamMembers, function (member) {
                        return member.user.Id;
                    });

                    //get the current application user's id
                    var currentUserId = teamThing.getCurrentUser().id;

                    //create the thing object
                    var thing = { CreatedById: currentUserId, Description: this.description, AssignedTo: assignedTo };

                    //save the new thing back to the server
                    teamThing.dataProvider.createResource("/api/thing", thing, function (result) {
                        teamThing.closeDialog(); //ewww
                        thingAdded(result);
                    });
                },
                cancel: function (e) {
                    teamThing.closeDialog(); //ewww
                }
            };

            teamThing.showDialog("#addThing", "Add a Thing", "/addThing.html", newThingModel);
        };

        return this;
    };

    function UserDashboard(_teamThing, teamUser) {
        var that = this;
        var teamThing = _teamThing;
        this.UserName = teamUser.EmailAddress;
        this.id = teamUser.Id;
        this.user = teamUser;

        function toogleUI() {
            //if the user only has one team, show that teams info!
            if (that.teams.length === 1) {
                that.viewTeam(that.teams[0].Id);
            }
        };

        function teamUpdated(team) {
            //TODO: Update team info in UI

            var teams = that.user.Teams;
            var i = teams.length;
            while (i--) {
                var currentTeam = teams[i];
                if (currentTeam.Id == team.Id) {
                    teams[i] = team;
                }
            }
            refresh();
        }

        function teamCreated(team) {
            that.user.Teams.push(team);
            refresh();
        }

        function refresh() {
            //TODO: Memory leak?!?
            teamThing.loadUser(that.user);
        }

        function teamRemoved(removedTeam) {
        
            that.user.Teams = $.map(that.user.Teams, function (team) {
                if (team.Id !== removedTeam.Id) {
                    return team;
                }
            });
            refresh();
        }

        function teamJoined(team) {
            if (team.IsPublic) {
                that.user.Teams.push(team);
            }
            else {
                that.user.PendingTeams.push(team);
            }

            refresh();
        };

        this.joinTeam = function () {

            var joinTeamViewModel = kendo.observable({
                name: "",
                canSave: false,
                foundTeamId: 0,
                searchStatusClass: 'ok',
                searchStatusMessage: '',
                searchTeams: function (e) {
                    var searchedName = this.name;
                    var vm = this;
                    //TODO: instead of an exact match we could let users se how many partials matches there are?
                    teamThing.dataProvider.get("/api/team?$filter=Name ne null and tolower(Name) eq '" + searchedName.toLowerCase() + "'", function (result) {
                        
                        if (result.length > 0) {
                            vm.set('canSave', true);
                            vm.set('searchStatusClass', 'searchResult ok');
                            vm.set('searchStatusMessage', 'Team Found');
                            vm.set('foundTeamId', result[0].Id);
                        }
                        else {
                            vm.set('canSave', false);
                            vm.set('searchStatusClass', 'searchResult error');
                            vm.set('searchStatusMessage', "No Team Found");
                            vm.set('foundTeamId', 0);
                        }
                    });
                },
                save: function (e) {

                    //get the current application user's id
                    var currentUserId = teamThing.getCurrentUser().id;

                    //create the team object
                    var team = { name: this.name, userId: currentUserId };
                    var vm = this;
                    //save the new team back to the server
                    teamThing.dataProvider.updateResource("/api/team/" + this.foundTeamId + "/join", team, function (result) {
                        kendo.unbind($("#joinTeam"), vm);
                        teamThing.closeDialog(); //ewww
                        teamJoined(result);
                    });
                },
                cancel: function (e) {
                    teamThing.closeDialog(); //ewww
                    kendo.unbind($("#joinTeam"), this);
                }
            });

            teamThing.showDialog("#joinTeam", "Join a Team", "/joinTeam.html", joinTeamViewModel);
        };

        this.removeTeam = function (team) {
            if (confirm("SRSLY?")) {
                var currentUserId = teamThing.getCurrentUser().id;
                teamThing.dataProvider.removeResource('/api/team/' + team.Id, { userId: currentUserId }, function () {
                    teamRemoved(team);
                });
            }
        };

        this.editTeam = function (team) {

            var teamEditorViewModel = {
                name: team.Name,
                id: team.Id,
                isPublic: team.IsPublic,
                canSave: true, 
                searchStatusClass: 'ok',
                searchStatusMessage: '',
                searchTeams: function (e) {
                    var searchedName = this.name;
                    var vm = this;
                    //TODO: instead of an exact match we could let users se how many partials matches there are?
                    teamThing.dataProvider.get("/api/team?$filter=Name ne null and tolower(Name) eq '" + searchedName.toLowerCase() + "'", function (result) {

                        if (result.length > 0 && result[0].Id != vm.id) {
                            vm.set('canSave', false);
                            vm.set('searchStatusClass', 'searchResult error');
                            vm.set('searchStatusMessage', 'Team Name Not Available');
                        }
                        else if (result.length > 0 && result[0].Id == vm.id) {
                            vm.set('canSave', true);
                            vm.set('searchStatusClass', 'searchResult ok');
                            vm.set('searchStatusMessage', '');
                        }
                        else {
                            vm.set('canSave', true);
                            vm.set('searchStatusClass', 'searchResult ok');
                            vm.set('searchStatusMessage', "Team Name Available");
                        }
                    });
                },
                save: function (e) {

                    //get the current application user's id
                    var currentUserId = teamThing.getCurrentUser().id;

                    var editedTeam = { id: this.id, name: this.name, ispublic: this.isPublic, updatedbyid: currentUserId };
                    var vm = this;
                    //save the new thing back to the server
                    teamThing.dataProvider.updateResource("/api/team/" + editedTeam.id, editedTeam, function (result) {
                        teamThing.closeDialog(); //ewww
                        teamUpdated(result);
                        kendo.unbind($("#newTeam"), vm);
                    });
                },
                cancel: function (e) {
                    teamThing.closeDialog(); //ewww
                    kendo.unbind($("#newTeam"), this);
                }
            };

            teamThing.showDialog("#newTeam", "Edit Team", "/teamEditor.html", teamEditorViewModel);
        };

        this.createTeam = function () {

            var newTeamViewModel = {
                name: "",
                isPublic: false,
                canSave: false,
                searchStatusClass: 'noResults',
                searchStatusMessage: '',
                searchTeams: function (e) {

                    var searchedName = this.name;
                    var vm = this;
                    //TODO: instead of an exact match we could let users se how many partials matches there are?
                    teamThing.dataProvider.get("/api/team?$filter=Name ne null and tolower(Name) eq '" + searchedName.toLowerCase() + "'", function (result) {

                        if (result.length > 0) {
                            vm.set('canSave', false);
                            vm.set('searchStatusClass', 'searchResult error');
                            vm.set('searchStatusMessage', 'Team Name Not Available');
                        }
                        else {
                            vm.set('canSave', true);
                            vm.set('searchStatusClass', 'searchResult ok');
                            vm.set('searchStatusMessage', "Team Name Available");
                        }
                    });
                },
                save: function (e) {

                    //get the current application user's id
                    var currentUserId = teamThing.getCurrentUser().id;

                    //create the thing object
                    var team = { name: this.name, ispublic: this.isPublic, createdById: currentUserId };

                    //save the new thing back to the server
                    teamThing.dataProvider.createResource("/api/team", team, function (result) {
                        teamThing.closeDialog(); //ewww
                        teamCreated(result);
                    });

                    kendo.unbind($("#newTeam"), this);
                },
                cancel: function (e) {
                    kendo.unbind($("#newTeam"), this);
                    teamThing.closeDialog(); //ewww
                }
            };

            teamThing.showDialog("#newTeam", "Create a New Team", "/teamEditor.html", newTeamViewModel);
        };

        this.viewTeam = function (id) {
            //todo: this is kinda ugly
            teamThing.dataProvider.get("api/team/" + id, teamThing.showTeam);
        };

        this.teams = $.map(teamUser.Teams, function (team) {
            return new TeamListItemViewModel(team, that);
        });

        this.pendingTeams = $.map(teamUser.PendingTeams, function (team) {
            return new TeamListItemViewModel(team, that);
        });

        this.things = teamUser.Things;

        return this;
    };

    function TeamListItemViewModel(team, userController) {

        var user = userController.user;
        var controller = userController;

        this.team = team;

        this.editTeam = function (e) { controller.editTeam(this.team); };
        this.removeTeam = function (e) { controller.removeTeam(this.team); };

        this.userCanEditTeam = function () {
            if (user != null && (this.team.OwnerId === user.Id || $.inArray(user.Id, this.team.Administrators) != -1)) {
                return true;
            }
            return false;
        };

        this.userCanViewTeam = function () {
            //TODO: check user team status
            return true;
        };
    };

    function PendingMemberListItemViewModel(user, teamController) {
        var controller = teamController;
        this.user = user;

        this.approveUser = function (e) { controller.approveMember(this.user); };
        this.denyUser = function (e) { controller.denyMember(this.user); };
    };

    function TeamMemberListItemViewModel(user, teamController) {
        var controller = teamController;
        this.user = user;

        this.viewUser = function (e) { controller.viewMember(this.user); };
        this.editUser = function (e) { controller.editMember(this.user); };
        this.denyUser = function (e) { controller.denyMember(this.user); };
    };

    function SignInViewModel(_teamThing) {
        var teamThing = _teamThing;
        this.userName = "";
        var that = this;

        function validateInput() {
            var minUserNameLength = 7; //'@.xxx'

            //TODO: that should be changed to this after release
            if (that.userName.length <= minUserNameLength) {
                alert("User name must be greater than " + minUserNameLength + " characters");
                return false;
            }

            return true;
        }

        this.signInUser = function () {
            if (validateInput()) {
                //TODO: that should be changed to this after release
                var userInfo = { EmailAddress: that.userName };
                //todo: this is kinda ugly
                teamThing.dataProvider.post("/api/user/signin", userInfo, teamThing.loadUser);
            }
        };

        this.registerUser = function () {
            if (validateInput()) {
                //TODO: that should be changed to this after release
                var userInfo = { EmailAddress: that.userName };
                //todo: this is kinda ugly
                teamThing.dataProvider.createResource("/api/user/register", userInfo, teamThing.loadUser);
            }
        };

        return this;
    }

    function Application(context) {
        var dialog;
        var activeUserViewModel;
        var views = [];

        this.context = context;
        this.dataProvider;
        var that = this;

        function createDialog() {
            if (dialog == null) {
                var windowEl = $("#dialog");

                if (!windowEl.data("kendoWindow")) {
                    dialog = windowEl.kendoWindow({
                        actions: ["Close"],
                        draggable: true,
                        modal: true,
                        resizable: false
                    });

                    windowEl.show();
                }
            }

            return dialog;
        };

        this.closeDialog = function () {
            if (dialog != null) {
                var window = dialog.data("kendoWindow");
                window.close();
            }
        };

        //TODO: maybe we should move the binding outside of this?!
        this.showDialog = function (bindingTargetSelector, title, url, viewModel) {

            var dialog = createDialog();

            var window = dialog.data("kendoWindow");

            var refreshCallback = function () {
                window.unbind('refresh', refreshCallback);
                kendo.bind($(bindingTargetSelector), viewModel);
            };

            window.bind("refresh", refreshCallback);

            window.refresh({ url: url })
                  .title(title)
                  .center()
                  .open();

            return window;
        };

        this.compileTemplate = function (templateName, viewModel) {
            var template = kendo.template($("#" + templateName).html());
            return template(viewModel);
        };

        this.showTeam = function (team) {
            var teamViewModel = new TeamDashboard(that, team);
            changeContent("teamDashboardTemplate", teamViewModel);
            kendo.bind($("#teamDashboard"), teamViewModel);
        };

        this.showLogin = function () {
            var signInViewModel = new SignInViewModel(that);
            changeContent("userSignInTemplate", signInViewModel);
            kendo.bind($("#userLogin"), signInViewModel);

            $('#navigation').hide(); //TODO: should be able to do style binding?

            //TODO: remove this after the release as you will be able to bind in templates :)
            $("#userLogin").delegate("#userName", "change", function () {
                signInViewModel.userName = this.value;
            });
        };

        function changeContent(templateName, viewModel) {

            var compiledTemplate;

            //cache caused issues (maybe force a refresh?) also need to include an identifier in the cache key
            //if (views[templateName] == null) {
            var template = kendo.template($("#" + templateName).html());
            compiledTemplate = that.compileTemplate(templateName, viewModel);
            views[templateName] = compiledTemplate;
            //            }
            //            else {
            //                compiledTemplate = views[templateName];
            //            }

            $("#appFrame").html(compiledTemplate);
        };

        //        this.previous = function () {
        //            //$("#appFrame").html(views.pop());
        //        };
        //        this.forward = function () {
        //            //$("#appFrame").html(views.p());
        //        };

        this.showErrors = function (errors) {
            var strErrors = "";
            if ($.isArray(errors)) {
                $.each(errors, function (index, err) {
                    strErrors += "*" + err + "\n";
                });
            }
            else {
                strErrors = errors;
            }
            alert(strErrors);
        };

        this.loadUser = function (user) {

            if (activeUserViewModel != null) {
                kendo.unbind($("#userDashboard"), activeUserViewModel);
            }

            activeUserViewModel = new UserDashboard(that, user);
            changeContent("userDashboardTemplate", activeUserViewModel);

            $('#navigation').show(); //TODO: should be able to do style binding?

            kendo.bind($("#userDashboard"), activeUserViewModel);
        };

        this.init = function () {

            //kendo.bind(this.context, this);

            configureRoutes();
            configureDataProvider();
            configureConnectivityDetection();

            that.showLogin();
        };

        function configureDataProvider() {
            that.dataProvider = new DataProvider(that.showErrors);
        };

        var isOnline;
        function connectivityChanged() {
            //here would swap out the data provider, and wire up some sync operations if needed
            if (navigator.onLine) {
                alert('online');
            }
            else {
                alert('offline');
            }
        };

        function configureConnectivityDetection() {
            setInterval(function () {
                if (isOnline != null && navigator.onLine != isOnline) {
                    connectivityChanged();
                }

                isOnline = navigator.onLine;
            }, 1000);
        };

        function configureRoutes() {

            // Here we define our routes.  You'll notice that I only define three routes, even
            // though there are four links.  Each route has an action assigned to it (via the 
            // `to` method, as well as an `enter` method.  The `enter` method is called before
            // the route is performed, which allows you to do any setup you need (changes classes,
            // performing AJAX calls, adding animations, etc.
            Path.map("/my").to(function () {
                that.loadUser(activeUserViewModel.user);
            }).enter(function () {
                if (activeUserViewModel == null) {
                    that.showLogin();
                    return false;
                }
            });

            Path.map("/logout").to(function () {
                that.activeUserViewModel = null;
                //that.showLogin();
                location.href = "/index.html";
            });

            Path.map("/team/:teamId").to(function () {
                that.dataProvider.get('api/team/' + this.params["teamId"], that.showTeam);
            }).enter(function () {
                if (activeUserViewModel == null) {
                    that.showLogin();
                    return false;
                }
            });

            Path.rescue(notFound);
            Path.root("/index.html");

            Path.history.listen();

            $('body').on('click', 'a.nav', function (event) {
                event.preventDefault();
                Path.history.pushState({}, "", $(this).attr("href"));
            });
        };

        function notFound() {
            alert("BAD PAGE DOOODE");
        };

        this.getCurrentUser = function () { return activeUserViewModel; };

        return this;
    };

    //TODO: can I leverage the datasource here?!
    function DataProvider(failback) {
        var that = this;
        this.failback = failback

        function handleErrors(errors, failback) {
            if (failback == null) {
                that.failback(errors);
            }
            else {
                failback(errors);
            }
        }

        this.createResource = function (url, model, success, fail) {
            $.ajax({
                url: url,
                data: JSON.stringify(model),
                type: "POST",
                contentType: "application/json;charset=utf-8",
                statusCode: {
                    201: function (newResource) {
                        if (success) {
                            success(newResource);
                        }
                    },
                    400: function (xhr) {
                        var errors = JSON.parse(xhr.responseText);
                        handleErrors(errors, fail);
                    }
                }
            });
        };

        this.updateResource = function (url, model, success, fail) {
            $.ajax({
                url: url,
                type: "PUT",
                data: JSON.stringify(model),
                contentType: "application/json;charset=utf-8",
                statusCode: {
                    200: function (result) {
                        if (success) {
                            success(result);
                        }
                    },
                    400: function (xhr) {
                        var errors = JSON.parse(xhr.responseText);
                        handleErrors(errors, fail);
                    }
                }
            });
        };

        this.removeResource = function (url, data, success, fail) {
            $.ajax({
                url: url,
                type: "DELETE",
                data: JSON.stringify(data),
                contentType: "application/json;charset=utf-8",
                statusCode: {
                    200: function (resource) {
                        if (success) {
                            success(resource);
                        }
                    },
                    204: function (resource) {
                        if (success) {
                            success(resource);
                        }
                    },
                    400: function (xhr) {
                        var errors = JSON.parse(xhr.responseText);
                        handleErrors(errors, fail);
                    }
                }
            });
        };

        this.get = function (url, success, fail) {
            $.ajax({
                url: url,
                type: "GET",
                contentType: "application/json;charset=utf-8",
                statusCode: {
                    200: function (resource) {
                        if (success) {
                            success(resource);
                        }
                    },
                    400: function (xhr) {
                        var errors = JSON.parse(xhr.responseText);
                        handleErrors(errors, fail);
                    }
                }
            });
        };

        //TODO: is this needed?! the only diff between it and create is handling 200 and 201?!
        this.post = function (url, data, success, fail) {
            $.ajax({
                url: url,
                data: JSON.stringify(data),
                type: "POST",
                contentType: "application/json;charset=utf-8",
                statusCode: {
                    200: function (resource) {
                        success(resource);
                    },
                    400: function (xhr) {
                        var errors = JSON.parse(xhr.responseText);
                        handleErrors(errors, fail);
                    }
                }
            });
        };

        return this;
    };

    application.init = function ($contextEl) {
        var app = new Application($contextEl);
        app.init();
    }

    return application;

} (window.application = window.application || {}, jQuery, kendo));