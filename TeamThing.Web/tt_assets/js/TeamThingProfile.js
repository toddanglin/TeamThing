
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
	
	$('a.leave-team-btn').bind("click", function(event) {
		event.preventDefault();
		ThisTeamID = $(this).attr('rel');
		//console.log(ThisTeamID);
		LeaveTeam(ThisTeamID,LoggedInUserID);
	});
}

// ---- Get User's Profile for Main Content Area ---- //
function UserProfileMain(UserID) {                   
	$.get(
		APPURL+'/api/user/'+UserID,
    	function(UserInfo) {
			console.log(UserInfo);
			$('#userpic-profile img').attr('src',UserInfo.imagePath);
			$('#userinfo-profile').html(UserInfo.nickname);
			ThisUserInfo = UserInfo;
			GetMyTeams(LoggedInUserID,'');	
		}
	);
}
UserProfileMain(LoggedInUserID);
// ---- / Get User's Profile for Main Content Area ---- //

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
function GetTeamProperties(TeamID,TeamFilter) {
	////console.log('Getting Team ID: ' + TeamID + ' to put into ' + TeamID);
	$.ajax({
  		url: APPURL+'/api/team/'+TeamID+'/members/'+TeamFilter,
  		type: 'GET',
  		success: function(TeamData) {
			TeamPropertiesOutput = '';
			TeamPropertiesOutput = TeamData.length; // How many Users on a Team
			TeamPropertiesOutput == 1 ? TeamPropertiesOutput+=' member' :  TeamPropertiesOutput+=' members';
			$('a.users-count-link#user-count-'+TeamID).text(TeamPropertiesOutput);
			
					TeamMembersForTray = '';
					if(TeamData.length > 0) {
						TeamMembersForTray+='<div class="users-tray">';
						for(i=0;i<TeamData.length;i++) {
							TeamMembersForTray+='<div class="userpic">';
					
							ThisUserImg = ImageURIRemoteOrRelative(TeamData[i].imagePath);
					
							TeamMembersForTray+='<img src="'+ThisUserImg+'" width="55" height="55" alt="'+TeamData[i].emailAddress+'" title="'+TeamData[i].emailAddress+'" id="userpic='+TeamData[i].id+'">';
							TeamMembersForTray+='</div>';
						}
						TeamMembersForTray+='<div class="clear-float"></div></div>';
					}
					$('#teamthing-'+TeamID).append(TeamMembersForTray);
					$('.users-tray').hide();
					
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
|	BEGIN: GET ANY TEAM'S PROPERTIES
|--------------------------------------------------------------------------
*/
function GetPendingUsers(TeamID) {
	////console.log('Getting Team ID: ' + TeamID + ' to put into ' + TeamID);
	$.ajax({
  		url: APPURL+'/api/team/'+TeamID+'/members/pending',
  		type: 'GET',
  		success: function(PendingData) {
					
			TeamPropertiesOutput=''
			TeamPropertiesOutput+=', <a href=#">' + PendingData.length + ' Pending</a>'; // How many Pending Users on a Team
			$('a.users-count-link#user-count-'+TeamID).after(TeamPropertiesOutput);

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
					UserTeamsOutput+='<a class="users-count-link" id="user-count-'+TeamsData[i].id+'" href="#"></a>';
					
				UserTeamsOutput+='</span>';
				
				/* Phase 2
				if(TeamsData[i].ownerId == LoggedInUserID) {
					UserTeamsOutput+='<div class="team-admin-tools"><span class="icon_delete"><a title="Delete Team" href="#"></a></span></div>';
				} else {
					UserTeamsOutput+='<div class="team-admin-tools"><a class="leave-team-btn" href="#" rel="'+TeamsData[i].id+'"><img src="tt_assets/images/icon_x.png" alt="X"></a></div>'
				}*/
				
          		UserTeamsOutput+='</div>';
			
				GetTeamProperties(TeamsData[i].id,'Approved');
			
				if(TeamsData[i].ownerId == LoggedInUserID) {
					GetPendingUsers(TeamsData[i].id);
				}
			}
			
			$(MyTeamsListDiv).html('<div class="list-heading">Your Teams</div>'+UserTeamsOutput);
			
			ActivateListViewButtons();
			
		}
	);
}
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
		dataType: 'json',
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
|	BEGIN: CREATE A TEAM WINDOW
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
|	END: CREATE A TEAM WINDOW
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: LEAVE A TEAM
|--------------------------------------------------------------------------
*/
function LeaveTeam(TeamID,UserID) {
	
	$.ajax({
  		url: APPURL+'/api/team/'+TeamID+'/leave',
  		type: 'PUT',
		data: {'userId':UserID},
		dataType: 'json',
  		success: function(LeaveTeamData) {
			////console.log('User: ' + UserID + ' is leaving Team: ' + TeamID);
			////console.log(LeaveTeamData);
			location.reload();
  		}
	});
	
}
/*
|--------------------------------------------------------------------------
|	END: LEAVE A TEAM
|--------------------------------------------------------------------------
*/

});