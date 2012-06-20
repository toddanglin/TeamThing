/*
|--------------------------------------------------------------------------
|	BEGIN: FUNCTION JUNCTION
|--------------------------------------------------------------------------
*/
function AdjustLayout(WindowWidth, WindowHeight) {

    NewWrapperMargin = (WindowHeight - WindowHeight) / 2;

    NewCarouselWidth = WindowWidth;
    $('#signin-slides').css('width', (NewCarouselWidth * 2) + 'px');
    $('#signin-slides li').css('width', NewCarouselWidth + 'px');
}

function JumpToMain(TeamID) {
    if (TeamID != '' && TeamID.length > 0 && !isNaN(TeamID)) {
        location.href = "main.html?teamid=" + TeamID;
    }
}
/*
|--------------------------------------------------------------------------
|	END: FUNCTION JUNCTION
|--------------------------------------------------------------------------
*/

$(document).ready(function () {

    /*
    |--------------------------------------------------------------------------
    |	BEGIN: INIT FUNCTIONS
    |--------------------------------------------------------------------------
    */ 

    AdjustLayout($(window).width(), $(window).height());

    $(window).resize(function () {
        AdjustLayout($(window).width(), $(window).height())
    });

    APPURL = 'http://teamthing.net';


    var oAuthProvider = new OAuthProvider();

    //CHECK SIGN IN STATUS
    checkUserAuthStatus();

   /* $.get(
		APPURL + '/api/user?$filter=EmailAddress ne null and tolower(EmailAddress) eq \'todd.anglin@telerik.com\'',
    	function (data) {
    	    console.log('Test Script: ' + data);
    	}
	)*/;
    /*
    |--------------------------------------------------------------------------
    |	END: INIT FUNCTIONS
    |--------------------------------------------------------------------------
    */

    /*
    |--------------------------------------------------------------------------
    |	BEGIN: GOOGLE SIGN-IN
    |--------------------------------------------------------------------------
    */
    $("#google-signin-btn").click(function (event) {
        event.preventDefault();
        connectUserWithGoogle(); // This will work when this page is harnessed with Kendo UI Team's current code.
        //onClose();
    });

    function connectUserWithGoogle() {
        oAuthProvider.signIn("google", window.location.href);
    }

    //This pretty will kick off all team thing end point specific things once the user's authStatus.signedIn = true;
    function checkUserAuthStatus() {
        var authStatus = getAuthStatus();
        if (authStatus.signedIn) {
            authSuccess(authStatus);            
        }
    }

    function getAuthStatus() {

        //check local store for auth info
        var authStatus = getAuthStatusFromStore();
        

        //if no auth info found in local store, check the auth provider for status
        if(!authStatus)
            authStatus = oAuthProvider.getStatus();

        return authStatus;
    }


    function saveAuthStatusInStore(authStatus) {
        	//TODO: write auth status to local store
    }

    function getAuthStatusFromStore() {
        	//TODO: read auth status from local store
    }

    //this will be called once the back and forth happens between the team thing application, and the OAuthProvider.
    function authSuccess(authStatus) {

        //auth has succeeded, lets store it in local storage
        saveAuthStatusInStore(authStatus);

        //create a package to send to the team thing end point
        var userInfo = { Provider: authStatus.provider, AuthToken: authStatus.accessToken };

        //configure ajax settings for calling team thing end points (only required once per full page load)
        configureAjaxHeaders(authStatus);

        //call team thing oauth end point to get the team thing user
        $.ajax({
            url: window.APPURL + '/api/user/oauth',
            type: "POST",
            data: userInfo,
            dataType: "JSON"
        })
        .success(loginSuccess)
        .error(loginFail);
    }

    //These headers are how the team thing end points validate each request, without these headers all ajax calls will fails
    function configureAjaxHeaders(authStatus) {
        $.ajaxSetup({
            'beforeSend': function (xhr) {
                xhr.setRequestHeader("X-AuthProvider", authStatus.provider);
                xhr.setRequestHeader("X-AuthToken", authStatus.accessToken);
            }
        });
    }  

    function loginSuccess(user) {

        GetUsersTeams(user.id);
		var ThisLeft = $('li#signin-slide-2').offset().left;
        $("#signin-slides").animate({ left: $('#signin-slides').offset().left + 30 }, { 'duration': 100, 'easing': 'linear' }).animate({ left: $('#signin-slides').offset().left - ThisLeft }, { 'duration': 1000, 'easing': 'easeInOutBack' });
		
		console.log("Hi my name is " + user.email + " Auth, and login have been SUCCESSFUL :)");

        //call methods to load user info here!
       
    }

    function loginFail(result) {
        console.log("Login FAILED:" + JSON.stringify(result));
    }
    /*
    |--------------------------------------------------------------------------
    |	END: GOOGLE SIGN-IN
    |--------------------------------------------------------------------------
    */

    /*
    |--------------------------------------------------------------------------
    |	BEGIN: CREATE TEAM FUNCTIONS
    |--------------------------------------------------------------------------
    */

    var validator = $("#create-team").kendoValidator().data("kendoValidator"), status = $(".status");

    $("submit").click(function () {
        if (validator.validate()) {
            //status.text("Hooray! Your tickets has been booked!").addClass("valid");
        } else {
            //status.text("Oops! There is invalid data in the form.").addClass("invalid");
        }
    });

    /*
    |--------------------------------------------------------------------------
    |	END: CREATE TEAM FUNCTIONS
    |--------------------------------------------------------------------------
    */

});
