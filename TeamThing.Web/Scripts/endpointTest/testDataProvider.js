//Using Ajax.data would add this param to the message body on post events, so the most reliable way i found was just manually forcing this param to the query string
//Example uses the hacky way
$.ajaxPrefilter(function (options) {
     options.url = options.url + "?noAuth=true";
});

//Using tokens
//$.ajaxSetup({
//    'beforeSend': function (xhr) {
//        xhr.setRequestHeader("X-AuthProvider", "Google");
//        xhr.setRequestHeader("X-AuthToken", "ya29.AHES6ZQNJ6iHeuSSCV8lc6-0ZMoljXWJOJbeH9MvfXRIzIaMf2GCNzBL");
//    }
//})

function remove(url, data, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: "DELETE",
        data: JSON.stringify(data),
        contentType: "application/json;charset=utf-8"
    })
        .success(function (resource) {
            if (successCallback) {
                successCallback(resource);
            }
        })
        .error(function (result) {
            if (errorCallback) {
                errorCallback(result);
            }
        });
};

function create(url, data, successCallback) {
    $.ajax({
        url: url,
        data: JSON.stringify(data),
        type: "POST",
        contentType: "application/json;charset=utf-8"
    })
        .success(function (resource) {
            if (successCallback) {
                successCallback(resource);
            }
        });
};

function put(url, data, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: "PUT",
        data: JSON.stringify(data),
        contentType: "application/json;charset=utf-8" })
        .success(function (resource) {
            if (successCallback) {
                successCallback(resource);
            }
        })
        .error(function (result) {
            if (errorCallback) {
                errorCallback(result);
            }
        });
}

function get(url, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: "GET",
        contentType: "application/json;charset=utf-8"})
        .success(function (resource) {
            if (successCallback) {
                successCallback(resource);
            }
        })
        .error(function (result) {
            if (errorCallback) {
                errorCallback(result);
            }
        });
}