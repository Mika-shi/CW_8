$(document).ready(function () {

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

                let avatar = '';

                if (result.avatarPath) {
                    avatar = `
                    <img src="${result.avatarPath}" style="width:50px;height:50px;object-fit:cover;border-radius:50%;flex-shrink:0;">
                `;
                } else {
                    let firstLetter = result.userName
                        ? result.userName.charAt(0).toUpperCase()
                        : '?';

                    avatar = `
                    <div class="d-flex align-items-center justify-content-center bg-secondary text-white"
                         style="width:50px;height:50px;border-radius:50%;font-weight:bold;flex-shrink:0;">
                        ${firstLetter}
                    </div>
                `;
                }

                let html = `
                <div class="card mb-3">
                    <div class="card-body">
                        <div class="d-flex gap-3">

                            ${avatar}

                            <div class="flex-grow-1">
                                <div class="d-flex justify-content-between mb-2">
                                    <a href="/Profile/Details/${result.userId}" class="text-decoration-none fw-bold">
                                        ${result.userName}
                                    </a>

                                    <small class="text-muted">
                                        ${result.createdOn}
                                    </small>
                                </div>

                                <div>
                                    ${result.text}
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            `;

                $('#replies').append(html);

                $('#replyText').val('');

                $('html, body').animate({
                    scrollTop: $('#replies').prop('scrollHeight') + $('#replies').offset().top
                }, 500);
            }
        });
    });

});