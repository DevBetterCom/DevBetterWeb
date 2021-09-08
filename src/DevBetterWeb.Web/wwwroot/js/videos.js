$(document).ready(function () {
    var userRole = '@(User.IsInRole("Administrators") ? "true" : "false")';
    var dateOptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
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
            { "data": "id", "name": "Id", "autoWidth": true },
            { "data": "name", "name": "Name", "autoWidth": true },
            {
                "data": "releaseTime", "name": "Release Time", "autoWidth": true,
                "render": function (data, type, full, meta) {
                    return new Date(full.releaseTime).toLocaleDateString("en-US", dateOptions);
                }
            },
            {
                "data": "createdTime", "name": "Created Time", "autoWidth": true,
                "render": function (data, type, full, meta) {
                    return new Date(full.createdTime).toLocaleDateString("en-US", dateOptions);
                }
            },
            { "data": "status", "name": "Status", "autoWidth": true },
            {
                "render": function (data, type, full, meta) {                    
                    if (userRole) {
                        return "<a href='Videos/Details/" + full.id + "'><i class='fas fa-info-circle'></i></a> |  <a href='Videos/Edit/" + full.id + "'><i class='fas fa-edit'></i></a> |<a href='Videos/Delete/" + full.id + "'><i class='far fa-trash-alt'></i></a>";
                    } else {
                        return "";
                    }
                        
                   
                }
            },
        ]
    });
});