currentPage = 0;
recordsTotal = 0;
page = 1;
recordsPage = 10;

$(document).ready(function () {
    

    var adminRole = '@(User.IsInRole("Administrators") ? "true" : "false")';    
    var dateOptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };    
    
    refreshMembersVideos(currentPage);


    $("#videosDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": false,
        "sort": false,
        "ajax": {
            "url": "/videos/list",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": [0],
            "visible": false,
            "searchable": false
        }],
        "columns": [
            { "data": "videoId", "name": "Id", "autoWidth": true },
            { "data": "title", "name": "Name", "autoWidth": true },
            {
                "data": "dateUploaded", "name": "Uploaded Time", "autoWidth": true,
                "render": function (data, type, full, meta) {
                    return new Date(full.dateUploaded).toLocaleDateString("en-US", dateOptions);
                }
            },
            {
                "data": "dateCreated", "name": "Created Time", "autoWidth": true,
                "render": function (data, type, full, meta) {
                    return new Date(full.dateCreated).toLocaleDateString("en-US", dateOptions);
                }
            },
            { "data": "status", "name": "Status", "autoWidth": true },
            {
                "render": function (data, type, full, meta) {                    
                    if (adminRole) {
                        return "<a href='Videos/Details/" + full.videoId + "'><i class='fas fa-info-circle'></i></a> |  <a href='Videos/Edit/" + full.videoId + "'><i class='fas fa-edit'></i></a> |<a href='Videos/Delete/" + full.videoId + "'><i class='far fa-trash-alt'></i></a>";
                    } else {
                        return "";
                    }
                        
                   
                }
            },
        ]
    });

});