$(document).ready(function () {
    'use strict';

    // Event handler for UserRole change
    $('input[name="UserRole"]').change(function () {
        var role = $(this).val();
        var additionalFields = $('#additionalFields');
        additionalFields.empty();

        if (role === 'UniversitySupervisor') {
            additionalFields.append(`
                <div class="form-group">
                    <label for="UniversityID">University ID</label>
                    <select id="UniversityID" name="UniversityID" class="form-control">
                        <!-- Options will be populated by JavaScript -->
                    </select>
                    <span class="text-danger field-validation-valid" data-valmsg-for="UniversityID" data-valmsg-replace="true"></span>
                </div>
            `);
            // Populate dropdown options dynamically
            $.getJSON('/Admin/GetUniversityNames', function (data) {
                var universitySelect = $('#UniversityID');
                universitySelect.empty();
                $.each(data, function (key, value) {
                    universitySelect.append($('<option></option>').attr('value', key).text(value));
                });
            });

        } else if (role === 'CompanySupervisor') {
            additionalFields.append(`
                <div class="form-group">
                    <label for="CompanyID">Company ID</label>
                    <select id="CompanyID" name="CompanyID" class="form-control">
                        <!-- Options will be populated by JavaScript -->
                    </select>
                    <span class="text-danger field-validation-valid" data-valmsg-for="CompanyID" data-valmsg-replace="true"></span>
                </div>
            `);
            // Populate dropdown options dynamically
            $.getJSON('/Admin/GetCompanyNames', function (data) {
                var companySelect = $('#CompanyID');
                companySelect.empty();
                $.each(data, function (key, value) {
                    companySelect.append($('<option></option>').attr('value', key).text(value));
                });
            });

        } else if (role === 'Trainer') {
            additionalFields.append(`
                <div class="form-group">
                    <label for="UniversityID">University ID</label>
                    <select id="UniversityID" name="UniversityID" class="form-control">
                        <!-- Options will be populated by JavaScript -->
                    </select>
                    <span class="text-danger field-validation-valid" data-valmsg-for="UniversityID" data-valmsg-replace="true"></span>
                </div>
                <div class="form-group">
                    <label for="UniversitySupervisorID">University Supervisor ID</label>
                    <select id="UniversitySupervisorID" name="UniversitySupervisorID" class="form-control">
                        <!-- Options will be populated by JavaScript -->
                    </select>
                    <span class="text-danger field-validation-valid" data-valmsg-for="UniversitySupervisorID" data-valmsg-replace="true"></span>
                </div>
                <div class="form-group">
                    <label for="CompanySupervisorID">Company Supervisor ID</label>
                    <select id="CompanySupervisorID" name="CompanySupervisorID" class="form-control">
                        <!-- Options will be populated by JavaScript -->
                    </select>
                    <span class="text-danger field-validation-valid" data-valmsg-for="CompanySupervisorID" data-valmsg-replace="true"></span>
                </div>
            `);
            // Populate dropdown options dynamically
            $.getJSON('/Admin/GetUniversityNames', function (data) {
                var universitySelect = $('#UniversityID');
                universitySelect.empty();
                $.each(data, function (key, value) {
                    universitySelect.append($('<option></option>').attr('value', key).text(value));
                });
            });
            $.getJSON('/Admin/GetUniversitySupervisors', function (data) {
                var supervisorSelect = $('#UniversitySupervisorID');
                supervisorSelect.empty();
                $.each(data, function (key, value) {
                    supervisorSelect.append($('<option></option>').attr('value', key).text(value));
                });
            });
            $.getJSON('/Admin/GetCompanySupervisors', function (data) {
                var companySupervisorSelect = $('#CompanySupervisorID');
                companySupervisorSelect.empty();
                $.each(data, function (key, value) {
                    companySupervisorSelect.append($('<option></option>').attr('value', key).text(value));
                });
            });
        }
    });

    // Function to handle form submission
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
                        location.reload(); // Or update the table dynamically
                    } else {
                        $(modalId + ' .modal-body').html(response);
                    }
                },
                error: function () {
                    alert('An error occurred.');
                }
            });
        });
    }

    // Initialize form handling for different modals
    handleFormSubmission('#addUniversitySupervisorModal', '#addUniversitySupervisorForm');
    handleFormSubmission('#addCompanySupervisorModal', '#addCompanySupervisorForm');
    handleFormSubmission('#addTrainerModal', '#addTrainerForm');

    // Function to delete user
    function deleteUser(id) {
        if (confirm("Are you sure you want to delete this user?")) {
            $.ajax({
                url: '@Url.Action("DeleteUser")',
                type: 'POST',
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        alert('User deleted successfully.');
                        location.reload(); // Reload the page or update the table dynamically
                    } else {
                        alert('Failed to delete user.');
                    }
                },
                error: function () {
                    alert('An error occurred.');
                }
            });
        }
    }
});
