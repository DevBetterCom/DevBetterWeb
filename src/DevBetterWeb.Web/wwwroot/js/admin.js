$(function () {

    $('#removeUserModal').on('show.bs.modal', function (e) {
        var button = $(e.relatedTarget);

        var userEmail = button.attr('data-userEmail');
        var userId = button.attr('data-userId');

        $('#spnRemoveUserEmail').html(userEmail);
        $('#hidRemoveUserId').val(userId);
    });
});