$(document).ready(function () {
    // Handle role change and dynamically load additional fields
        alert("er");
    $('input[name="UserRole"]').change(function () {

        var role = $(this).val();
        var additionalFields = $('#additionalFields');
        additionalFields.empty();

        if (role === 'UniversitySupervisor') {
            additionalFields.append(`
                        <div class="form-group">
                            @Html.LabelFor(model => model.UniversityID, htmlAttributes: new { @class = "control-label col-md-2" })
                            <div class="col-md-10">
                                @Html.DropDownListFor(model => model.UniversityID, Model.UniversityNames, new { @class = "form-control" })
                                @Html.ValidationMessageFor(model => model.UniversityID, "", new { @class = "text-danger" })
                            </div>
                        </div>
                    `);
        } else if (role === 'CompanySupervisor') {
            additionalFields.append(`
                        <div class="form-group">
                            @Html.LabelFor(model => model.CompanyID, htmlAttributes: new { @class = "control-label col-md-2" })
                            <div class="col-md-10">
                            
                                @Html.DropDownListFor(model => model.CompanyID, Model.CompaniesNames, new { @class = "form-control" })
                                @Html.ValidationMessageFor(model => model.CompanyID, "", new { @class = "text-danger" })
                            </div>
                        </div>
                    `);
            alert("com");
        }
        else if (role === 'Trainer') {
            additionalFields.append(`
                        <div class="form-group">
                            @Html.LabelFor(m=>m.UniversityID)
                            @Html.DropDownListFor(m=>m.UniversityID, Model.UniversityNames)
                            @Html.ValidationMessageFor(m=>m.UniversityID)
                        </div>
                        <div class="form-group">
                            @Html.LabelFor(m=>m.UniversitySupervisorID)
                            @Html.DropDownListFor(m=>m.UniversitySupervisorID, Model.UniSupervisors)
                            @Html.ValidationMessageFor(m=>m.UniversitySupervisorID)
                        </div>
                        <div class="form-group">
                            @Html.LabelFor(m=>m.CompanySupervisorID)
                            @Html.DropDownListFor(m=>m.CompanySupervisorID, Model.CompanySupervisors)
                            @Html.ValidationMessageFor(m=>m.CompanySupervisorID)
                        </div>
                    `);
        }
    });

    // Function to handle form submission with AJAX
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
    handleFormSubmission('#addUserModal', '#addUserForm');
});
