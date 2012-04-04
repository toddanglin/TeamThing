(function (application, $, kendo, undefined) {
    "use strict";
    var application;

    function TeamDashboard(team) {
        var that = this;
        this.team = team;

        this.pendingTeamMembers = $.map(team.pendingTeamMembers, function (member) {
            return new PendingMemberListItemViewModel(member, that);
        });

        this.teamMembers = $.map(team.teamMembers, function (member) {
            return new TeamMemberListItemViewModel(member, that);
        });

        this.thingList = new ThingListViewModel(this, team);

        this.refresh = function () {
            //TODO: Memory leak?!?
            application.showTeam(that.team);
        }

        function thingAdded(thing) {
            that.team.things.push(thing);
            that.refresh();
        }

        function memberApproved(user) {
            that.team.teamMembers.push(user);
            that.team.pendingTeamMembers = $.grep(that.team.pendingTeamMembers, function (e) { return e.id != user.id });            
            that.refresh();
        }

        this.userIsAdmin = function (user) {
            return true; //TODO: check if passed user is an admin for the team
        };

        this.editMember = function (user) {
            alert("edit" + user.id);
        };

        this.viewMember = function (user) {
            alert("view" + user.id);
        };

        this.denyMember = function (user) {

            var teamId = that.team.id;
            var approvalInfo = { userId: user.id };

            //ToDO: will this cause a memory leak?!
            application.dataProvider.updateResource("/api/team/" + teamId + "/denymember", approvalInfo, this.refresh);
        };
        
        this.approveMember = function (user) {
            var teamId = that.team.id;
            var approvalInfo = { userId: user.id };

            //ToDO: will this cause a memory leak?!
            application.dataProvider.updateResource("/api/team/" + teamId + "/approvemember", approvalInfo, function () {
                memberApproved(user);
            });
        };

        this.addThing = function () {

            var newThingModel = {
                description: "",
                availableTeamMembers: that.team.teamMembers,
                selectedTeamMembers: [],
                save: function (e) {

                    //get the current application user's id
                    var currentUserId = application.user.id;
                    var assignedTo = $.map(this.selectedTeamMembers, function (member) { return member.id });

                    var vm = this;
                    //create the thing object
                    var thing = { createdById: currentUserId, description: this.description, assignedTo: assignedTo, teamId: that.team.id };

                    //save the new thing back to the server
                    application.dataProvider.createResource("/api/thing", thing, function (result) {
                        application.closeDialog(); //ewww
                        thingAdded(result);
                    });
                },
                cancel: function (e) {
                    application.closeDialog(); //ewww
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
                that.viewTeam(that.teams[0].id);
            }
        };

        function teamUpdated(team) {
            //TODO: Update team info in UI

            var teams = that.user.teams;
            var i = teams.length;
            while (i--) {
                var currentTeam = teams[i];
                if (currentTeam.id == team.id) {
                    teams[i] = team;
                }
            }
            that.refresh();
        }

        function teamCreated(team) {
            that.user.teams.push(team);
            that.refresh();
        }

        this.refresh = function () {
            //TODO: Memory leak?!?
            application.loadUser(that.user);
        }

        function teamRemoved(removedTeam) {

            that.user.teams = $.grep(that.user.teams, function (e) { return e.id != removedTeam.id });
            that.refresh();
        }

        function teamJoined(team) {
            if (team.isPublic) {
                that.user.teams.push(team);
            }
            else {
                that.user.pendingTeams.push(team);
            }

            that.refresh();
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
                            vm.set('foundTeamId', result[0].id);
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
                    var currentUserId = application.user.id;

                    //create the team object
                    var team = { userId: currentUserId, id:this.foundTeamId };
                    var vm = this;
                    //save the new team back to the server
                    application.dataProvider.updateResource("/api/team/" + this.foundTeamId + "/join", team, function (result) {
                        application.closeDialog(); //ewww
                        teamJoined(result);
                    });
                },
                cancel: function (e) {
                    application.closeDialog(); //ewww
                }
            });

            application.showDialog("#joinTeam", "Join a Team", "/joinTeam.html", joinTeamViewModel);
        };

        this.removeTeam = function (team) {
            if (confirm("SRSLY?")) {
                var currentUserId = application.user.id;
                var vm = { userId: currentUserId };
                application.dataProvider.removeResource('/api/team/' + team.id, vm, function () {
                    teamRemoved(team);
                });
            }
        };

        this.editTeam = function (team) {

            var teamEditorViewModel = {
                name: team.name,
                id: team.id,
                isPublic: team.isPublic,
                canSave: true,
                searchStatusClass: 'ok',
                searchStatusMessage: '',
                searchTeams: function (e) {
                    var searchedName = this.name;
                    var vm = this;
                    //TODO: instead of an exact match we could let users se how many partials matches there are?
                    application.dataProvider.get("/api/team?$filter=Name ne null and tolower(Name) eq '" + searchedName.toLowerCase() + "'", function (result) {

                        if (result.length > 0 && result[0].id != vm.id) {
                            vm.set('canSave', false);
                            vm.set('searchStatusClass', 'searchResult error');
                            vm.set('searchStatusMessage', 'Team Name Not Available');
                        }
                        else if (result.length > 0 && result[0].id == vm.id) {
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
                    var currentUserId = application.user.id;

                    var editedTeam = { id: this.id, name: this.name, ispublic: this.isPublic, updatedbyid: currentUserId };
                    var vm = this;
                    //save the new thing back to the server
                    application.dataProvider.updateResource("/api/team/" + editedTeam.id, editedTeam, function (result) {
                        application.closeDialog(); //ewww
                        teamUpdated(result);
                    });
                },
                cancel: function (e) {
                    application.closeDialog(); //ewww
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
                    var currentUserId = application.user.id;

                    //create the thing object
                    var team = { name: this.name, ispublic: this.isPublic, createdById: currentUserId };

                    //save the new thing back to the server
                    application.dataProvider.createResource("/api/team", team, function (result) {
                        application.closeDialog(); //ewww
                        teamCreated(result);
                    });
                },
                cancel: function (e) {
                    application.closeDialog(); //ewww
                }
            };

            application.showDialog("#newTeam", "Create a New Team", "/teamEditor.html", newTeamViewModel);
        };

        this.viewTeam = function (id) {
            //todo: this is kinda ugly
            application.dataProvider.get("api/team/" + id, application.showTeam);
        };

        this.teams = $.map(this.user.teams, function (team) {
            return new TeamListItemViewModel(team, that);
        });

        this.pendingTeams = $.map(this.user.pendingTeams, function (team) {
            return new TeamListItemViewModel(team, that);
        });

        this.thingList = new ThingListViewModel(this, this.user);

        return this;
    };

    function ThingListViewModel(parent, _thingContainer) {

        var that = this;
        var things = _thingContainer.things;
        var thingContainer = _thingContainer;

        var activeThings = things.filter(function (t) {
            return t.status == "InProgress";
        });
        var completedThings = things.filter(function (t) {
            return t.status == "Completed";
        });

        this.allTheThings = $.map(things, createThingViewModel);
        this.activeThings = $.map(activeThings, createThingViewModel);
        this.completedThings = $.map(completedThings, createThingViewModel);

        function createThingViewModel(thing) {
            return new ThingListItemViewModel(thing, that);
        }

        function thingAdded(thing) {
            thingContainer.things.push(thing);
            parent.refresh();
        }

        function thingRemoved(thing) {
            thingContainer.things = $.grep(thingContainer.things, function (e) { return e.id != thing.id });
            parent.refresh();
        }

        function thingUpdated(thing) {

            var i = thingContainer.things.length;
            while (i--) {
                var current = thingContainer.things[i];
                if (current.id == thing.id) {
                    thingContainer.things[i] = thing;
                }
            }
            parent.refresh();
        }

        this.edit = function (editedThing) {

            application.dataProvider.get("/api/team/" + editedThing.team.id, function (result) {
                var thingEditModel = {
                    description: editedThing.description,
                    availableTeamMembers: result.teamMembers,
                    selectedTeamMembers: $.map(editedThing.assignedTo, function (member) { return member }),
                    save: function (e) {

                        //get the current application user's id
                        var currentUserId = application.user.id;

                        var assignedTo = $.map(this.selectedTeamMembers, function (member) { return member.id });

                        //create the thing object
                        var thing = { editedById: currentUserId, description: this.description, assignedTo: assignedTo, id: editedThing.id };

                        //save the new thing back to the server
                        application.dataProvider.updateResource("/api/thing/" + editedThing.id, thing, function (result) {
                            application.closeDialog(); //ewww
                            thingUpdated(result);
                        });
                    },
                    cancel: function (e) {
                        application.closeDialog(); //ewww
                    }
                };

                application.showDialog("#thingEditor", "Edit a Thing", "/thingEditor.html", thingEditModel);
            });

        };
        this.remove = function (thing) {
            if (confirm("SRSLY?")) {
                var currentUserId = application.user.id;
                application.dataProvider.removeResource('/api/thing/' + thing.id, { deletedById: currentUserId, id:thing.id }, function () {
                    thingRemoved(thing);
                });
            }
        };
        this.complete = function (thing) {
            if (confirm("SRSLY?")) {
                var vm = { id: thing.id, userId: application.user.id };
                application.dataProvider.updateResource('/api/thing/' + thing.id + '/complete', vm, function (result) {
                    thingUpdated(result);
                });
            }
        };
        this.view = function (thing) {
            alert("view" + thing.id);
        };

        return this;
    }

    function ThingListItemViewModel(_thing, thingListViewModel) {

        var parent = thingListViewModel;
        this.thing = _thing;

        this.edit = function (e) { parent.edit(this.thing); };
        this.view = function (e) { parent.view(this.thing); };
        this.remove = function (e) { parent.remove(this.thing); };
        this.complete = function (e) { parent.complete(this.thing); };

        this.isDone = function () {
            return this.thing.status == "Completed";
        };

        this.userCanEdit = function () {
            var applicationUser = application.user;
            var assignedUsers = $.map(this.thing.assignedTo, function (user) {
                return user.id;
            });

            if (this.isDone == true) {
                return false;
            }

            //if (applicationUser != null && (this.thing.OwnerId === applicationUser.Id || $.inArray(applicationUser.Id, assignedUsers) != -1)) {
            if (applicationUser != null && this.thing.owner.id === applicationUser.id) {
                return true;
            }
            return false;
        };
        this.userCanRemove = function () {
            var applicationUser = application.user;

            if (this.isDone == true) {
                return false;
            }

            if (applicationUser != null && this.thing.owner.id === applicationUser.id) {
                return true;
            }
            return false;
        };
        this.userCanComplete = function () {
            var applicationUser = application.user;

            if (this.isDone==true){
                return false;
            }

            var assignedToIds = $.map(this.thing.assignedTo, function (thingMember) {
                return thingMember.id;
            });

            if ( applicationUser != null && $.inArray(applicationUser.id, assignedToIds) != -1) {
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
            if (user != null && (this.team.ownerId === user.id || $.inArray(user.id, this.team.administrators) != -1)) {
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
        this.user = pendingMember;

        this.approveUser = function (e) { parent.approveMember(this.user); };
        this.denyUser = function (e) { parent.denyMember(this.user); };

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

            if (parent.userIsAdmin(application.user) || user.id == application.user.id) {
                return true;
            }

            return false;
        };

        //this.userCanBeDenied = function () {

        //    if (parent.userIsAdmin(application.user) || user.id != application.user.id) {
        //        return true;
        //    }

        //    return false;
        //};
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
                var userInfo = { emailAddress: that.userName };
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
                kendo.unbind($(dialog.selector), dialog.viewModel);
                var window = dialog.data("kendoWindow");
                window.content(' ') //empty string does not work, it still returns the content
                      .close();
            }
        };


        //TODO: maybe we should move the binding outside of this?!
        this.showDialog = function (bindingTargetSelector, title, url, viewModel) {

            var dialog = createDialog();
            dialog.binding = {
                selector: bindingTargetSelector,
                viewModel: viewModel
            };

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
            //We need to unbind this
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
                that.dataProvider.get('/api/user/' + activeUserViewModel.user.id, that.loadUser);
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
                that.dataProvider.get('/api/team/' + this.params["teamId"], that.showTeam);
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