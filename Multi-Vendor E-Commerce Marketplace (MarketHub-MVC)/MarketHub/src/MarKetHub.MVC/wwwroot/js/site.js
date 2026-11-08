// Small progressive-enhancement helpers. Every page still works without JavaScript.
(function ($) {
    'use strict';

    // <form data-confirm="Are you sure?"> asks before submitting.
    $(document).on('submit', 'form[data-confirm]', function (e) {
        if (!window.confirm(this.getAttribute('data-confirm'))) {
            e.preventDefault();
        }
    });

    // <form data-once> can only be submitted once (protects checkout from a double click creating two orders).
    $(document).on('submit', 'form[data-once]', function (e) {
        var $form = $(this);
        if ($form.data('submitted')) {
            e.preventDefault();
            return;
        }
        if ($.fn.valid && !$form.valid()) {
            return; // client-side validation failed: let the user fix it and try again
        }
        $form.data('submitted', true);
        $form.find('button[type=submit]').prop('disabled', true).text('Please wait...');
    });

    // <input type="checkbox" data-toggle-target="#id"> shows or hides the target block.
    $(document).on('change', 'input[data-toggle-target]', function () {
        $($(this).data('toggle-target')).toggleClass('d-none', !this.checked);
    });

    // <img data-main-image="#mainImage"> swaps the big product picture.
    $(document).on('click', 'img[data-main-image]', function () {
        $($(this).data('main-image')).attr('src', this.src);
    });
})(jQuery);
