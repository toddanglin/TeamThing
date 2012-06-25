
MyThingsListDiv = '.mythingslist .list';
MyTeamThingsListDiv = '.myteamsthingslist .list';
MyTeamMembersPanel = '.myteam #myteam-members';

/*
|--------------------------------------------------------------------------
|	BEGIN: GET ALL TEAMS FOR CREATE THING PULLDOWN
|--------------------------------------------------------------------------
*/
function GetUsersTeamsForCreate(UserID,WhichSelector) {
	$.get(
		APPURL+'/api/user/'+UserID+'/teams',
    	function(data) { 
			TeamsOutput='<option></option>';
			for(i=0;i<data.length;i++) {
				TeamsOutput+='<option value="'+data[i].id+'">'+data[i].name+'</option>';
			}
			$('#'+WhichSelector).append(TeamsOutput);
			
			$('#'+WhichSelector).kendoDropDownList({
    			index: 0
			});
		}
	);
}
GetUsersTeamsForCreate(LoggedInUserID,'thingteamselect');
/*
|--------------------------------------------------------------------------
|	END: GET ALL TEAMS FOR CREATE THING PULLDOWN
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: GET ANY THING'S PROPERTIES
|--------------------------------------------------------------------------
*/
function GetThingProperties(ThingID,ThingFilter,ThisDiv) {
	$.ajax({
  		url: APPURL+'/api/thing/'+ThingID,
  		type: 'GET',
  		success: function(ThingData) {
			
			if(ThingFilter == 'assignedTo') {
 				////console.log(ThingData.assignedTo);
				ThingPropertiesOutput = ThingData.assignedTo.length; // How many Users are assigned to this Thing
				$(ThisDiv+' a.users-count').text(ThingPropertiesOutput);
				
				// ---- Populate A Thing's Members Tray ----/
				TeamMembersForTray = '';
				AssigneesArray = []; // Used later when we edit a Thing's description.  So we don't wipe out assignees and don't have to make another call to get that information.
				if(ThingData.assignedTo.length > 0) {
					for(i=0;i<ThingData.assignedTo.length;i++) {
						TeamMembersForTray+='<div class="userpic">';
						ThisUserImg = ImageURIRemoteOrRelative(ThingData.assignedTo[i].imagePath);
						TeamMembersForTray+='<img src="'+ThisUserImg+'" width="55" height="55" alt="'+ThingData.assignedTo[i].nickname+'" title="'+ThingData.assignedTo[i].nickname+'" id="userpic='+ThingData.assignedTo[i].id+'">';
						TeamMembersForTray+='</div>';
						AssigneesArray.push(ThingData.assignedTo[i].id);
					}
				}
				TeamMembersForTray+='<input class="hidden-assignees-array" type="hidden" name="AssignedToVals-'+ThingID+'" id="AssignedToVals-'+ThingID+'" value="'+AssigneesArray+'" />';
				$(ThisDiv + ' .users-tray').prepend(TeamMembersForTray);
				$('.users-tray').hide();
				// ----/ Populate A Thing's Members Tray ----/
			}
			
			if(ThingFilter == '') {
				ThingPropertiesOutput = ThingData;
			}
			
  		}
	});
}
/*
|--------------------------------------------------------------------------
|	END: GET ANY THING'S PROPERTIES
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: STAR A THING
|--------------------------------------------------------------------------
*/
function StarThing(ThingID,NewStatus) {
	console.log('User: ' + LoggedInUserID + ' is trying to ' + NewStatus + ' a thing');
	$.ajax({
  		url: APPURL+'/api/thing/'+ThingID+'/'+NewStatus,
  		type: 'PUT',
		data: {
			'userId': LoggedInUserID
		},
		dataType: 'json',
  		success: function(StarThingData) {
    		console.log(StarThingData);
  		}
	});
}
/*
|--------------------------------------------------------------------------
|	END: STAR A THING
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: MAKE SIDEBAR TEAM MEMBERS DRAGGABLE
|--------------------------------------------------------------------------
*/
function SidebarMembersDraggable() {
	$('.member .userpic').bind('mouseenter', function(event) {
		event.preventDefault();
						
		$(this).kendoDraggable({
			hint: function(e) {
				return $(e).clone();
			}
		});
	});
	
	$('.member .userpic').bind('mousedown', function(event) {
		ThisMemberDivID = $(this).attr('data-id');
	});
	
	$(".userpic-dropzone").kendoDropTarget({
    	drop: UserDragDropped
  	});	
}

