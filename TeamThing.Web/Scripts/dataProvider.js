//TODO: can I leverage the datasource here?!
function DataProvider(failback) {
    var that = this;
    this.failback = failback


    //$.ajaxSetup({
    //    statusCode: {
    //        401: function (xhr) {

    //            var wwwAuthHeaderValue = xhr.getResponseHeader("WWW-Authenticate");

    //            //get the location paramater and call it in an ajax modal
    //            var re = /\w+\slocation="([/\.\?\=:&\w]+)"/i;
    //            var results = re.exec(wwwAuthHeaderValue);
    //            var authZServerUrl = results[1];

    //            alert("navigating to: " + authZServerUrl);
    //            var w = window.open(authZServerUrl+"&callback=teamThing.loadUser", "_blank", 'width=900,height=600,toolbar=no,menubar=no');

    //        }
    //    }
    //});

    function handleErrors(errors, failback) {
        if (failback == null) {
            that.failback(errors);
        }
        else {
            failback(errors);
        }
    }

    this.setAuthHeaders = function (authToken, authFailedCallback) {
        $.ajaxSetup({
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', "Bearer " + authToken);
                xhr.setRequestHeader('Accept', "application/json");
            },

            statusCode: {
                401: function (xhr) { 
                    if (authFailedCallback) {
                        authFailedCallback();
                    }
                }
            }
        });
    };

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

    return this;
}