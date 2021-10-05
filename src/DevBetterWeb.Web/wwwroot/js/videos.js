$(document).ready(function () {
    var adminRole = '@(User.IsInRole("Administrators") ? "true" : "false")';    
    var dateOptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };    
    
    refreshMebersVideos();

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

    function refreshMebersVideos() {
        var userRole = '@(User.IsInRole("Administrators,Members,Alumni") ? "true" : "false")';
        if (userRole) {
            $.ajax({
                type: "POST",
                url: "/videos/list",
                data: { draw: '1', start: '0', length: '10' },
                dataType: "json",
                success: function (videosReponse) {
                    var divHtml = "";
                    if (videosReponse && videosReponse.data && videosReponse.data.length > 0) {
                        videosReponse.data.forEach(video => {
                            divHtml += "<div class='col col-sm-12 col-md-3 col-lg-3 padding-10'><h3>" + video?.title + "</h3><span class='style-scope ytd-video-meta-block'>" + video?.views + " views</span><span class='style-scope ytd-video-meta-block'> " + timeSince(new Date(video?.dateCreated)) + " ago</span></div>";
                        });
                    }
                    document.getElementById('members-videos-list').innerHTML = divHtml;
                },
                error: function (errMsg) {
                    alert(errMsg);
                }
            });
        }
    }

    function timeSince(date) {

        var seconds = Math.floor((new Date() - date) / 1000);

        var interval = seconds / 31536000;

        if (interval > 1) {
            return Math.floor(interval) + " years";
        }
        interval = seconds / 2592000;
        if (interval > 1) {
            return Math.floor(interval) + " months";
        }
        interval = seconds / 86400;
        if (interval > 1) {
            return Math.floor(interval) + " days";
        }
        interval = seconds / 3600;
        if (interval > 1) {
            return Math.floor(interval) + " hours";
        }
        interval = seconds / 60;
        if (interval > 1) {
            return Math.floor(interval) + " minutes";
        }
        return Math.floor(seconds) + " seconds";
    }
});