function UserDragDropped(e){
    DropTargetDataID = $(e.sender.element.context).data("id");
	DropTargetID = $(e.sender.element).attr("id");
	$('#'+DropTargetID).addClass('processing');
   	////console.log(DropTargetDataID, ThisMemberDivID);
	AddMemberToTeam(CurrentTeamURLID,DropTargetDataID,ThisMemberDivID);
}
/*
|--------------------------------------------------------------------------
|	END: MAKE SIDEBAR TEAM MEMBERS DRAGGABLE
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: ASSIGN USER TO THING ENDPOINT AND RESULTS
|--------------------------------------------------------------------------
*/
function AddMemberToTeam(ThingTeamID,AddToThingID,AddMemberID) {
	$.ajax({
  		url: APPURL+'/api/thing/'+AddToThingID,
  		type: 'GET',
  		success: function(ThisTeamData) {
			//console.log('GET A THING');
			//console.log(ThisTeamData);
			
			ThisThingAssignedTo = ThisTeamData.assignedTo;
			ThisThingAssignedToArray = [];
			
			for(i=0;i<ThisThingAssignedTo.length;i++) {
				ThisThingAssignedToArray.push(ThisThingAssignedTo[i].id);
			}
			
			if($.inArray(AddMemberID, ThisThingAssignedToArray) < 0) {
				ThisThingAssignedToArray.push(AddMemberID);
			}
			
			ThisThing = $('#dropzone-'+AddToThingID).parent('.users-tray').parent('.thing').parent('.list').parent('div'); // traversing the divs
			ThisThingID = ThisThing.attr('id');
			//console.log(ThisThingID);
			ThisUpdatedDescription = $('#'+ThisThingID+' .list .thing#teamthing-'+AddToThingID+ ' .listitem .thingdesc').text();
			//console.log(ThisUpdatedDescription);
    		
			$.ajax({
  				url: APPURL+'/api/thing/'+AddToThingID,
  				type: 'PUT',
				data: {
					'editedById': LoggedInUserID,
					'description': ThisUpdatedDescription,
					'assignedTo': ThisThingAssignedToArray
				},
				dataType: 'json',
  				success: function(AddMemberToTeamData) {
    				//console.log(AddMemberToTeamData);
					UpdateThingTray(AddMemberToTeamData,AddToThingID);
				}
			});
			
		}
	});
}

function UpdateThingTray(UpdatedMembersData,AddToThingID) {
	
	TeamMembersForTray = '';
	TeamMembersAssignedTo = UpdatedMembersData.assignedTo;
	AssigneesArray = []; // Used later when we edit a Thing's description.  So we don't wipe out assignees and don't have to make another call to get that information.
	
	for(i=0;i<TeamMembersAssignedTo.length;i++) {
		TeamMembersForTray+='<div class="userpic">';
		ThisUserImg = ImageURIRemoteOrRelative(TeamMembersAssignedTo[i].imagePath);
		TeamMembersForTray+='<img src="'+ThisUserImg+'" width="55" height="55" alt="'+TeamMembersAssignedTo[i].nickname+'" title="'+TeamMembersAssignedTo[i].nickname+'" id="userpic='+TeamMembersAssignedTo[i].id+'">';
		TeamMembersForTray+='</div>';
		AssigneesArray.push(TeamMembersAssignedTo[i].id);
	}
	
	TeamMembersForTray+='<input class="hidden-assignees-array" type="hidden" name="AssignedToVals-'+AddToThingID+'" id="AssignedToVals-'+AddToThingID+'" value="'+AssigneesArray+'" />';
	
	$('#teamthing-' + AddToThingID + ' .users-tray .userpic').remove();
	$('#teamthing-' + AddToThingID + ' .users-tray .hidden-assignees-array').remove();
	$('#teamthing-' + AddToThingID + ' .users-tray').prepend(TeamMembersForTray);
	$('#teamthing-' + AddToThingID + ' .users-count').html(TeamMembersAssignedTo.length);
	$('#'+DropTargetID).removeClass('processing');
}
/*
|--------------------------------------------------------------------------
|	BEGIN: ASSIGN USER TO THING ENDPOINT AND RESULTS
|--------------------------------------------------------------------------
*/


