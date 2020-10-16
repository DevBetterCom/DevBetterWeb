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

    $('#deleteSubscriptionModal').on('show.bs.modal', function (e) {
        var button = $(e.relatedTarget);

        var subscriptionId = button.attr('data-subscriptionId');
        var startDate = button.attr('data-startDate');
        var endDate = button.attr('data-endDate');

        $('#hidDeleteSubscriptionId').val(subscriptionId);
        $('#spnDeleteSubscriptionStartDate').html(startDate);
        $('#spnDeleteSubscriptionEndDate').html(endDate);
    });

    $('#editSubscriptionModal').on('show.bs.modal', function (e) {
        var button = $(e.relatedTarget);

        var subscriptionId = button.attr('data-subscriptionId');
        var startDate = button.attr('data-startDate');
        var endDate = button.attr('data-endDate');

        $('#hidEditSubscriptionId').val(subscriptionId);
        $('#inputEditSubscriptionStartDate').val(startDate);
        $('#inputEditSubscriptionEndDate').val(endDate);
    })

});