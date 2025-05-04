// Document ready function
$(document).ready(function () {
    // Initialize the page
    updateBatchButtonsState();

    // Select All checkbox handler
    $('#selectAll').click(function () {
        $('.row-checkbox').prop('checked', this.checked);
        updateBatchButtonsState();
    });

    // Individual checkbox handler
    $('.row-checkbox').change(function () {
        if (!this.checked) {
            $('#selectAll').prop('checked', false);
        }
        updateBatchButtonsState();
    });

    // Batch Activate button handler
    $('#batchActivate').click(function () {
        var selectedUsers = getSelectedUserNames();
        if (selectedUsers.length === 0) return;

        var statuses = getSelectedStatuses(selectedUsers);
        var inactiveCount = statuses.filter(s => !s.isActive).length;
        var activeCount = statuses.filter(s => s.isActive).length;

        var message = "Are you sure you want to activate the selected users?";
        if (activeCount > 0) {
            message = `${activeCount} selected user(s) are already active. ` +
                `${inactiveCount} user(s) will be activated. Continue?`;
        }

        if (confirm(message)) {
            batchToggleStatus(selectedUsers, true);
        }
    });

    // Batch Deactivate button handler
    $('#batchDeactivate').click(function () {
        var selectedUsers = getSelectedUserNames();
        if (selectedUsers.length === 0) return;

        var statuses = getSelectedStatuses(selectedUsers);
        var activeCount = statuses.filter(s => s.isActive).length;
        var inactiveCount = statuses.filter(s => !s.isActive).length;

        var message = "Are you sure you want to deactivate the selected users?";
        if (inactiveCount > 0) {
            message = `${inactiveCount} selected user(s) are already inactive. ` +
                `${activeCount} user(s) will be deactivated. Continue?`;
        }

        if (confirm(message)) {
            batchToggleStatus(selectedUsers, false);
        }
    });
});

// Helper functions
function updateBatchButtonsState() {
    var checkedBoxes = $('.row-checkbox:checked');
    var hasChecked = checkedBoxes.length > 0;

    $('#batchActivate, #batchDeactivate').prop('disabled', !hasChecked);

    if (hasChecked) {
        var allInactive = checkedBoxes.closest('tr').find('.status-badge').toArray()
            .every(el => $(el).hasClass('status-inactive'));
        var allActive = checkedBoxes.closest('tr').find('.status-badge').toArray()
            .every(el => $(el).hasClass('status-active'));

        $('#batchActivate').toggle(!allActive);
        $('#batchDeactivate').toggle(!allInactive);
    }
}

function getSelectedUserNames() {
    return $('.row-checkbox:checked').map(function () {
        return $(this).data('userid');
    }).get();
}

function getSelectedStatuses(userNames) {
    return userNames.map(function (userName) {
        var row = $(`.row-checkbox[data-userid="${userName}"]`).closest('tr');
        return {
            userName: userName,
            isActive: row.find('.status-badge').hasClass('status-active')
        };
    });
}

function batchToggleStatus(userNames, activate) {
    $.ajax({
        url: '/User/BatchToggleUserStatus', // This should match your route
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            userNames: userNames,
            activate: activate
        }),
        success: function (response) {
            if (response.success) {
                showToast(activate ? 'Users activated successfully' : 'Users deactivated successfully');

                userNames.forEach(function (userName) {
                    var checkbox = $(`.row-checkbox[data-userid="${userName}"]`);
                    var row = checkbox.closest('tr');
                    var badge = row.find('.status-badge');
                    badge
                        .removeClass('status-active status-inactive')
                        .addClass(activate ? 'status-active' : 'status-inactive')
                        .text(activate ? 'Active' : 'Inactive');
                    row.toggleClass('inactive-row', !activate);
                    checkbox.prop('checked', false);
                });
                $('#selectAll').prop('checked', false);
                updateBatchButtonsState();
            } else {
                showToast('Error: ' + response.message, 'error');
            }
        },
        error: function (xhr) {
            showToast('An error occurred: ' + xhr.responseText, 'error');
        }
    });
}

function showToast(message, type = 'success') {
    alert(message);
}