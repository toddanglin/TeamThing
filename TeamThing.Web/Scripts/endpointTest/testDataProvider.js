
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
}