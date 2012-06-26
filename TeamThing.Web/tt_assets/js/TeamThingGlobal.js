
APPURL = 'http://teamthing.net';
LoggedInUserID = getQueryVariable('userid');
CurrentTeamURLID = getQueryVariable('teamid');

/*
|--------------------------------------------------------------------------
|	BEGIN: GENERIC LOADING SPINNER
|--------------------------------------------------------------------------
*/
function SetUpLoadingAnim(ParentDiv) {
	$(ParentDiv).append('<div id="loading-div"><img src="tt_assets/kendoui/styles/Silver/loading-image.gif"><div>');
	ParentDivHeight = $(ParentDiv).height();
	$(ParentDiv+' #loading-div').css('height', ParentDivHeight+'px');
}
/*
|--------------------------------------------------------------------------
|	END: GENERIC LOADING SPINNER
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: GENERIC GET URL VARIABLE
|--------------------------------------------------------------------------
*/
function getQueryVariable(variable){
	var query = window.location.search.substring(1);
	var vars = query.split("&");
	for (var i=0;i<vars.length;i++) {
		var pair = vars[i].split("=");
		if(pair[0] == variable){return pair[1];}
	}
	return(false);
}
/*
|--------------------------------------------------------------------------
|	END: GENERIC GET URL VARIABLE
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: IS IMAGE REFERENCE REMOTE OR RELATIVE
|--------------------------------------------------------------------------
*/
function ImageURIRemoteOrRelative(ThisImageURI) {
	console.log('HAS BAD PORT REFERNCE');
	console.log(ThisImageURI.indexOf("16167"));
	if(ThisImageURI.substring(0, 4) == 'http' || ThisImageURI.substring(0, 5) == 'https') {
		return ThisImageURI;
	} else if (ThisImageURI.indexOf("16167") >= 0) { // <-- Added to keep legacy image references from that port #16167 from breaking the page
		console.log('BAD IMAGE REFERENCE');
		return ThisImageURI.replace(':16167','');
	} else {
		return APPURL+ThisImageURI;
	}
}
/*
|--------------------------------------------------------------------------
|	END: IS IMAGE REFERENCE REMOTE OR RELATIVE
|--------------------------------------------------------------------------
*/

$(document).ready(function() {
	
/*
|--------------------------------------------------------------------------
|	BEGIN: GET LOGGED IN USER'S PROFILE DETAILS
|--------------------------------------------------------------------------
*/
function UserProfile(UserID) {                   
	$.get(
		APPURL+'/api/user/'+UserID,
    	function(UserInfo) {
			//console.log(UserInfo);
			$('#userpic img').attr('src',ImageURIRemoteOrRelative(UserInfo.imagePath));
			$('#userinfo').html(UserInfo.nickname+'<br /><span class="usernav"><a href="profile.html?userid='+LoggedInUserID+'&teamid='+CurrentTeamURLID+'">View Profile</a>&nbsp;|&nbsp;<a href="../">Sign Out</a></span>');
		}
	);
}
UserProfile(LoggedInUserID,CurrentTeamURLID);
/*
|--------------------------------------------------------------------------
|	END: GET LOGGED IN USER'S PROFILE DETAILS
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: GET ALL TEAMS FOR JUMP MENU
|--------------------------------------------------------------------------
*/
function GetUsersTeams(UserID) {
	$.get(
		APPURL+'/api/user/'+UserID+'/teams',
    	function(data) { 
			TeamsOutput = { TeamsList: [] };
			TeamsOutput.TeamsList.push({"TeamsListLabel":"Select a Team...","TeamsListValue":""});
			for(i=0;i<data.length;i++) {
				TeamsOutput.TeamsList.push({"TeamsListLabel":""+data[i].name+"","TeamsListValue":""+data[i].id+""});
			}
			$("#jumpMenu").kendoComboBox({
				dataTextField: "TeamsListLabel",
				dataValueField: "TeamsListValue",
				dataSource: TeamsOutput.TeamsList,
				index: 0
			});
			var TeamsListSelected = function(e) {
				var dataItem = e.item.index()+1;
				console.log(dataItem);
                ThisTeamID = $('#jumpMenu :nth-child('+dataItem+')').attr('value');
    			location.href = './main.html?userid='+UserID+'&teamid='+ThisTeamID;
			};
			$("#jumpMenu").data("kendoComboBox").bind("select", TeamsListSelected);
		}
	);
}
GetUsersTeams(LoggedInUserID);
/*
|--------------------------------------------------------------------------
|	END: GET ALL PUBLIC TEAMS FOR JUMP MENU
|--------------------------------------------------------------------------
*/

});