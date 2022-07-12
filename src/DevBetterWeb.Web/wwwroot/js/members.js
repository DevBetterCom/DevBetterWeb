$(function () {

    $('#membersTable').DataTable({ "pageLength" : 25 });

});

$(document).ready(function() {
	showAllMembersFunc();

	$('#showAllMembers').change(function() {
		showAllMembersFunc();
	});
});

function showAllMembersFunc() {
	var showAllMembers = document.getElementById("showAllMembers").checked;
	var table = $('#membersTable').DataTable();
	table.column(4).search( showAllMembers?"":"true" ).draw();
}
