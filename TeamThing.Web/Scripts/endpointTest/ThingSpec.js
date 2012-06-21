r
///<reference path="../jquery-1.7.1.min.js"/>
describe("A thing", function () {

    //baseAddress is defined in test set up
    var baseThingAddress = baseAddress + "thing/";
    var createdThing;

    function createdThingBaseAddress() {
        if (createdThing) {
            return baseThingAddress + createdThing.id;
        }

        alert("Created thing is null, all tests will fail.");
    }

    it("should be able to be retrived in a list", function () {

        var successCallback = jasmine.createSpy();
        get(baseThingAddress, successCallback, null);

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

            createdThing = result;
            successCallback(result);
        };

        /*Todo: this is a little hard coded at the moment, tests will break if the team or user is ever deleted!*/
        /* in the set up method we need to retrieve a team, and 2 users (second user will be added in the update test to), and use that info here*/

        var data = {
            createdById: 10,
            description: "My New Thing",
            assignedTo: [10], //only assigned to one user at first
            teamId: 2
        };

        create(baseAddress + "/thing", data, successCallbackWrapper);

        waitsFor(function () {
            return successCallback.callCount > 0;
        });
        runs(function () {
            expect(successCallback).toHaveBeenCalled();
        });
    });

    describe("Once created", function () {

        it("should be able to be retrived by its id", function () {

            var successCallback = jasmine.createSpy();
            get(createdThingBaseAddress(), successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to be updated", function () {
            var successCallback = jasmine.createSpy();

            //reset created thing so we have the updated assigned to info
            var successCallbackWrapper = function (result) {
                createdThing = result;
                successCallback(result);
            };

            var data = {
                editedById: 10,
                description: "My Updated Thing",
                assignedTo: [10, 12] //Adding a new user with the id of '12' to the assigned to list
            };

            put(createdThingBaseAddress(), data, successCallbackWrapper, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to have status updated by someone it is assigned to", function () {
            var successCallback = jasmine.createSpy();

            var data = { userId: createdThing.assignedTo[0].id, status: "delayed" };

            put(createdThingBaseAddress() + "/updatestatus", data, successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to have status updated by its owner", function () {
            var successCallback = jasmine.createSpy();

            var data = { userId: createdThing.owner.id, status: "delayed" };

            put(createdThingBaseAddress() + "/updatestatus", data, successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to have status updated by someone that is an admin on the team", function () {
            var successCallback = jasmine.createSpy();

            /*TODO: we shouldn't hard code this*/
            var data = { userId: 10, status: "delayed" };
            put(createdThingBaseAddress() + "/updatestatus", data, successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to be completed by someone it is assigned to", function () {
            var successCallback = jasmine.createSpy();

            var data = { userId: createdThing.assignedTo[0].id };
            put(createdThingBaseAddress() + "/complete", data, successCallback, null);
            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to be completed by its owner", function () {
            var successCallback = jasmine.createSpy();

            var data = { userId: createdThing.owner.id };
            put(createdThingBaseAddress() + "/complete", data, successCallback, null);
            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to be completed by someone that is an admin on the team", function () {
            var successCallback = jasmine.createSpy();

            /*TODO: we shouldn't hard code this*/
            var data = { userId: 10 }; 
            put(createdThingBaseAddress() + "/complete", data, successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should not be able to be completed by someone that is not assigned to the thing, is not an owner, and is not an admin on the team", function () {
            var errorCallback = jasmine.createSpy();

            var data = { userId: 0 /*0 is always an invalid userid*/ };
            put(createdThingBaseAddress() + "/complete", data, null, errorCallback);

            waitsFor(function () {
                return errorCallback.callCount > 0;
            });
            runs(function () {
                expect(errorCallback).toHaveBeenCalled();
            });
        });

        it("should be able to be unstarred by the owner", function () {
            var successCallback = jasmine.createSpy();

            var data = { userId: createdThing.owner.id };
            put(createdThingBaseAddress() + "/unstar", data, successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to be unstarred by someone it is assigned to", function () {
            var successCallback = jasmine.createSpy();

            var data = { userId: createdThing.assignedTo[0].id };
            put(createdThingBaseAddress() + "/unstar", data, successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });
        
        it("should be able to be starred by the owner", function () {
            var successCallback = jasmine.createSpy();

            var data = { userId: createdThing.owner.id };
            put(createdThingBaseAddress() + "/star", data, successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });

        it("should be able to be starred by someone it is assigned to", function () {
            var successCallback = jasmine.createSpy();

            var data = { userId: createdThing.assignedTo[0].id };
            put(createdThingBaseAddress() + "/star", data, successCallback, null);

            waitsFor(function () {
                return successCallback.callCount > 0;
            });
            runs(function () {
                expect(successCallback).toHaveBeenCalled();
            });
        });          

        it("should not be able to be deleted by someone it is assigned to if they are not the owner", function () {
            var errorCallback = jasmine.createSpy();

            var data = { deletedById: createdThing.assignedTo[1].id };
            remove(createdThingBaseAddress(), data, null, errorCallback);

            waitsFor(function () {
                return errorCallback.callCount > 0;
            });
            runs(function () {
                expect(errorCallback).toHaveBeenCalled();
            });
        });

        it("should not be able to be deleted by someone that is not the owner", function () {
            var errorCallback = jasmine.createSpy();

            var data = { deletedById: 0/*0 is always an invalid userid*/ };
            remove(createdThingBaseAddress(), data, null, errorCallback);

            waitsFor(function () {
                return errorCallback.callCount > 0;
            });
            runs(function () {
                expect(errorCallback).toHaveBeenCalled();
            });
        });

        it("should be able to be deleted by its owner", function () {
            var successCallback = jasmine.createSpy();

            var data = { deletedById: createdThing.owner.id };
            remove(createdThingBaseAddress(), data, successCallback, null);

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