$(document).ready(function() {

/*
|--------------------------------------------------------------------------
|	BEGIN: INIT FUNCTIONS
|--------------------------------------------------------------------------
*/
$("#tabstrip").kendoTabStrip({
	animation:	{
		open: {
			effects: "fadeIn"
		}
	}
});

$("#statusfilterlist").kendoDropDownList({
    index: 0,
	change: StatusListSelected
});

function StatusListSelected() {
	ThisFilter = $('#statusfilterlist').val();
	
	if(ThisFilter != 'undefined' && ThisFilter != null) {
		GetThingsListings(LoggedInUserID,ThisFilter);
		//GetTeamThings(CurrentTeamURLID,ThisFilter);
	}
	
};
/*
|--------------------------------------------------------------------------
|	END: INIT FUNCTIONS
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: ACTIVATE ALL THING INTERACTIONS (TRIGGERED AFTER THINGS ARE LOADED VIA API)
|--------------------------------------------------------------------------
*/
function ActivateListViewButtons() {
	$('.users-tray').hide();
	
	// ---- Indicate Starred Things ---- //
	$.ajax({
  		url: APPURL+'/api/user/'+LoggedInUserID+'/starred?teamid='+CurrentTeamURLID,
  		type: 'GET',
  		success: function(StarredThingsData) {
			//console.log('MY STARRED THINGS');
			//console.log(StarredThingsData);
			MyStarredThingsArray = [];
			for(i=0;i<StarredThingsData.length;i++) {
				MyStarredThingsArray.push(''+StarredThingsData[i].id+'');
			}
			
			//console.log(MyStarredThingsArray);
			
			$('.list .thing').each(function(index) {
				ThisThingElementID = $(this).attr('id');
				ThisThingID = ThisThingElementID.replace('teamthing-','');
				//console.log($.inArray(ThisThingID, MyStarredThingsArray));
				if($.inArray(ThisThingID, MyStarredThingsArray) >= 0) {
					$('#'+ThisThingElementID+ ' .listitem .thingcontrols .star').addClass('active');
				}
			});
		}
		
	});
	// ----/ Indicate Starred Things ---- //
	
	// ---- Open/Close Assigned Users Tray ---- //
	$('a.users-count').bind("click", function(event) {
  		event.preventDefault();
		$(this).parent('.thing').children('.users-tray').slideToggle(250);
	});
	// ----/ Open/Close Assigned Users Tray ---- //
	
	// ---- Star A Thing ---- //
	$('a.star').bind("click", function(event) {
  		event.preventDefault();
		
		ThisThingID = $(this).parent('.thingcontrols').parent('.listitem').parent('.thing').attr('id'); // traversing the divs
		ThisThingID = ThisThingID.replace('teamthing-','');
		
		if($(this).hasClass('active')) {
			$(this).removeClass('active');
			StarThing(ThisThingID,'unstar');
			$('#teamthing-'+ThisThingID+ ' .listitem .thingcontrols .star').removeClass('active');
		} else {
			$(this).addClass('active');
			StarThing(ThisThingID,'star');
			$('#teamthing-'+ThisThingID+ ' .listitem .thingcontrols .star').addClass('active');
		}

	});
	// ----/ Star A Thing ---- //
	
	// ---- Change Thing Status ---- //
	$(".thingstatus").bind("click", function(event) {
		
		ThisThingID = $(this).parent('.controls').parent('.thingcontrols').parent('.listitem').parent('.thing').attr('id'); // traversing the divs
		////console.log(ThisThingID);
		ThisThingID = ThisThingID.replace('teamthing-','');
		
		if($(this).is(":checked")) {
			UpdateThing(ThisThingID,'Delayed');
		} else {
			UpdateThing(ThisThingID,'InProgress');
		}
	});
	// ----/ Change Thing Status ---- //
	
	// ---- Complete A Thing ---- //
	$('.icon_complete a').bind("click", function(event) {
		event.preventDefault();
		
		ThisThing = $(this).parent('.icon_complete').parent('.iconcontrols').parent('.controls').parent('.thingcontrols').parent('.listitem').parent('.thing'); // traversing the divs
		ThisThingID = ThisThing.attr('id');
		
		////console.log(ThisThingID);
		ThisThingID = ThisThingID.replace('teamthing-','');
		
		UpdateThing(ThisThingID,'Completed');
		
		ThisThing.fadeOut(750);

	});
	// ----/ Complete A Thing ---- //
	
	// ---- Delete A Thing ---- //
	$('.icon_delete a').bind("click", function(event) {
		event.preventDefault();
		
		ThisThing = $(this).parent('.icon_delete').parent('.iconcontrols').parent('.controls').parent('.thingcontrols').parent('.listitem').parent('.thing'); // traversing the divs
		ThisThingID = ThisThing.attr('id');
		
		////console.log(ThisThingID);
		ThisThingID = ThisThingID.replace('teamthing-','');
		
		UpdateThing(ThisThingID,'Deleted');
		
		ThisThing.fadeOut(750);

	});
	// ----/ Delete A Thing ---- //
	
	// ---- Edit A Thing ---- //
	$('.icon_edit a').bind("click", function(event) {
		event.preventDefault();
		
		ThisThing = $(this).parent('.icon_edit').parent('.iconcontrols').parent('.controls').parent('.thingcontrols').parent('.listitem').parent('.thing'); // traversing the divs
		ThisThingID = ThisThing.attr('id');
		
		CurrentThingEditID = ThisThingID.replace('teamthing-','');
		ThingAssignees = $('#'+ThisThingID+' #AssignedToVals-'+CurrentThingEditID).val().split(',');
		//alert(ThingAssignees);
		
		CurrentDesc = '';
		CurrentDesc = $('#'+ThisThingID+' .thingdesc').html();
		$('#thingdescriptionedit').val('');
		$('#thingdescriptionedit').val(CurrentDesc);
		
    	var EditWindow = $("#editthinginfo").data("kendoWindow");
    	EditWindow.center();
    	EditWindow.open();
	});
	// ----/ Edit A Thing ---- //
	
	$('#loading-div').remove();
}
/*
|--------------------------------------------------------------------------
|	END: ACTIVATE ALL THING INTERACTIONS (TRIGGERED AFTER THINGS ARE LOADED VIA API)
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: UPDATE A THING'S STATUS
|--------------------------------------------------------------------------
*/
function UpdateThing(ThingID,NewStatus) {
	////console.log('Updating ID: ' + ThingID + ' with Status: ' + NewStatus);
	$.ajax({
  		url: APPURL+'/api/thing/'+ThingID+'/updatestatus',
  		type: 'PUT',
		data: {
			"UserId": LoggedInUserID,
			'Status': NewStatus
		},
		dataType: 'json',
  		success: function(TeamThingsData) {
    		GetThingsListings(LoggedInUserID,'');
  		}
	});
	
}
/*
|--------------------------------------------------------------------------
|	END: UPDATE A THING'S STATUS
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: GET THINGS LISTINGS (PROCEDURALLY FROM LEFT-TO-RIGHT
|--------------------------------------------------------------------------
*/
function GetThingsListings(UserID,ThingsFilter) {
	
	// ---- Get My Things ---- //
	$(MyThingsListDiv).html('');
	SetUpLoadingAnim(MyThingsListDiv);
	
	ThisQueryString = '';
	ThisQueryString = APPURL+'/api/team/'+CurrentTeamURLID+'/things/'+ThingsFilter;
	
	$.get(
		ThisQueryString,
    	function(TeamThingsData) { 
			//console.log('MY THINGS');
			//console.log(TeamThingsData);
			TeamThingsOutput = '';
			for(i=0;i<TeamThingsData.length;i++) {
			
			//console.log(TeamThingsData[0].assignedTo);
			
			if($.inArray(UserID, TeamThingsData[i].assignedTo) >= 0 || TeamThingsData[i].owner.id == UserID) {
				
				TeamThingsOutput+='<div class="thing" id="teamthing-'+TeamThingsData[i].id+'">';
          		TeamThingsOutput+='<div class="listpic"><img src="tt_assets/images/listpic.png" width="83" height="83" alt=""></div>';
                TeamThingsOutput+='<span class="listitem">';
            		TeamThingsOutput+='<div class="thingcontrols">';
						TeamThingsOutput+='<a class="star" href="#"></a>';						
                        TeamThingsOutput+='<span class="controls">';
						
						if(TeamThingsData[i].owner.id == UserID) {
							if(TeamThingsData[i].status != 'Completed') {
							
								if(TeamThingsData[i].status == 'Delayed') {
             						TeamThingsOutput+='<input class="thingstatus" type="checkbox" data-icon1="In Progress" data-icon2="Delayed" checked="checked" />';
								} else if(TeamThingsData[i].status == 'InProgress') {
									TeamThingsOutput+='<input class="thingstatus" type="checkbox" data-icon1="In Progress" data-icon2="Delayed" />';
								}
							
							} else {
								TeamThingsOutput+='<div class="completed-status">'+TeamThingsData[i].status+'</div>';
							}
						} else {
							TeamThingsOutput+='<div class="completed-status">'+TeamThingsData[i].status+'</div>';
						}
						
              				TeamThingsOutput+='<br />';
              				TeamThingsOutput+='<span class="iconcontrols">';
              					if(TeamThingsData[i].status != 'Completed') {
									TeamThingsOutput+='<div class="icon_complete"><a href="#"></a></div>';
								}
								TeamThingsOutput+='<div class="icon_share"><a href="#"></a></div>';
								TeamThingsOutput+='<div class="icon_edit"><a href="#"></a></div>';
								if(TeamThingsData[i].status != 'Deleted') {
									TeamThingsOutput+='<div class="icon_delete"><a href="#"></a></div>';
								}
              				TeamThingsOutput+='</span>';
                       TeamThingsOutput+='</span>';
                    TeamThingsOutput+='</div>';
            		TeamThingsOutput+='<div class="thingdesc">'+TeamThingsData[i].description+'</div>';
				TeamThingsOutput+='</span>';
                TeamThingsOutput+='<div class="users-tray">';					
                    TeamThingsOutput+='<div class="userpic-dropzone" id="dropzone-'+TeamThingsData[i].id+'" data-id="'+TeamThingsData[i].id+'"></div>';
                    TeamThingsOutput+='<div class="clear-float"></div>';
                TeamThingsOutput+='</div>';
				
                TeamThingsOutput+='<a class="users-count" href="#"></a>';
          	TeamThingsOutput+='</div>';
			
			GetThingProperties(TeamThingsData[i].id,'assignedTo',MyThingsListDiv+' #teamthing-'+TeamThingsData[i].id);
			//GetTeamProperties(CurrentTeamURLID,'');
			
			}}
			
			$(MyThingsListDiv).html(TeamThingsOutput);
			// ----/ Get My Things ---- //
				
			// ---- Then Get My Team's Things ---- //
			$(MyTeamThingsListDiv).html('');
			SetUpLoadingAnim(MyTeamThingsListDiv);
			
			ThisQueryString = '';
			ThisQueryString = APPURL+'/api/team/'+CurrentTeamURLID+'/things/'+ThingsFilter;
	
			$.get(
				ThisQueryString,
    			function(TeamThingsData) { 
					//console.log('MY TEAM THINGS');
					//console.log(TeamThingsData);
					TeamThingsOutput = '';
					for(i=0;i<TeamThingsData.length;i++) {
				
						TeamThingsOutput+='<div class="thing" id="teamthing-'+TeamThingsData[i].id+'">';
          				TeamThingsOutput+='<div class="listpic"><img src="tt_assets/images/listpic.png" width="83" height="83" alt=""></div>';
                		TeamThingsOutput+='<span class="listitem">';
            				TeamThingsOutput+='<div class="thingcontrols">';
							TeamThingsOutput+='<a class="star" href="#"></a>';
                       		 TeamThingsOutput+='<span class="controls">';
								
								
								if(TeamThingsData[i].owner.id == UserID) {
									if(TeamThingsData[i].status != 'Completed') {
										if(TeamThingsData[i].status == 'Delayed') {
             								TeamThingsOutput+='<input class="thingstatus" type="checkbox" data-icon1="In Progress" data-icon2="Delayed" checked="checked" />';
										} else if(TeamThingsData[i].status == 'InProgress') {
											TeamThingsOutput+='<input class="thingstatus" type="checkbox" data-icon1="In Progress" data-icon2="Delayed" />';
										}
									} else {
										TeamThingsOutput+='<div class="completed-status">'+TeamThingsData[i].status+'</div>';
									}
								} else {
									TeamThingsOutput+='<div class="completed-status">'+TeamThingsData[i].status+'</div>';
								}
								
              						TeamThingsOutput+='<br />';
              						TeamThingsOutput+='<span class="iconcontrols">';
										if(TeamThingsData[i].status != 'Completed' && TeamThingsData[i].owner.id == LoggedInUserID) {
											TeamThingsOutput+='<div class="icon_complete"><a href="#"></a></div>';
										}
								
										TeamThingsOutput+='<div class="icon_share"><a href="#"></a></div>';
								
										if($.inArray(UserID, TeamThingsData[i].assignedTo) >= 0 || TeamThingsData[i].owner.id == UserID) {
											TeamThingsOutput+='<div class="icon_edit"><a href="#"></a></div>';
										}
								
										if(TeamThingsData[i].status != 'Deleted' && TeamThingsData[i].owner.id == LoggedInUserID) {
											TeamThingsOutput+='<div class="icon_delete"><a href="#"></a></div>';
										}
								
              						TeamThingsOutput+='</span>';
                       		TeamThingsOutput+='</span>';
                   		 TeamThingsOutput+='</div>';
            				TeamThingsOutput+='<div class="thingdesc">'+TeamThingsData[i].description+'</div>';
						TeamThingsOutput+='</span>';
               			TeamThingsOutput+='<div class="users-tray">';
						if($.inArray(UserID, TeamThingsData[i].assignedTo) >= 0 || TeamThingsData[i].owner.id == UserID) {				
                    		TeamThingsOutput+='<div class="userpic-dropzone" id="dropzone-'+TeamThingsData[i].id+'" data-id="'+TeamThingsData[i].id+'"></div>';
						}
                    	TeamThingsOutput+='<div class="clear-float"></div>';
                		TeamThingsOutput+='</div>';
				
                		TeamThingsOutput+='<a class="users-count" href="#"></a>';
          			TeamThingsOutput+='</div>';
			
					GetThingProperties(TeamThingsData[i].id,'assignedTo',MyTeamThingsListDiv+' #teamthing-'+TeamThingsData[i].id);
					//GetTeamProperties(CurrentTeamURLID,'');
			
					}
			
					$(MyTeamThingsListDiv).html(TeamThingsOutput);
			// ----/ Then Get My Team's Things ---- //
			
			// ---- Set Up Team Members In Sidebar ---- //
					GetSideBarTeamMembers(CurrentTeamURLID,'');
			// ----/ Set Up Team Members In Sidebar ---- //
			
			// ---- Trigger All Functional Thing Buttons, Now That They Are Rendered ---- //
					ActivateListViewButtons();
			// ----/ Trigger All Functional Thing Buttons, Now That They Are Rendered ---- //
			
				}
			);

		}
	);
}

GetThingsListings(LoggedInUserID,'');
/*
|--------------------------------------------------------------------------
|	END: GET THINGS LISTINGS (PROCEDURALLY FROM LEFT-TO-RIGHT
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: TEAM MEMBERS FOR SIDEBAR PANEL
|--------------------------------------------------------------------------
*/
function GetSideBarTeamMembers(TeamID,TeamMembersFilter) {
	
	$.ajax({
  		url: APPURL+'/api/team/'+TeamID,
  		type: 'GET',
  		success: function(TeamData) {
			$('.sidebar-header').text(TeamData.name);
			
  			ThisQueryString = '';
			ThisQueryString = APPURL+'/api/team/'+TeamID+'/members/'+TeamMembersFilter;
	
			$.get(
				ThisQueryString,
    			function(TeamMembersData) { 
					////console.log(TeamMembersData);
			
					TeamMembersDataOutput = '';
					CurrentTeamMembersArray = [];
					DraggableOutput = [];
			
					for(i=0;i<TeamMembersData.length;i++) {
						
						ThisUserImg = ImageURIRemoteOrRelative(TeamMembersData[i].imagePath);
						
						TeamMembersDataOutput+='<div class="member"><div class="userpic" data-id="'+TeamMembersData[i].id+'" rel="'+TeamMembersData[i].id+'">';
						TeamMembersDataOutput+='<img src="'+ThisUserImg+'" width="55" height="55" alt="">';
						TeamMembersDataOutput+='</div>';
						TeamMembersDataOutput+=TeamMembersData[i].emailAddress+'</div>';
						
						CurrentTeamMembersArray.push(TeamMembersData[i].id);
				
					}
					TeamMembersDataOutput+='<div class="clear-float"></div>';
			
					$(MyTeamMembersPanel).html(TeamMembersDataOutput);
					$('.sidebar-header').html();
					
					SidebarMembersDraggable();
	
					GetAllUsers('');

				}
			);
		}
	});
}
/*
|--------------------------------------------------------------------------
|	END: TEAM MEMBERS FOR SIDEBAR PANEL
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: CREATE A THING
|--------------------------------------------------------------------------
*/
function CreateThing(CreatedById,Description,AssignedTo,teamId) {
	
	$.ajax({
  		url: APPURL+'/api/thing',
  		type: 'POST',
		data: {
			'CreatedById': CreatedById,
			'Description': Description,
			'AssignedTo': AssignedTo,
			'teamId': teamId
		},
  		success: function(CreateThingData) {
			GetThingsListings(LoggedInUserID,'');
			$("#createthinginfo").data("kendoWindow").close();
			//location.reload();
  		}
	});
	
}
/*
|--------------------------------------------------------------------------
|	END: CREATE A THING
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: EDIT A THING
|--------------------------------------------------------------------------
*/
function EditThing(editedById,Description,assignedTo) {
	console.log('Assignees: ' + ThingAssignees);
	$.ajax({
  		url: APPURL+'/api/thing/'+CurrentThingEditID,
  		type: 'PUT',
		data: {
			'editedById': editedById,
			'description': Description,
			'assignedTo': ThingAssignees
		},
		dataType: 'json',
  		success: function(ThingData) {
    		////console.log(ThingData);
			CurrentThingEditID = null;
			GetThingsListings(LoggedInUserID,'');
			$("#editthinginfo").data("kendoWindow").close();
			//location.reload();
  		}
	});
}
/*
|--------------------------------------------------------------------------
|	BEGIN: EDIT A THING
|--------------------------------------------------------------------------
*/   

/*
|--------------------------------------------------------------------------
|	BEGIN: CREATE THING WINDOW
|--------------------------------------------------------------------------
*/
    var CreateThingWindow = $("#createthinginfo").kendoWindow({
        height: "220px",
        title: "Create a Thing",
        visible: false,
        width: "400px"
    }).data("kendoWindow");

	$("#creatething").click(function(){
    	var CreateThingWindow = $("#createthinginfo").data("kendoWindow");
    	CreateThingWindow.center();
    	CreateThingWindow.open();
	});
	
	ValidateCreateThing = $("#createthinginfo").kendoValidator().data("kendoValidator"),

    $("#createthingbtn").click(function() {
		if (ValidateCreateThing.validate()) {
			CreateThing(LoggedInUserID, $('#thingdescription').val(), LoggedInUserID, $('#thingteamselect').attr('value'));
		} else {
             //
		}
	});
/*
|--------------------------------------------------------------------------
|	END: CREATE THING WINDOW
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: EDIT THING WINDOW
|--------------------------------------------------------------------------
*/
    var EditWindow = $("#editthinginfo").kendoWindow({
        height: "220px",
        title: "Edit Thing",
        visible: false,
        width: "400px"
    }).data("kendoWindow");
	
	ValidateEditThing = $("#editthinginfo").kendoValidator().data("kendoValidator"),

    $("#updatethingbtn").click(function() {
		if (ValidateEditThing.validate()) {
			ThisUpdatedDescription = $('#thingdescriptionedit').val();
			EditThing(LoggedInUserID,ThisUpdatedDescription,ThingAssignees);
		} else {
             //
		}
	});
/*
|--------------------------------------------------------------------------
|	END: EDIT THING WINDOW
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: RENDER ALL USERS TO SIDEBAR POP UP WINDOW
|--------------------------------------------------------------------------
*/
function GetAllUsers(UserFilter) {
	
	$.ajax({
  		url: APPURL+'/api/user/'+UserFilter,
  		type: 'GET',
  		success: function(UsersData) {
			AllUsersOutput = '';
			for(i=0;i<UsersData.length;i++) {
				
				////console.log('raw img: ' + UsersData[i].imagePath);
				ThisUserImg = ImageURIRemoteOrRelative(UsersData[i].imagePath);
				
				if($.inArray(UsersData[i].id, CurrentTeamMembersArray) < 0) {
					AllUsersOutput+='<a href="#" class="memberlistitem" rel="'+UsersData[i].id+'"><span class="imgwrap"><img src="'+ThisUserImg+'" width="32" height="32"></span>'+UsersData[i].nickname+'</a>';
				}
			}
			
			$('#addtoteam').append(AllUsersOutput);
			
			$('a.memberlistitem').bind("click", function(event) {
  				event.preventDefault();
				ThisUserID = $(this).attr('rel');
				
				////console.log('Tried adding User ID: ' + ThisUserID + ' to Team ID: ' + CurrentTeamURLID);
				
				$.ajax({
  					url: APPURL+'/api/team/'+CurrentTeamURLID+'/join',
  					type: 'PUT',
					data: {
						'userId': ThisUserID
					},
					dataType: 'json',
  					success: function(AddUserToTeamData) {
						GetThingsListings(LoggedInUserID,'');
						$("#addtoteam").data("kendoWindow").close();
						//location.reload();
  					}
				});
				
			});
  		}
	});
	
}
/*
|--------------------------------------------------------------------------
|	BEGIN: RENDER ALL USERS TO SIDEBAR POP UP WINDOW
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: INVITE A USER
|--------------------------------------------------------------------------
*/
function InviteUser(UserEmail,TeamID,CreatedByID) {
	console.log(UserEmail+','+TeamID+','+CreatedByID);
	$.ajax({
  		url: APPURL+'/api/team/'+TeamID+'/addmember',
  		type: 'PUT',
		data: {
			'addedByUserId':CreatedByID,
			'emailAddress':UserEmail
		},
		dataType: 'json',
  		success: function(CreateTeamData) {
			GetThingsListings(LoggedInUserID,'');
    		$("#addtoteam").data("kendoWindow").close();
			//location.reload();
  		}
	});
	
}
/*
|--------------------------------------------------------------------------
|	END: INVITE A USER
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: ADD TO TEAM WINDOW AND FUNCTIONS
|--------------------------------------------------------------------------
*/

    AddUserToTeamWindow = $("#addtoteam").kendoWindow({
        height: "400px",
        title: "Add A User To My Team",
        visible: false,
        width: "380px"
    }).data("kendoWindow");


	$(".plus").click(function(){
		AddUserToTeamWindow = $("#addtoteam").data("kendoWindow");
    	AddUserToTeamWindow.center();
    	AddUserToTeamWindow.open();
	});
	
	ValidateCreateWindow = $("#addtoteam").kendoValidator().data("kendoValidator"),

    $("#inviteuserbtn").click(function() {
		if (ValidateCreateWindow.validate()) {
			ThisUserEmail = $('#useremailinput').val();
			ThisUserTeamAssignment = $('#userteamselect').val();
			InviteUser(ThisUserEmail, ThisUserTeamAssignment, LoggedInUserID);
		} else {
             //
		}
	});
	
	GetUsersTeamsForCreate(LoggedInUserID,'userteamselect'); // TeamsOutput contains all the Teams we gathered onLoad
/*
|--------------------------------------------------------------------------
|	END: ADD TO TEAM WINDOW AND FUNCTIONS
|--------------------------------------------------------------------------
*/  

/*
|--------------------------------------------------------------------------
|	BEGIN: KEEP SIDEBAR TEAM MEMBERS IN VIEW WHEN SCROLLING
|--------------------------------------------------------------------------
*/ 
	$sidebar   = $("#myteam-teammembers"), 
	$window    = $(window),
	offset     = $sidebar.offset(),
	topPadding = 15;
	
	//console.log('Offset: ' + offset);
    $window.scroll(function() {
		//console.log($window.scrollTop() + ' and ' + offset.top);
        if ($window.scrollTop() > offset.top) {
            $sidebar.stop().animate({
                marginTop: $window.scrollTop() - $window.scrollTop()
            });
        } else {
            $sidebar.stop().animate({
                marginTop: 0
            });
        }
    });
/*
|--------------------------------------------------------------------------
|	BEGIN: KEEP SIDEBAR TEAM MEMBERS IN VIEW WHEN SCROLLING
|--------------------------------------------------------------------------
*/ 

$('.displaylogo').parent('a').attr('href','main.html?userid='+LoggedInUserID+'&teamid='+CurrentTeamURLID);

});