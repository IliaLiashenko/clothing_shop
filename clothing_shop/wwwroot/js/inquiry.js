var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        }
        else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    loadDataTable("all");
                }
            }
        }
    }
});

function loadDataTable(status) {
    if ($.fn.DataTable.isDataTable('#tblData')) {
        dataTable.clear().destroy();
    }

    dataTable = $("#tblData").DataTable({
        "ajax": {
            url: '/inquiry/getall?status=' + status
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "fullName", "width": "10%" },
            { "data": "phoneNumber", "width": "10%" },
            { "data": "email", "width": "15%" },
            { "data": "orderStatus", "width": "10%" },
            { "data": "orderTotal", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Inquiry/Details/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                Details
                            </a>
                        </div>
                    `;
                },
                "width": "5%"
            }
        ]
    });

}
