function loadReplies(page) {
    let container = $('#repliesContainer');
    let topicId = container.data('topic-id');

    $.ajax({
        url: '/Topic/GetReplies',
        type: 'GET',
        data: {
            topicId: topicId,
            page: page
        },
        success: function (html) {
            container.html(html);
        }
    });
}

$(document).ready(function () {

    if ($('#repliesContainer').length) {
        loadReplies(1);
    }

    $(document).on('click', '.reply-page-link', function () {
        let page = $(this).data('page');

        loadReplies(page);
    });

    $('.password-toggle').click(function () {
        let button = $(this);
        let targetId = button.data('target');
        let input = $('#' + targetId);
        let icon = button.find('i');

        if (input.attr('type') === 'password') {
            input.attr('type', 'text');
            icon.removeClass('bi-eye');
            icon.addClass('bi-eye-slash');
        } else {
            input.attr('type', 'password');
            icon.removeClass('bi-eye-slash');
            icon.addClass('bi-eye');
        }
    });

    $('#passwordInput').on('input', function () {
        let password = $(this).val();

        let hasLength = password.length >= 6;
        let hasUpper = /[A-Z]/.test(password);
        let hasLower = /[a-z]/.test(password);
        let hasDigit = /[0-9]/.test(password);

        $('#ruleLength').toggleClass('text-success', hasLength).toggleClass('text-danger', !hasLength);
        $('#ruleUpper').toggleClass('text-success', hasUpper).toggleClass('text-danger', !hasUpper);
        $('#ruleLower').toggleClass('text-success', hasLower).toggleClass('text-danger', !hasLower);
        $('#ruleDigit').toggleClass('text-success', hasDigit).toggleClass('text-danger', !hasDigit);
    });

    $('#confirmPasswordInput').on('input', function () {
        let password = $('#passwordInput').val();
        let confirmPassword = $(this).val();

        if (confirmPassword !== password) {
            $('#confirmPasswordError').text('Пароли не совпадают');
        } else {
            $('#confirmPasswordError').text('');
        }
    });

    $('#registerForm').submit(function (event) {
        let password = $('#passwordInput').val();
        let confirmPassword = $('#confirmPasswordInput').val();

        let hasLength = password.length >= 6;
        let hasUpper = /[A-Z]/.test(password);
        let hasLower = /[a-z]/.test(password);
        let hasDigit = /[0-9]/.test(password);

        if (!hasLength || !hasUpper || !hasLower || !hasDigit) {
            event.preventDefault();
            return;
        }

        if (password !== confirmPassword) {
            event.preventDefault();
            $('#confirmPasswordError').text('Пароли не совпадают');
        }
    });
    $('#scrollToReplyButton').click(function () {
        let replyForm = $('#replyForm');

        if (replyForm.length) {
            $('html, body').animate({
                scrollTop: replyForm.offset().top
            }, 500);

            $('#replyText').focus();
        }
    });

    $('#sendReplyButton').click(function () {
        let button = $(this);
        let topicId = button.data('topic-id');
        let text = $('#replyText').val().trim();

        if (text.length === 0) {
            $('#replyError').text('Введите текст ответа');
            return;
        }

        $('#replyError').text('');

        $.ajax({
            url: '/Topic/AddReply',
            type: 'POST',
            data: {
                topicId: topicId,
                text: text
            },
            success: function (result) {
                if (!result.success) {
                    $('#replyError').text(result.error);
                    return;
                }

                $('#replyText').val('');

                loadReplies(result.totalPages);

                setTimeout(function () {
                    $('html, body').animate({
                        scrollTop: $('#repliesContainer').offset().top + $('#repliesContainer').height()
                    }, 500);
                }, 200);
            }
        });
    });

});