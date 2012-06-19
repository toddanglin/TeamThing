
LoggedInUserID = getQueryVariable('userid');
MyTeamsListDiv = '.myteamslist .list';

$(document).ready(function() {

/*
|--------------------------------------------------------------------------
|	BEGIN: INIT FUNCTIONS
|--------------------------------------------------------------------------
*/
function ActivateListViewButtons() {
	$('.users-tray').hide();
	
	$('a.users-count-link').bind("click", function(event) {
  		event.preventDefault();
		$(this).parent('.listitem').parent('.thing').children('.users-tray').slideToggle(250);
	});
	
// ---- Get User's Profile for Main Content Area ---- //
function UserProfileMain(UserID) {                   
	$.get(
		APPURL+'/api/user/'+UserID,
    	function(UserInfo) {
			console.log(UserInfo);
			$('#userpic-profile img').attr('src',UserInfo.imagePath);
			$('#userinfo-profile').html(UserInfo.emailAddress);
		}
	);
}
UserProfileMain(LoggedInUserID);
// ---- / Get User's Profile for Main Content Area ---- //
	
//UserProfile(LoggedInUserID); //TO DO: This number needs to be dynamic
/*
|--------------------------------------------------------------------------
|	END: GET LOGGED IN USER'S PROFILE DETAILS
|--------------------------------------------------------------------------
*/
}
/*
|--------------------------------------------------------------------------
|	END: INIT FUNCTIONS
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: GET ANY TEAM'S PROPERTIES
|--------------------------------------------------------------------------
*/
function GetTeamProperties(TeamID,TeamFilter,ThisDiv) {
	console.log('Getting Team ID: ' + TeamID);
	$.ajax({
  		url: APPURL+'/api/team/'+TeamID+'/members/'+TeamFilter,
  		type: 'GET',
  		success: function(TeamData) {
			console.log(TeamData);
				TeamPropertiesOutput = TeamData.length; // How many Users on a Team
				TeamPropertiesOutput == 1 ? MembersOutput = 'member' :  MembersOutput = 'members';
				$(ThisDiv).text(TeamPropertiesOutput + ' ' + MembersOutput);
			
  		}
	});
}
/*
|--------------------------------------------------------------------------
|	END: GET ANY TEAM'S PROPERTIES
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: GET A SPECIFIC USER'S TEAMS AND LIST THEM OUT
|--------------------------------------------------------------------------
*/
function GetMyTeams(UserID,TeamFilter) {
	
	SetUpLoadingAnim(MyTeamsListDiv);
	
	ThisQueryString = '';
	ThisQueryString = APPURL+'/api/user/'+UserID+'/teams/'+TeamFilter;
	
	$.get(
		ThisQueryString,
    	function(TeamsData) { 
			console.log(TeamsData);
			UserTeamsOutput = '';
			
			for(i=0;i<TeamsData.length;i++) {
				
			UserTeamsOutput+='<div class="thing" id="teamthing-'+TeamsData[i].id+'">';
          		UserTeamsOutput+='<div class="listpic"><img src="'+APPURL+'/'+TeamsData[i].imagePath+'" width="83" height="83" alt=""></div>';
                UserTeamsOutput+='<span class="listitem">';
            		UserTeamsOutput+='<div class="thingcontrols">';
                        
                    UserTeamsOutput+='</div>';
            		
					UserTeamsOutput+='<div class="teamdesc">'+TeamsData[i].name+'</div>';
					
					ThisThingAssignedToCount = GetTeamProperties(TeamsData[i].id,'assignedTo','#teamthing-'+TeamsData[i].id+' a.users-count-link');
					UserTeamsOutput+='<a class="users-count-link" href="#"></a>';
					
				UserTeamsOutput+='</span>';
                
				if(ThisThingAssignedToCount > 0) {
					UserTeamsOutput+='<div class="users-tray">';
                    	UserTeamsOutput+='<div class="clear-float"></div>';
                	UserTeamsOutput+='</div>';
				}
				
				if(TeamsData[i].ownerId == LoggedInUserID) {
					UserTeamsOutput+='<div class="team-admin-tools"><span class="icon_delete"><a title="Delete Team" href="#"></a></span></div>';
				} else {
					UserTeamsOutput+='<div class="team-admin-tools"><a class="leave-team-btn" href="#"><img src="tt_assets/images/icon_x.png" alt="X"></a></div>'
				}
				
          	UserTeamsOutput+='</div>';
			}
			
			$(MyTeamsListDiv).html('<div class="list-heading">Your Teams</div>'+UserTeamsOutput);
			
			ActivateListViewButtons();
			
		}
	);
}

GetMyTeams(getQueryVariable('userid'),'');
/*
|--------------------------------------------------------------------------
|	END: GET A SPECIFIC USER'S TEAMS AND LIST THEM OUT
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: CREATE A TEAM
|--------------------------------------------------------------------------
*/
function CreateTeam(TeamName,CreatedByID,IsPublic) {
	
	$.ajax({
  		url: APPURL+'/api/team',
  		type: 'POST',
		data: {
			'name':''+TeamName+'',
			'createdById':CreatedByID,
			'ispublic':IsPublic
		},
  		success: function(CreateTeamData) {
    		$("#createteamwindow").data("kendoWindow").close();
			location.reload();
  		}
	});
	
}

/*
|--------------------------------------------------------------------------
|	END: CREATE A TEAM
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: EDIT THING WINDOW
|--------------------------------------------------------------------------
*/
    CreateWindow = $("#createteamwindow").kendoWindow({
        height: "220px",
        title: "Add A Team",
        visible: false,
        width: "400px"
    }).data("kendoWindow");
	
	ValidateCreateWindow = $("#createteamwindow").kendoValidator().data("kendoValidator"),

    $("#createteambtn").click(function() {
		if (ValidateCreateWindow.validate()) {
			ThisTeamName = $('#teamnameinput').val();
			CreateTeam(ThisTeamName, LoggedInUserID, true);
		} else {
             //
		}
	});
	
	$('a#add-team-btn').bind("click", function(event) {
		event.preventDefault();
		CreateWindow = $("#createteamwindow").data("kendoWindow");
    	CreateWindow.center();
    	CreateWindow.open();
	});
/*
|--------------------------------------------------------------------------
|	END: EDIT THING WINDOW
|--------------------------------------------------------------------------
*/

});