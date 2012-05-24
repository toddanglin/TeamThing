function OAuthProvider() {

    var fbAuthBase = "https://www.facebook.com/dialog/oauth?client_id=YOUR_APP_ID &redirect_uri=https://www.facebook.com/connect/login_success.html&response_type=token";
    var googleAuthBase = "https://accounts.google.com/o/oauth2/auth?state=authHandled&response_type=token&client_id=&redirect_uri=http://localhost:5079/&scope=https://www.googleapis.com/auth/userinfo.profile+https://www.googleapis.com/auth/userinfo.email";

    var providers = {
        google: {
            name: 'Google',
            url: 'https://accounts.google.com/o/oauth2/auth',
            clientId: '1071592151045.apps.googleusercontent.com',
            scope: 'https://www.googleapis.com/auth/userinfo.profile+https://www.googleapis.com/auth/userinfo.email'
        },
        facebook: {
            name: 'Facebook',
            url: 'https://www.facebook.com/dialog/oauth',
            scope: 'email',
            clientId: '384951088223271'
        }
    };

    function doSignIn(provider) {
        location.href = buildAuthPath(provider);
    }

    function buildAuthPath(provider) {
        var path = provider.url;
        path += '?response_type=token';
        path += '&client_id=' + provider.clientId;
        path += '&scope=' + provider.scope;
        path += '&state=' + provider.name;
        path += '&redirect_uri=http://localhost:5079/';

        return path;
    }

    function signIn(providerName) {
        var provider;

        switch (providerName) {
            case "google":
                provider = providers.google;
                break;
            case "facebook":
                provider = providers.facebook;
                break;
        }

        if (provider !== undefined) {
            doSignIn(provider);
        }
        else {
            console.log("Invalid Auth Provider");
        }
    }

    function getStatus() {
        var hash = document.location.hash;
        if (hash.match(/state=(\w+)/) && hash.match(/access_token=(\w+)/)) {

            var accessToken = document.location.hash.match(/access_token=([^&]*)/);
            accessToken = !!accessToken && accessToken[1];

            var provider = document.location.hash.match(/state=([^&]*)/);
            provider = !!provider && provider[1];

            //clear hash
            document.location.hash = "";
            return {
                signedIn: true,
                provider: provider,
                accessToken: accessToken
            };
        }

        //TODO: check offline db / cookies?

        return {
            signedIn: false
        };
    }

    return {
        getStatus: getStatus,
        signIn: signIn
    };
}