(function (application, $, kendo, undefined) {
    "use strict";
    var application;

    function TeamDashboard(team) {
        var that = this;
        this.team = team;

        this.pendingTeamMembers = $.map(team.PendingTeamMembers, function (member) {
            return new PendingMemberListItemViewModel(member, that);
        });

        this.teamMembers = $.map(team.TeamMembers, function (member) {
            return new TeamMemberListItemViewModel(member, that);
        });

        this.thingList = new ThingListViewModel(team.Things);

        function thingAdded(thing) {
            refresh();
        };

        function refresh() {
            //TODO: Memory leak?!?
            application.showTeam(that.team);
        }

        this.userIsAdmin = function (user) {
            return true; //TODO: check if passed user is an admin for the team
        };

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
            application.dataProvider.updateResource("/api/team/" + teamId + "/denymember", approvalInfo, this.refresh);
        };

        this.approveMember = function (user) {
            var teamId = that.team.Id;
            var approvalInfo = { teamId: teamId, userId: user.Id };

            //ToDO: will this cause a memory leak?!
            application.dataProvider.updateResource("/api/team/" + teamId + "/approvemember", approvalInfo, this.refresh);
        };

        this.addThing = function () {

            var newThingModel = {
                description: "",
                availableTeamMembers: that.team.TeamMembers,
                selectedTeamMembers: [],
                save: function (e) {

                    //get the current application user's id
                    var currentUserId = application.user.Id;
                    var assignedTo = $.map(this.selectedTeamMembers, function (member) { return member.Id });

                    var vm = this;
                    //create the thing object
                    var thing = { CreatedById: currentUserId, Description: this.description, AssignedTo: assignedTo, TeamId: that.team.Id };

                    //save the new thing back to the server
                    application.dataProvider.createResource("/api/thing", thing, function (result) {
                        application.closeDialog(); //ewww
                        thingAdded(result);

                        kendo.unbind($("#thingEditor"), vm);
                    });
                },
                cancel: function (e) {
                    application.closeDialog(); //ewww
                    kendo.unbind($("#thingEditor"), this);
                }
            };

            application.showDialog("#thingEditor", "Add a Thing", "/thingEditor.html", newThingModel);
        };

        return this;
    };

    function UserDashboard(_user) {
        var that = this;
        this.user = _user;

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
            application.loadUser(that.user);
        }

        function teamRemoved(removedTeam) {

            that.user.Teams = $.grep(that.user.Teams, function (e) { return e.Id != removedTeam.Id });
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
                    application.dataProvider.get("/api/team?$filter=Name ne null and tolower(Name) eq '" + searchedName.toLowerCase() + "'", function (result) {

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
                    var currentUserId = application.user.Id;

                    //create the team object
                    var team = { name: this.name, userId: currentUserId };
                    var vm = this;
                    //save the new team back to the server
                    application.dataProvider.updateResource("/api/team/" + this.foundTeamId + "/join", team, function (result) {
                        application.closeDialog(); //ewww
                        teamJoined(result);
                        kendo.unbind($("#joinTeam"), vm);
                    });
                },
                cancel: function (e) {
                    application.closeDialog(); //ewww
                    kendo.unbind($("#joinTeam"), this);
                }
            });

            application.showDialog("#joinTeam", "Join a Team", "/joinTeam.html", joinTeamViewModel);
        };

        this.removeTeam = function (team) {
            if (confirm("SRSLY?")) {
                var currentUserId = application.user.Id;
                application.dataProvider.removeResource('/api/team/' + team.Id, { userId: currentUserId }, function () {
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
                    application.dataProvider.get("/api/team?$filter=Name ne null and tolower(Name) eq '" + searchedName.toLowerCase() + "'", function (result) {

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
                    var currentUserId = application.user.Id;

                    var editedTeam = { id: this.id, name: this.name, ispublic: this.isPublic, updatedbyid: currentUserId };
                    var vm = this;
                    //save the new thing back to the server
                    application.dataProvider.updateResource("/api/team/" + editedTeam.id, editedTeam, function (result) {
                        application.closeDialog(); //ewww
                        teamUpdated(result);

                        kendo.unbind($("#newTeam"), vm);
                    });
                },
                cancel: function (e) {
                    application.closeDialog(); //ewww
                    kendo.unbind($("#newTeam"), this);
                }
            };

            application.showDialog("#newTeam", "Edit Team", "/teamEditor.html", teamEditorViewModel);
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
                    application.dataProvider.get("/api/team?$filter=Name ne null and tolower(Name) eq '" + searchedName.toLowerCase() + "'", function (result) {

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
                    var currentUserId = application.user.Id;

                    //create the thing object
                    var team = { name: this.name, ispublic: this.isPublic, createdById: currentUserId };

                    //save the new thing back to the server
                    application.dataProvider.createResource("/api/team", team, function (result) {
                        application.closeDialog(); //ewww
                        teamCreated(result);

                        kendo.unbind($("#newTeam"), this);
                    });
                },
                cancel: function (e) {
                    kendo.unbind($("#newTeam"), this);
                    application.closeDialog(); //ewww
                }
            };

            application.showDialog("#newTeam", "Create a New Team", "/teamEditor.html", newTeamViewModel);
        };

        this.viewTeam = function (id) {
            //todo: this is kinda ugly
            application.dataProvider.get("api/team/" + id, application.showTeam);
        };

        this.teams = $.map(this.user.Teams, function (team) {
            return new TeamListItemViewModel(team, that);
        });

        this.pendingTeams = $.map(this.user.PendingTeams, function (team) {
            return new TeamListItemViewModel(team, that);
        });

        this.thingList = new ThingListViewModel(this.user.Things);

        return this;
    };

    function ThingListViewModel(things) {

        var that = this;

        var activeThings = things.filter(function (t) {
            return t.Status == "InProgress";
        });

        var completedThings = things.filter(function (t) {
            return t.Status == "Completed";
        });

        this.allTheThings = $.map(things, createThingViewModel);
        this.activeThings = $.map(activeThings, createThingViewModel);
        this.completedThings = $.map(completedThings, createThingViewModel);

        this.edit = function (editedThing) {

            application.dataProvider.get("/api/team/" + editedThing.Team.Id, function (result) {
                var thingEditModel = {
                    description: editedThing.Description,
                    availableTeamMembers: result.TeamMembers,
                    selectedTeamMembers: $.map(editedThing.AssignedTo, function (member) { return member }),
                    save: function (e) {

                        //get the current application user's id
                        var currentUserId = application.user.Id;

                        var assignedTo = $.map(this.selectedTeamMembers, function (member) { return member.Id });

                        //create the thing object
                        var thing = { EditedById: currentUserId, Description: this.description, AssignedTo: assignedTo };

                        //save the new thing back to the server
                        application.dataProvider.updateResource("/api/thing/" + editedThing.Id, thing, function (result) {
                            application.closeDialog(); //ewww
                            thingAdded(result);
                        });
                    },
                    cancel: function (e) {
                        application.closeDialog(); //ewww
                    }
                };

                application.showDialog("#thingEditor", "Edit a Thing", "/thingEditor.html", thingEditModel);
            });

        };

        function createThingViewModel(thing) {
            return new ThingListItemViewModel(thing, that);
        }
        function thingAdded(thing) {
            //Oh the horror
            var thingVm = createThingViewModel(thing);
            that.activeThings.push(thingVm);
            that.allTheThings.push(thingVm);
        }

        function thingRemoved(thing) {
            //Oh the horror
            that.allTheThings = $.grep(that.allTheThings, function (e) { return e.Id != thing.Id });
            that.activeThings = $.grep(that.activeThings, function (e) { return e.Id != thing.Id });
            that.completedThings = $.grep(that.completedThings, function (e) { return e.Id != thing.Id });
        }

        function thingCompleted(thing) {
            //Oh the horror
            that.activeThings = $.grep(that.activeThings, function (e) { return e.Id != thing.Id });
            that.completedThings.push(createThingViewModel(thing));
        }

        this.remove = function (thing) {
            if (confirm("SRSLY?")) {
                var currentUserId = application.user.Id;
                application.dataProvider.removeResource('/api/thing/' + thing.Id, { DeletedById: currentUserId }, function () {
                    thingRemoved(thing);
                });
            }
        };

        this.complete = function (thing) {
            if (confirm("SRSLY?")) {
                var currentUserId = application.user.Id;
                application.dataProvider.updateResource('/api/thing/' + thing.Id + '/complete', { userId: currentUserId }, function () {
                    thingCompleted(thing);
                });
            }
        };

        return this;
    }

    function ThingListItemViewModel(thing, thingListViewModel) {

        var parent = thingListViewModel;
        this.thing = thing;

        this.edit = function (e) { parent.edit(this.thing); };
        this.remove = function (e) { parent.remove(this.thing); };
        this.complete = function (e) { parent.complete(this.thing); };

        this.userCanEdit = function () {
            var applicationUser = application.user;
            var assignedUsers = $.map(thing.AssignedTo, function (user) {
                return user.Id;
            });
            //if (applicationUser != null && (this.thing.OwnerId === applicationUser.Id || $.inArray(applicationUser.Id, assignedUsers) != -1)) {
            if (applicationUser != null && this.thing.Owner.Id === applicationUser.Id ) {
                return true;
            }
            return false;
        };

        this.userCanRemove = function () {
            var applicationUser = application.user;

            if (applicationUser != null && this.thing.Owner.Id === applicationUser.Id) {
                return true;
            }
            return false;
        };

        this.userCanComplete = function () {
            var applicationUser = application.user;

            var assignedToIds = $.map(this.thing.AssignedTo, function (thingMember) {
                return thingMember.Id;
            });

            if (applicationUser != null && $.inArray(applicationUser.Id, assignedToIds) != -1) {
                return true;
            }
            return false;
        };

        return this;
    };

    function TeamListItemViewModel(team, userController) {

        var user = application.user;
        var parent = userController;

        this.team = team;

        this.editTeam = function (e) { parent.editTeam(this.team); };
        this.removeTeam = function (e) { parent.removeTeam(this.team); };


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

    function PendingMemberListItemViewModel(pendingMember, teamController) {
        var parent = teamController;
        this.pendingMember = pendingMember;

        this.approveUser = function (e) { parent.approveMember(this.pendingMember); };
        this.denyUser = function (e) { parent.denyMember(this.pendingMember); };

        this.userCanApprove = function () {

            if (parent.userIsAdmin(application.user)) {
                return true;
            }

            return false;
        };
    };

    function TeamMemberListItemViewModel(user, teamController) {
        var parent = teamController;
        this.user = user;

        this.viewUser = function (e) { parent.viewMember(this.user); };
        this.editUser = function (e) { parent.editMember(this.user); };
        this.denyUser = function (e) { parent.denyMember(this.user); };
        this.hint = function (element) {
            return element.clone();
        }
        this.userCanApprove = function () {

            if (parent.userIsAdmin(application.user) || user.Id == application.user.Id) {
                return true;
            }

            return false;
        };
    };

    function SignInViewModel() {
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

        function userLoaded(user) {
            application.user = user;
            application.loadUser(user);
        }

        this.signInUser = function () {
            if (validateInput()) {
                //TODO: that should be changed to this after release
                var userInfo = { EmailAddress: that.userName };
                //todo: this is kinda ugly
                application.dataProvider.post("/api/user/signin", userInfo, userLoaded);
            }
        };

        this.registerUser = function () {
            if (validateInput()) {
                //TODO: that should be changed to this after release
                var userInfo = { EmailAddress: that.userName };
                //todo: this is kinda ugly
                application.dataProvider.createResource("/api/user/register", userInfo, userLoaded);
            }
        };

        return this;
    }

    function TeamThingApplication(_context) {

        this.dataProvider;
        this.user = null;

        var dialog;
        var views = [];
        var isOnline;
        var context = _context;
        var that = this;

        function changeContent(templateName, viewModel) {

            //cache caused issues (maybe force a refresh?) also need to include an identifier in the cache key
            //if (views[templateName] == null) {
            //var template = kendo.template($("#" + templateName).html());
            var compiledTemplate = that.compileTemplate(templateName, viewModel);
            views[templateName] = compiledTemplate;
            //            }
            //            else {
            //                compiledTemplate = views[templateName];
            //            }

            $("#appFrame").html(compiledTemplate);
        }

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
                window.content(' ') //empty string does not work, it still returns the content
                      .close();
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
            //todo: bind on close to unbind view model?
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
            var teamViewModel = new TeamDashboard(team);
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

        var activeUserViewModel; //TODO: get rid of this
        this.loadUser = function (user) {

            if (activeUserViewModel != null) {
                kendo.unbind($("#userDashboard"), activeUserViewModel);
            }

            activeUserViewModel = new UserDashboard(user);
            changeContent("userDashboardTemplate", activeUserViewModel);

            $('#navigation').show(); //TODO: should be able to do style binding?

            kendo.bind($("#userDashboard"), activeUserViewModel);
        };

        this.appViewModel = {
            user: null
        };

        function init() {

            kendo.bind(context, that);

            configureRoutes();
            configureDataProvider();
            configureConnectivityDetection();

            that.showLogin();
        };

        function configureDataProvider() {
            that.dataProvider = new DataProvider(that.showErrors);
        };

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
                location.href = Path.history.initial.URL;
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
            Path.root("/");

            Path.history.listen();

            $('body').on('click', 'a.nav', function (event) {
                event.preventDefault();
                Path.history.pushState({}, "", $(this).attr("href"));
            });
        };

        function notFound() {
            alert("BAD PAGE DOOODE");
        };

        init(context);
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

    teamThing.init = function ($contextEl) {
        application = new TeamThingApplication($contextEl);
    }

    return teamThing;

} (window.teamThing = window.teamThing || {}, jQuery, kendo));