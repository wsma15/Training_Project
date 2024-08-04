$(document).ready(function () {
    var receiverId;
    var pageIndex = 1; // Track the page index for loading messages

    // Handle user link click to load chat history
    $(document).on('click', '.user-link', function (e) {
        e.preventDefault();
        receiverId = $(this).data('senderid');
        $('#receiverId').val(receiverId);
        $('#chat-user-name').text($(this).find('.name').text());

        // Get the Base64 string for the avatar
        var avatarBase64 = $(this).data('avatar');

        // Set the avatar source using Base64 data
        var avatarSrc = avatarBase64 ? "data:image/png;base64," + avatarBase64 : '~/Content/Images/default.jpg';
        $('#chat-avatar').attr('src', avatarSrc);

        pageIndex = 1; // Reset the page index
        loadChatHistory(receiverId, pageIndex, true); // Load the initial messages
    });

    // Load more messages when the "Load More" button is clicked
    $('#loadMoreBtn').click(function () {
        pageIndex++;
        loadChatHistory(receiverId, pageIndex, false);
    });

    // Function to load chat history
    async function loadChatHistory(receiverId, pageIndex, clearChat) {
        try {
            console.log('Loading chat history for:', receiverId, 'Page:', pageIndex); // Debug log
            const response = await fetch(`@Url.Action("GetChatHistory", "Message")?receiverId=${receiverId}&pageIndex=${pageIndex}`);
            if (!response.ok) throw new Error('Network response was not ok');
            const data = await response.json();
            const chatMessages = $('#chat-messages');
            if (clearChat) chatMessages.empty(); // Clear chat if loading the initial messages

            data.messages.forEach(message => {
                const formattedTime = formatTimestamp(message.Timestamp);
                chatMessages.prepend(`<div class="message-item ${message.IsMine ? 'my-message' : 'other-message'}"><strong>${message.SenderName}:</strong> ${message.MessageText} <span class="text-muted small">${formattedTime}</span></div><hr/>`);
            });

            $('#loadMoreBtn').toggle(data.hasMoreMessages); // Show or hide the "Load More" button

            if (clearChat) chatMessages.scrollTop(chatMessages[0].scrollHeight); // Scroll to bottom only if initial load
        } catch (error) {
            console.error('Error loading chat history:', error);
        }
    }

    // Handle form submission for sending a new message
    $('#messageForm').submit(async function (e) {
        e.preventDefault();
        const formData = new FormData(this);
        try {
            const response = await fetch('@Url.Action("SendNewMessage", "Message")', {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });
            const result = await response.json();
            if (result.success) {
                loadChatHistory(receiverId, 1, true); // Reload the chat history after sending a message
                $('#messageText').val('');
            } else {
                alert('Error sending message: ' + result.message);
            }
        } catch (error) {
            console.error('Error sending message:', error);
        }
    });

    // Function to format timestamps
    function formatTimestamp(timestamp) {
        var now = moment();
        var messageTime = moment.utc(timestamp).local();
        if (messageTime.isSame(now, 'day')) {
            return messageTime.format('h:mm A') + ' (Today)';
        } else if (messageTime.isSame(now.subtract(1, 'days'), 'day')) {
            return messageTime.format('h:mm A') + ' (Yesterday)';
        } else {
            return messageTime.format('h:mm A, MMM D, YYYY');
        }
    }

    // Search users based on input
    $('#searchButton').click(async function () {
        const query = $('#searchInput').val().trim();
        searchUsers(query);
    });

    // Also trigger search on input change for a better UX
    $('#searchInput').on('input', function () {
        const query = $(this).val().trim();
        searchUsers(query);
    });

    async function searchUsers(query) {
        try {
            console.log('Searching users for:', query); // Debug log
            const response = await fetch('@Url.Action("SearchUsers", "Message")?query=' + encodeURIComponent(query));
            if (!response.ok) throw new Error('Network response was not ok');
            const users = await response.json();
            const userList = $('#userList');
            userList.empty();
            users.forEach(user => {
                var avatarUrl = user.AvatarUrl ? user.AvatarUrl.replace('~', '') : '~/images/default.jpg';
                userList.append(`<li class="clearfix user-link p-2 border-bottom" data-senderid="${user.Id}" data-avatar="${avatarUrl}">
                    <img src="${avatarUrl.replace('~', '')}" alt="avatar" class="rounded-circle mr-2" width="40" height="40" />
                    <div class="about">
                        <div class="name font-weight-bold">${user.Name}</div>
                        <div class="status text-muted">
                            <i class="fa fa-circle ${user.IsOnline ? "text-success" : "text-danger"}"></i> ${user.IsOnline ? "Online" : "Offline"}
                        </div>
                    </div>
                </li>`);
            });
        } catch (error) {
            console.error('Error searching users:', error);
        }
    }

    // Show the "Load More" button when scrolling near the top of the chat messages
    $('#chat-messages').on('scroll', function () {
        if ($(this).scrollTop() < 50) { // Adjust the threshold as needed
            $('#loadMoreBtn').show();
        } else {
            $('#loadMoreBtn').hide();
        }
    });
});
