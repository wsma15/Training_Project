$(document).ready(function () {
    'use strict';

    // Ensure variables are correctly defined in the global scope
    var universityNames = window.universityNames || [];
    var companyNames = window.companyNames || [];
    var universitySupervisors = window.universitySupervisors || [];
    var companySupervisors = window.companySupervisors || [];
    var deleteUrl = window.deleteUrl || '';

    // Change event handler for user role radio buttons
    $('input[name="UserRole"]').change(function () {
        var role = $(this).val();
        var additionalFields = $('#additionalFields');
        additionalFields.empty();  // Clear previous fields

        // Append relevant fields based on user role
        if (role === 'UniversitySupervisor') {
            additionalFields.append(`
                <div class="form-group">
                    <label for="UniversityID" class="control-label col-md-2">University</label>
                    <div class="col-md-10">
                        <select id="UniversityID" name="UniversityID" class="form-control"></select>
                        <span class="text-danger" data-valmsg-for="UniversityID" data-valmsg-replace="true"></span>
                    </div>
                </div>
            `);
            populateDropdown('#UniversityID', universityNames);
        } else if (role === 'CompanySupervisor') {
            additionalFields.append(`
                <div class="form-group">
                    <label for="CompanyID" class="control-label col-md-2">Company</label>
                    <div class="col-md-10">
                        <select id="CompanyID" name="CompanyID" class="form-control"></select>
                        <span class="text-danger" data-valmsg-for="CompanyID" data-valmsg-replace="true"></span>
                    </div>
                </div>
            `);
            populateDropdown('#CompanyID', companyNames);
        } else if (role === 'Trainer') {
            additionalFields.append(`
                <div class="form-group">
                    <label for="UniversityID" class="control-label col-md-2">University</label>
                    <div class="col-md-10">
                        <select id="UniversityID" name="UniversityID" class="form-control"></select>
                        <span class="text-danger" data-valmsg-for="UniversityID" data-valmsg-replace="true"></span>
                    </div>
                </div>
                <div class="form-group">
                    <label for="UniversitySupervisorID" class="control-label col-md-2">University Supervisor</label>
                    <div class="col-md-10">
                        <select id="UniversitySupervisorID" name="UniversitySupervisorID" class="form-control"></select>
                        <span class="text-danger" data-valmsg-for="UniversitySupervisorID" data-valmsg-replace="true"></span>
                    </div>
                </div>
                <div class="form-group">
                    <label for="CompanySupervisorID" class="control-label col-md-2">Company Supervisor</label>
                    <div class="col-md-10">
                        <select id="CompanySupervisorID" name="CompanySupervisorID" class="form-control"></select>
                        <span class="text-danger" data-valmsg-for="CompanySupervisorID" data-valmsg-replace="true"></span>
                    </div>
                </div>
            `);
            populateDropdown('#UniversityID', universityNames);
            populateDropdown('#UniversitySupervisorID', universitySupervisors);
            populateDropdown('#CompanySupervisorID', companySupervisors);
        }
    });

    // Function to populate dropdowns with data
    function populateDropdown(selector, data) {
        var dropdown = $(selector);
        dropdown.empty();  // Clear existing options
        $.each(data, function (index, item) {
            dropdown.append(new Option(item.Text, item.Value));
        });
    }

    // Handle form submissions with AJAX
    function handleFormSubmission(modalId, formId) {
        $(document).on('submit', formId, function (e) {
            e.preventDefault();
            var form = $(this);
            $.ajax({
                url: form.attr('action'),
                type: 'POST',
                data: form.serialize(),
                success: function (response) {
                    if (response.success) {
                        $(modalId).modal('hide');
                        location.reload();
                    } else {
                        $(modalId + ' .modal-body').html(response);
                    }
                },
                error: function () {
                    alert('An error occurred while processing the request.');
                }
            });
        });
    }

    // Initialize form handlers
    handleFormSubmission('#addUniversitySupervisorModal', '#addUniversitySupervisorForm');
    handleFormSubmission('#addCompanySupervisorModal', '#addCompanySupervisorForm');
    handleFormSubmission('#addTrainerModal', '#addTrainerForm');

    // Delete user function
    function deleteUser(id) {
        if (confirm("Are you sure you want to delete this user?")) {
            $.ajax({
                url: deleteUrl,
                type: 'POST',
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        alert('User deleted successfully.');
                        location.reload();
                    } else {
                        alert('Failed to delete user.');
                    }
                },
                error: function () {
                    alert('An error occurred while deleting the user.');
                }
            });
        }
    }

    // Expose the deleteUser function to global scope for use in HTML
    window.deleteUser = deleteUser;
});
