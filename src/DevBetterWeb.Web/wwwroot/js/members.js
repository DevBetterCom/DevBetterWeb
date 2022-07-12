$(function () {

    $('#membersTable').DataTable({ "pageLength" : 25 });

});

function showAllMembersFunc() {
	var showAllMembers = document.getElementById("showAllMembers").checked;
	var table = $('#membersTable').DataTable();
	table.column(4).search( showAllMembers?"":"true" ).draw();
}
