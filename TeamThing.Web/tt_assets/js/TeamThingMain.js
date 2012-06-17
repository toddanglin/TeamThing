/*
|--------------------------------------------------------------------------
|	BEGIN: FUNCTION JUNCTION
|--------------------------------------------------------------------------
*/
function JumpToMain(TeamID) {
	
	if (TeamID != '' && TeamID.length > 0 && !isNaN(TeamID))
	{
		location.href="../main.html?teamid="+TeamID;
	}
}

function ActivateListViewButtons() {
	$('.users-tray').hide();
	
	$('a.users-count').bind("click", function(event) {
  		event.preventDefault();
		$(this).parent('.thing').children('.users-tray').slideToggle(250);
	});
	
	$('a.star').bind("click", function(event) {
  		event.preventDefault();
		$(this).addClass('active');
	});
}
/*
|--------------------------------------------------------------------------
|	END: FUNCTION JUNCTION
|--------------------------------------------------------------------------
*/


$(document).ready(function() {

/*
|--------------------------------------------------------------------------
|	BEGIN: INIT FUNCTIONS
|--------------------------------------------------------------------------
*/

APPURL = 'http://teamthing.apphb.com';

$("#tabstrip").kendoTabStrip({
	animation:	{
		open: {
			effects: "fadeIn"
		}
	}
});

$("#editor").kendoEditor({
	tools: [
		"bold",
		"italic",
		"underline"
	]
});

$("#statusfilterlist").kendoDropDownList();


/*
|--------------------------------------------------------------------------
|	END: INIT FUNCTIONS
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: GET ALL TEAMS FOR PULLDOWN
|--------------------------------------------------------------------------
*/
	$.get(
		APPURL+'/api/team',
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
		}
	);
/*
|--------------------------------------------------------------------------
|	END: GET ALL PUBLIC TEAMS FOR PULLDOWN
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: GET A SPECIFIC TEAM'S THINGS AND LIST THEM OUT
|--------------------------------------------------------------------------
*/
function GetTeamThings(TeamID) {
	$.get(
		APPURL+'/api/team/'+TeamID+'/things',
    	function(TeamThingsData) { 
			console.log(TeamThingsData);
			TeamThingsOutput = '';
			for(i=0;i<TeamThingsData.length;i++) {
				
				TeamThingsOutput+='<div class="thing" id="teamthing-'+TeamThingsData[i].id+'">';
          		TeamThingsOutput+='<div class="listpic"><img src="tt_assets/images/listpic.png" width="83" height="83" alt=""></div>';
                TeamThingsOutput+='<span class="listitem">';
            		TeamThingsOutput+='<div class="thingcontrols">';
                        if(TeamThingsData[i].isStarred == true) {
							TeamThingsOutput+='<a class="star active" href="#"></a>';
						} else {
							TeamThingsOutput+='<a class="star" href="#"></a>';
						}
                        TeamThingsOutput+='<span class="controls">';
						if(TeamThingsData[i].status == 'Delayed') {
             				TeamThingsOutput+='<input type="checkbox" data-icon1="In Progress" data-icon2="Delayed" checked="checked" />';
						} else if(TeamThingsData[i].status == 'InProgress') {
							TeamThingsOutput+='<input type="checkbox" data-icon1="In Progress" data-icon2="Delayed" />';
						}
              				TeamThingsOutput+='<br />';
              				TeamThingsOutput+='<span class="iconcontrols">';
              					TeamThingsOutput+='<div class="icon_complete"><a href="#"></a></div>';
								TeamThingsOutput+='<div class="icon_share"><a href="#"></a></div>';
								TeamThingsOutput+='<div class="icon_edit"><a href="#"></a></div>';
								TeamThingsOutput+='<div class="icon_delete"><a href="#"></a></div>';
              				TeamThingsOutput+='</span>';
                       TeamThingsOutput+='</span>';
                    TeamThingsOutput+='</div>';
            		TeamThingsOutput+='<div class="thingdesc">'+TeamThingsData[i].description+'</div>';
				TeamThingsOutput+='</span>';
                TeamThingsOutput+='<div class="users-tray">';
                	TeamThingsOutput+='<div class="userpic"><img src="tt_assets/images/listpic-default.png" width="55" height="55" alt=""></div>';
                    TeamThingsOutput+='<div class="userpic"><img src="tt_assets/images/listpic-default.png" width="55" height="55" alt=""></div>';
                    TeamThingsOutput+='<div class="userpic"><img src="tt_assets/images/listpic-default.png" width="55" height="55" alt=""></div>';
                    TeamThingsOutput+='<div class="userpic-dropzone" id="userpic-dropzone"></div>';
                    TeamThingsOutput+='<div class="clear-float"></div>';
                TeamThingsOutput+='</div>';
                TeamThingsOutput+='<a class="users-count" href="#">?</a>';
          	TeamThingsOutput+='</div>';
			}
			
			$('.myteamsthingslist .list').html(TeamThingsOutput);
			ActivateListViewButtons();
		}
	);
}

GetTeamThings(10);
/*
|--------------------------------------------------------------------------
|	END: GET A SPECIFIC TEAM'S THINGS AND LIST THEM OUT
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: GET MY THINGS AND LIST THEM OUT
|--------------------------------------------------------------------------
*/
function GetMyThings(UserID) {
	$.get(
		APPURL+'/api/user/'+UserID+'/things',
    	function(TeamThingsData) { 
			console.log(TeamThingsData);
			TeamThingsOutput = '';
			for(i=0;i<TeamThingsData.length;i++) {
				
				TeamThingsOutput+='<div class="thing" id="teamthing-'+TeamThingsData[i].id+'">';
          		TeamThingsOutput+='<div class="listpic"><img src="tt_assets/images/listpic.png" width="83" height="83" alt=""></div>';
                TeamThingsOutput+='<span class="listitem">';
            		TeamThingsOutput+='<div class="thingcontrols">';
                        if(TeamThingsData[i].isStarred == true) {
							TeamThingsOutput+='<a class="star active" href="#"></a>';
						} else {
							TeamThingsOutput+='<a class="star" href="#"></a>';
						}
                        TeamThingsOutput+='<span class="controls">';
						if(TeamThingsData[i].status == 'Delayed') {
             				TeamThingsOutput+='<input type="checkbox" data-icon1="In Progress" data-icon2="Delayed" checked="checked" />';
						} else if(TeamThingsData[i].status == 'InProgress') {
							TeamThingsOutput+='<input type="checkbox" data-icon1="In Progress" data-icon2="Delayed" />';
						}
              				TeamThingsOutput+='<br />';
              				TeamThingsOutput+='<span class="iconcontrols">';
              					TeamThingsOutput+='<div class="icon_complete"><a href="#"></a></div>';
								TeamThingsOutput+='<div class="icon_share"><a href="#"></a></div>';
								TeamThingsOutput+='<div class="icon_edit"><a href="#"></a></div>';
								TeamThingsOutput+='<div class="icon_delete"><a href="#"></a></div>';
              				TeamThingsOutput+='</span>';
                       TeamThingsOutput+='</span>';
                    TeamThingsOutput+='</div>';
            		TeamThingsOutput+='<div class="thingdesc">'+TeamThingsData[i].description+'</div>';
				TeamThingsOutput+='</span>';
                TeamThingsOutput+='<div class="users-tray">';
                	TeamThingsOutput+='<div class="userpic"><img src="tt_assets/images/listpic-default.png" width="55" height="55" alt=""></div>';
                    TeamThingsOutput+='<div class="userpic"><img src="tt_assets/images/listpic-default.png" width="55" height="55" alt=""></div>';
                    TeamThingsOutput+='<div class="userpic"><img src="tt_assets/images/listpic-default.png" width="55" height="55" alt=""></div>';
                    TeamThingsOutput+='<div class="userpic-dropzone" id="userpic-dropzone"></div>';
                    TeamThingsOutput+='<div class="clear-float"></div>';
                TeamThingsOutput+='</div>';
                TeamThingsOutput+='<a class="users-count" href="#">?</a>';
          	TeamThingsOutput+='</div>';
			}
			
			$('.mythingslist .list').html(TeamThingsOutput);
			ActivateListViewButtons();
		}
	);
}

GetMyThings(11);
/*
|--------------------------------------------------------------------------
|	END: GET MY THINGS AND LIST THEM OUT
|--------------------------------------------------------------------------
*/

/*
|--------------------------------------------------------------------------
|	BEGIN: GET LOGGED IN USER'S PROFILE DETAILS
|--------------------------------------------------------------------------
*/
                    
	$.get(
		APPURL+'/api/user?$filter=EmailAddress ne null and tolower(EmailAddress) eq \'toddanglin@gmail.com\'',
    	function(UserInfo) {
			console.log(UserInfo);
			$('#userpic img').attr('src',UserInfo[0].imagePath);
			$('#userinfo').html(UserInfo[0].emailAddress+'<br /><span class="usernav"><a href="#">View Profile</a> <a href="#">Sign Out</a></span>');
		}
	);
/*
|--------------------------------------------------------------------------
|	END: GET LOGGED IN USER'S PROFILE DETAILS
|--------------------------------------------------------------------------
*/	
	
/*
|--------------------------------------------------------------------------
|	BEGIN: ADD TO TEAM WINDOW AND FUNCTIONS
|--------------------------------------------------------------------------
*/

    var window = $("#addtoteam").kendoWindow({
        height: "220px",
        title: "Add to My Team",
        visible: false,
        width: "400px"
    }).data("kendoWindow");


$(".plus").click(function(){
    var window = $("#addtoteam").data("kendoWindow");
    window.center();
    window.open();
}); 
/*
|--------------------------------------------------------------------------
|	END: ADD TO TEAM WINDOW AND FUNCTIONS
|--------------------------------------------------------------------------
*/     

/*
|--------------------------------------------------------------------------
|	BEGIN: ADD THING WINDOW AND FUNCTIONS
|--------------------------------------------------------------------------
*/
    var window = $("#createthinginfo").kendoWindow({
        height: "220px",
        title: "Create a Thing",
        visible: false,
        width: "400px"
    }).data("kendoWindow");

$("#creatething").click(function(){
    var window = $("#createthinginfo").data("kendoWindow");
    window.center();
    window.open();
});
/*
|--------------------------------------------------------------------------
|	END: ADD THING WINDOW AND FUNCTIONS
|--------------------------------------------------------------------------
*/


	

/*
|--------------------------------------------------------------------------
|	BEGIN: ADD THING WINDOW AND FUNCTIONS
|--------------------------------------------------------------------------
*/
	
	function draggableOnDragStart(e) {
		//$("#draggable img").attr('src','tt_assets/images/listpic-halo.png');
		//$("#userpic-dropzone").text("(Drop here)");
	}

	function droptargetOnDragEnter(e) {
		//$("#userpic-dropzone").text("Now you can drop it.");
	}

	function droptargetOnDragLeave(e) {
		//$("#userpic-dropzone").text("(Drop here)");
	}

	function droptargetOnDrop(e) {
		//$("#userpic-dropzone").text("You did great!");
		//$("#draggable").removeClass("hollow");
		alert('On Target');
	}

	function draggableOnDragEnd(e) {
		var draggable = $("#draggable");
		if (!draggable.data("kendoDraggable").dropped) {
    		// drag ended outside of any droptarget
     		//$("#userpic-dropzone").text("Try again!");
		}

		draggable.removeClass("hollow");
	}

	$("#draggable").kendoDraggable({
		hint: function() {
			return $("#draggable").clone();
        },
   		dragstart: draggableOnDragStart,
    	dragend: draggableOnDragEnd
	});

	$("#userpic-dropzone").kendoDropTarget({
    	dragenter: droptargetOnDragEnter,
    	dragleave: droptargetOnDragLeave,
    	drop: droptargetOnDrop
	});

	var draggable = $("#draggable").data("kendoDraggable");

});
