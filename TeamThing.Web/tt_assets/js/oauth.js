var extractToken = function (hash) {
	var match = hash.match(/access_token=(\w+)/);
	return !!match && match[1];
};

var setting =
	{
		'host': "accounts.google.com/o/oauth2",
		'clientId': '1071592151045.apps.googleusercontent.com',
		'scope': 'https://www.googleapis.com/auth/userinfo.profile'
};

var authHost = "https://" + setting.host;
var resourceHost = "https://www.googleapis.com/oauth2/v1";

var endUserAuthorizationEndpoint = authHost + "/auth";

var token = extractToken(document.location.hash);

if (token) {
	$('div.authenticated').show();

	$('span.token').text(token);

	$.ajax({
		url: resourceHost + '/userinfo?access_token=' + token
//            , beforeSend: function (xhr) {
//                xhr.setRequestHeader('Authorization', "Bearer " + token);
//                xhr.setRequestHeader('Accept', "application/json");
//            }
    	, 
		success: function (response) {
			var container = $('span.user');
			if (response) {
				container.text(response.name);
			} else {
				container.text("An error occurred.");
			}
		}
	});
} else {
	$('div.authenticate').show();

	authUrl = endUserAuthorizationEndpoint +
	"?response_type=token" +
	"&client_id=" + setting.clientId +
	"&redirect_uri=" + window.location +
	"&scope=" + setting.scope;

                //$("a.connect").attr("href", authUrl);
}