// Initiate datatables in roles, tables, users page
(function() {
    
    $('#dataTables-example').DataTable({
        responsive: true,
        pageLength: 10,
        lengthChange: false,
        searching: true,
        ordering: true,
        "columnDefs": [
            {
                "targets": -1, // Target the last column
                "orderable": false // Disable sorting on the last column
            }
        ],
    });
})();