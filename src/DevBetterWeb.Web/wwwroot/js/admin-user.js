$(function () {
    var form = $('#personalInfoForm');
    var addressFields = form.find('[name="UserPersonalUpdateModel.Address"], [name="UserPersonalUpdateModel.City"], [name="UserPersonalUpdateModel.State"], [name="UserPersonalUpdateModel.Country"], [name="UserPersonalUpdateModel.PostalCode"]');

    function normalize(value) {
        return (value || '').replace(/\r\n/g, '\n').trim();
    }

    // Matches OnPostUpdatePersonalInfoAsync: the address is only required when the admin changes it to something non-blank.
    function isAddressRequired() {
        var fields = addressFields.toArray();
        var hasValue = fields.some(function (field) { return normalize(field.value) !== ''; });
        var isChanged = fields.some(function (field) { return normalize(field.value) !== normalize(field.defaultValue); });
        return hasValue && isChanged;
    }

    addressFields.filter('[data-val-required]').each(function () {
        $(this).rules('remove', 'required');
        $(this).rules('add', { required: { depends: isAddressRequired }, normalizer: normalize });
    });

    // Filling in or clearing one address field can resolve errors already shown on the others.
    addressFields.on('change', function () {
        addressFields.filter('.input-validation-error').each(function () {
            $(this).valid();
        });
    });
});
