currentPage = 0;
recordsTotal = 0;
page = 1;
recordsPage = 12;
$pagination = $('#videoPagination');

$(document).ready(function () {
    refreshMembersVideos(currentPage);
});