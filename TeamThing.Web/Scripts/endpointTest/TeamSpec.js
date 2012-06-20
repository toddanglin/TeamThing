
///<reference path="../jquery-1.7.1.min.js"/>
describe("A team", function () {
    //baseAddress is defined in test set up
    var baseResourceAddress = baseAddress + "team/";
    var createdResource;

    function createdResourceBaseAddress() {
        if (createdResource) {
            return baseResourceAddress + createdResource.id;
        }

        alert("Created team is null, all team tests will fail.");
    }

    it("should be able to be retrived in a list", function () {

        var successCallback = jasmine.createSpy();
        get(baseResourceAddress, successCallback, null);

        waitsFor(function () {
            return successCallback.callCount > 0;
        });
        runs(function () {
            expect(successCallback).toHaveBeenCalled();
        });
    });

    it("should be able to be created", function () {

        var successCallback = jasmine.createSpy();

        var successCallbackWrapper = function (result) {

            createdResource = result;
            successCallback(result);
        };

        /*Todo: this is a little hard coded at the moment, tests will break if user is ever deleted!*/

        var data = {
            name: "My New Team - Test Jasmine " + Math.random(),
            ispublic: false,
            createdById: 10
        };

        create(baseResourceAddress, data, successCallbackWrapper);

        waitsFor(function () {
            return successCallback.callCount > 0;
        });
        runs(function () {
            expect(successCallback).toHaveBeenCalled();
        });
    });

    describe("Once created", function () {

        it("the creator should be the owner", function () {

            expect(createdResource.ownerId).toEqual(10);
        });

        it("the creator should be a team admin", function () {

            expect(createdResource.administrators).toEqual([10]);
        });

        it("should be able to be retrived by its id", function () {


            var successCallback = jasmine.createSpy();
            get(createdResourceBaseAddress(), successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to retrieve its members", function () {


            var successCallback = jasmine.createSpy();
            get(createdResourceBaseAddress() + "/members", successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to retrieve its things", function () {


            var successCallback = jasmine.createSpy();
            get(createdResourceBaseAddress() + "/things", successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to retrieve its stats", function () {


            var successCallback = jasmine.createSpy();
            get(createdResourceBaseAddress() + "/stats", successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to be joined by a user", function () {


            var successCallback = jasmine.createSpy();
            
            var data = { "userId": 12 };
            put(createdResourceBaseAddress() +"/join", data, successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        describe("Once a member joins they ", function () {
            it("should NOT be able to be approved for a team they are not a member of - covers issue reported by graham", function () {

                var errorCallback = jasmine.createSpy();

                var data = {
                    'userId': 0,
                    statusChangedByUserId: createdResource.administrators[0]
                };
                put(createdResourceBaseAddress() + "/approvemember", data, null, errorCallback);

                waitsFor(function () {
                    return errorCallback.callCount > 0;
                });
                runs(function () {
                    expect(errorCallback).toHaveBeenCalled();
                });
            });
            it("should be able to be approved by a team admin", function () {

                var successCallback = jasmine.createSpy();

                var data = { "userId": 12, statusChangedByUserId: createdResource.administrators[0] };
                put(createdResourceBaseAddress() + "/approvemember", data, successCallback, null);

                waitsFor(function () {
                    return successCallback.callCount > 0;
                });
                runs(function () {
                    expect(successCallback).toHaveBeenCalled();
                });
            });

            it("should be able to be approved by a team owner", function () {

                var successCallback = jasmine.createSpy();

                var data = { "userId": 12, statusChangedByUserId: createdResource.ownerId };
                put(createdResourceBaseAddress() + "/approvemember", data, successCallback, null);

                waitsFor(function () {
                    return successCallback.callCount > 0;
                });
                runs(function () {
                    expect(successCallback).toHaveBeenCalled();
                });
            });

            it("should not be able to be approved by themselves", function () {

                var errorCallback = jasmine.createSpy();

                var data = { userId: createdResource.ownerId, statusChangedByUserId: 12 };
                put(createdResourceBaseAddress() + "/denymember", data, null, errorCallback);

                waitsFor(function () {
                    return errorCallback.callCount > 0;
                });
                runs(function () {
                    expect(errorCallback).toHaveBeenCalled();
                });
            });

            it("should not be able to be approved by invalid user", function () {

                var errorCallback = jasmine.createSpy();

                var data = { userId: createdResource.ownerId, statusChangedByUserId: 0 };
                put(createdResourceBaseAddress() + "/denymember", data, null, errorCallback);

                waitsFor(function () {
                    return errorCallback.callCount > 0;
                });
                runs(function () {
                    expect(errorCallback).toHaveBeenCalled();
                });
            });

            it("should be able to be denied by a team admin", function () {

                var successCallback = jasmine.createSpy();

                var data = { "userId": 12, statusChangedByUserId: createdResource.administrators[0] };
                put(createdResourceBaseAddress() + "/denymember", data, successCallback, null);

                waitsFor(function () {
                    return successCallback.callCount > 0;
                });
                runs(function () {
                    expect(successCallback).toHaveBeenCalled();
                });
            });



            it("should be able to be denied by a team owner", function () {

                var successCallback = jasmine.createSpy();

                var data = { "userId": 12, statusChangedByUserId: createdResource.ownerId };
                put(createdResourceBaseAddress() + "/denymember", data, successCallback, null);

                waitsFor(function () {
                    return successCallback.callCount > 0;
                });
                runs(function () {
                    expect(successCallback).toHaveBeenCalled();
                });
            });

            it("should not be able to be denied by themselves", function () {

                var errorCallback = jasmine.createSpy();

                var data = { userId: createdResource.ownerId, statusChangedByUserId: 12 };
                put(createdResourceBaseAddress() + "/denymember", data, null, errorCallback);

                waitsFor(function () {
                    return errorCallback.callCount > 0;
                });
                runs(function () {
                    expect(errorCallback).toHaveBeenCalled();
                });
            });

            it("should not be able to be denied by invalid user", function () {

                var errorCallback = jasmine.createSpy();

                var data = { userId: createdResource.ownerId, statusChangedByUserId: 0 };
                put(createdResourceBaseAddress() + "/denymember", data, null, errorCallback);

                waitsFor(function () {
                    return errorCallback.callCount > 0;
                });
                runs(function () {
                    expect(errorCallback).toHaveBeenCalled();
                });
            });

            it("should not be able to be kicked from a team if they are the owner", function () {

                var errorCallback = jasmine.createSpy();

                var data = { userId: createdResource.ownerId, statusChangedByUserId: createdResource.ownerId };
                put(createdResourceBaseAddress() + "/denymember", data, null, errorCallback);

                waitsFor(function () {
                    return errorCallback.callCount > 0;
                });
                runs(function () {
                    expect(errorCallback).toHaveBeenCalled();
                });
            });

            it("should be able to be leave the team", function () {

                var successCallback = jasmine.createSpy();

                var data = { "userId": 12 };
                put(createdResourceBaseAddress() + "/leave", data, successCallback, null);

                waitsFor(function () {
                    return successCallback.callCount > 0;
                });
                runs(function () {
                    expect(successCallback).toHaveBeenCalled();
                });
            });
        });

        it("should be able to have members invited by the owner using an email address", function () {

            var successCallback = jasmine.createSpy();

            var data = {
                "addedByUserId": createdResource.ownerId,
                "emailAddress": "random@someplace.comd"
            }
            ;
            put(createdResourceBaseAddress() + "/addmember", data, successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to have members invited by an admin using an email address", function () {

            var successCallback = jasmine.createSpy();

            var data = {
                "addedByUserId": createdResource.administrators[0],
                "emailAddress": "random@someplace.comd23"
            }
            ;
            put(createdResourceBaseAddress() + "/addmember", data, successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should not be able to be added to a team if they already exist", function () {

            var errorCallback = jasmine.createSpy();

            var data = {
                "addedByUserId": createdResource.administrators[0],
                "emailAddress": "random@someplace.comd"
            }
            ;
            put(createdResourceBaseAddress() + "/addmember", data, null, errorCallback);

            waitsFor(function () {
                return errorCallback.callCount > 0;
            });
            runs(function () {
                expect(errorCallback).toHaveBeenCalled();
            });
        });

        it("should be able to be updated", function () {
            var successCallback = jasmine.createSpy();

            //reset created thing so we have the updated assigned to info
            var successCallbackWrapper = function (result) {
                createdResource = result;
                successCallback(result);
            };


            var data = {
                name: "My Updated Team - Test Jasmine 3" + Math.random(),
                ispublic: true,
                updatedById: 10
            };

            put(createdResourceBaseAddress(), data, successCallbackWrapper, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should not be able to be deleted by someone that is not a team admin", function () {
            var errorCallback = jasmine.createSpy();

            var data = { userId: 0/*0 is always an invalid userid*/ };
            remove(createdResourceBaseAddress(), data, null, errorCallback);

            waitsFor(function () {
                return errorCallback.callCount > 0;
            });
            runs(function () {
                expect(errorCallback).toHaveBeenCalled();
            });
        });

        it("should be able to be deleted by a team admin", function () {
            var successCallback = jasmine.createSpy();

            var data = { userId: createdResource.administrators[0] };
            remove(createdResourceBaseAddress(), data, successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });
    });


    //describe("should be able to updated", function() {
    //  beforeEach(function() {
    //    player.play(song);
    //    player.pause();
    //  });

    //  it("should be able to star", function() {

    //  });

    //  it("should be possible to resume", function() {
    //    player.resume();
    //    expect(player.isPlaying).toBeTruthy();
    //    expect(player.currentlyPlayingSong).toEqual(song);
    //  });
    //});

    //// demonstrates use of spies to intercept and test method calls
    //it("tells the current song if the user has made it a favorite", function() {
    //  spyOn(song, 'persistFavoriteStatus');

    //  player.play(song);
    //  player.makeFavorite();

    //  expect(song.persistFavoriteStatus).toHaveBeenCalledWith(true);
    //});

    ////demonstrates use of expected exceptions
    //describe("#resume", function() {
    //  it("should throw an exception if song is already playing", function() {
    //    player.play(song);

    //    expect(function() {
    //      player.resume();
    //    }).toThrow("song is already playing");
    //  });
    //});
});