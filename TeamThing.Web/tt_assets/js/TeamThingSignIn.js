/*
|--------------------------------------------------------------------------
|	BEGIN: FUNCTION JUNCTION
|--------------------------------------------------------------------------
*/
function AdjustLayout(WindowWidth,WindowHeight) {
	
	NewWrapperMargin = (WindowHeight-WindowHeight)/2;
	
	NewCarouselWidth = WindowWidth;
	$('#signin-slides').css('width', (NewCarouselWidth*2)+'px');
	$('#signin-slides li').css('width', NewCarouselWidth+'px');
}

function JumpToMain(TeamID) {
	if (TeamID != '' && TeamID.length > 0 && !isNaN(TeamID))
	{
		location.href="main.html?teamid="+TeamID;
	}
}
/*
|--------------------------------------------------------------------------
|	END: FUNCTION JUNCTION
|--------------------------------------------------------------------------
*/

$(document).ready(function(){
	
/*
|--------------------------------------------------------------------------
|	BEGIN: INIT FUNCTIONS
|--------------------------------------------------------------------------
*/
	teamThing.init($("#teamThing"));
	
	AdjustLayout($(window).width(),$(window).height());
	
	$(window).resize(function(){
		AdjustLayout($(window).width(),$(window).height())
	});
	
	APPURL = 'http://teamthing.apphb.com';
	
	$.get(
		APPURL+'/api/user?$filter=EmailAddress ne null and tolower(EmailAddress) eq \'todd.anglin@telerik.com\'',
    	function(data) { 
			console.log('Test Script: ' + data);
		}
	);
/*
|--------------------------------------------------------------------------
|	END: INIT FUNCTIONS
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: GET ALL PUBLIC TEAMS FOR PULLDOWN MENU
|--------------------------------------------------------------------------
*/	
	$.get(
		APPURL+'/api/team?$filter=IsPublic%20eq%20true',
    	function(data) { 
			/*$('#jumpMenu').append(TeamsOutput, function() {
				$("#jumpMenu").kendoComboBox();
			});*/
			
			TeamsOutput = { TeamsList: [] };
			TeamsOutput.TeamsList.push({"TeamsListLabel":"Select a Team...","TeamsListValue":""});
			for(i=0;i<data.length;i++) {
				TeamsOutput.TeamsList.push({"TeamsListLabel":""+data[i].name+"","TeamsListValue":""+data[i].id+""});
			}
			
			console.log('Teams Listing 2: ' + TeamsOutput.TeamsList);
			
			$("#jumpMenu").kendoComboBox({
				dataTextField: "TeamsListLabel",
				dataValueField: "TeamsListValue",
				dataSource: TeamsOutput.TeamsList,
				index: 0
			});
		}
	);
/*
|--------------------------------------------------------------------------
|	END: GET ALL PUBLIC TEAMS FOR PULLDOWN MENU
|--------------------------------------------------------------------------
*/	
	
/*
|--------------------------------------------------------------------------
|	BEGIN: GOOGLE SIGN-IN
|--------------------------------------------------------------------------
*/
	$("#google-signin-btn").click(function(event) {
  		event.preventDefault();
		//connectUserWithGoogle(); // This will work when this page is harnessed with Kendo UI Team's current code.
		onClose();
	});
	
	onClose = function() {
		var ThisLeft = $('li#signin-slide-2').offset().left;
		$("#signin-slides").animate({ left: $('#signin-slides').offset().left + 30 }, { 'duration': 100, 'easing': 'linear' }).animate({ left: $('#signin-slides').offset().left - ThisLeft }, { 'duration': 1000, 'easing': 'easeInOutBack' });
	}
	/*

	var SignInWindow = $("#signin-window").kendoWindow({
        visible: false,
		width: "500px",
		height: "400px",
		title: "Sign In with Google",
		content: authUrl,
		iframe:true,
		modal:true,
		close: onClose
    }).data("kendoWindow");

	$("#google-signin-btn").click(function() {
    	var SignInWindow = $("#signin-window").data("kendoWindow");
		SignInWindow.center();
    	SignInWindow.open();
	});
	
	$.ajax({
  		url: authUrl,
  		success: function(data) {
			console.log(data);
		}
	});*/
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

	$("submit").click(function() {
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