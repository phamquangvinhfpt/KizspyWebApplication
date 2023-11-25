"use strict";
var KTSignupGeneral = (function () {
    var e,
        t,
        a,
        s,
        r = function () {
            return 100 === s.getScore();
        };
    return {
        init: function () {
            (e = document.querySelector("#kt_sign_up_form")),
                (t = document.querySelector("#kt_sign_up_submit")),
                (s = KTPasswordMeter.getInstance(
                    e.querySelector('[data-kt-password-meter="true"]')
                )),
                (a = FormValidation.formValidation(e, {
                    fields: {
                        UserName: {
                            validators: {
                                notEmpty: { message: "User name is required" },
                            }
                        },
                        email: {
                            validators: {
                                notEmpty: { message: "Email address is required" },
                                emailAddress: {
                                    message: "The value is not a valid email address",
                                },
                            },
                        },
                        password: {
                            validators: {
                                notEmpty: { message: "The password is required" },
                                callback: {
                                    message: "Please enter valid password",
                                    callback: function (e) {
                                        if (e.value.length > 0) return r();
                                    },
                                },
                            },
                        },
                        "confirm-password": {
                            validators: {
                                notEmpty: { message: "The password confirmation is required" },
                                identical: {
                                    compare: function () {
                                        return e.querySelector('[name="password"]').value;
                                    },
                                    message: "The password and its confirm are not the same",
                                },
                            },
                        },
                        toc: {
                            validators: {
                                notEmpty: {
                                    message: "You must accept the terms and conditions",
                                },
                            },
                        },
                    },
                    plugins: {
                        trigger: new FormValidation.plugins.Trigger({
                            event: { password: !1 },
                        }),
                        bootstrap: new FormValidation.plugins.Bootstrap5({
                            rowSelector: ".fv-row",
                            eleInvalidClass: "",
                            eleValidClass: "",
                        }),
                    },
                })),
                t.addEventListener("click", function (r) {
                    var model = {
                        UserName: e.querySelector('[name="UserName"]').value,
                        email: e.querySelector('[name="email"]').value,
                        password: e.querySelector('[name="password"]').value,
                        confirmPassword: e.querySelector('[name="confirm-password"]').value,
                        returnUrl: "/",
                    }
                    r.preventDefault(),
                        a.revalidateField("password"),
                        a.validate().then(function (a) {
                            "Valid" == a
                                ? (t.setAttribute("data-kt-indicator", "on"),
                                    (t.disabled = !0),
                                    setTimeout(function () {
                                        t.removeAttribute("data-kt-indicator"),
                                            (t.disabled = !1),
                                            //Swal.fire({
                                            //    text: "You have successfully reset your password!",
                                            //    icon: "success",
                                            //    buttonsStyling: !1,
                                            //    confirmButtonText: "Ok, got it!",
                                            //    customClass: { confirmButton: "btn btn-primary" },
                                            //}).then(function (t) {
                                            //    t.isConfirmed && (e.reset(), s.reset());
                                            //});
                                            $.ajax({
                                                type: "POST",
                                                url: "/Account/Register",
                                                data: model,
                                                success: function (response) {
                                                    if (response.success) {
                                                        Swal.fire({
                                                            text: "You have successfully registered!",
                                                            icon: "success",
                                                            buttonsStyling: !1,
                                                            confirmButtonText: "Ok, got it!",
                                                            customClass: { confirmButton: "btn btn-primary" },
                                                        }).then(function (t) {
                                                            //reload page
                                                            window.location.href = response.returnUrl;
                                                        });
                                                    } else {
                                                        Swal.fire({
                                                            text: response.message,
                                                            icon: "error",
                                                            buttonsStyling: !1,
                                                            confirmButtonText: "Ok, got it!",
                                                            customClass: { confirmButton: "btn btn-primary" },
                                                        });
                                                    }
                                                },
                                                    error: function (response) {
                                                        Swal.fire({
                                                            text: "Sorry, looks like there are some errors detected, please try again.",
                                                            icon: "error",
                                                            buttonsStyling: !1,
                                                            confirmButtonText: "Ok, got it!",
                                                            customClass: { confirmButton: "btn btn-primary" },
                                                    });
                                                }
                                            });
                                    }, 1500))
                                : Swal.fire({
                                    text: "Sorry, looks like there are some errors detected, please try again.",
                                    icon: "error",
                                    buttonsStyling: !1,
                                    confirmButtonText: "Ok, got it!",
                                    customClass: { confirmButton: "btn btn-primary" },
                                });
                        });
                }),
                e
                    .querySelector('input[name="password"]')
                    .addEventListener("input", function () {
                        this.value.length > 0 &&
                            a.updateFieldStatus("password", "NotValidated");
                    });
        },
    };
})();
KTUtil.onDOMContentLoaded(function () {
    KTSignupGeneral.init();
});
