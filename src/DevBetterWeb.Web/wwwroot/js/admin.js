$(function () {

    $('#usersTable').DataTable();

    $('#removeUserModal').on('show.bs.modal', function (e) {
        var button = $(e.relatedTarget);

        var userEmail = button.attr('data-userEmail');
        var userId = button.attr('data-userId');

        $('#spnRemoveUserEmail').html(userEmail);
        $('#hidRemoveUserId').val(userId);
    });

    $('#removeRoleModal').on('show.bs.modal', function (e) {
        var button = $(e.relatedTarget);

        var roleName = button.attr('data-roleName');
        var roleId = button.attr('data-roleId');

        $('#spnRemoveRoleName').html(roleName);
        $('#hidRemoveRoleId').val(roleId);
    });

  

});