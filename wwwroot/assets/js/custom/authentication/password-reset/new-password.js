"use strict";
var KTPasswordResetNewPassword = (function () {
    
  var e,
    t,
    r,
    o,
    s = function () {
      return 100 === o.getScore();
    };
  return {
    init: function () {
      (e = document.querySelector("#kt_new_password_form")),
        (t = document.querySelector("#kt_new_password_submit")),
        (o = KTPasswordMeter.getInstance(
          e.querySelector('[data-kt-password-meter="true"]')
        )),
        (r = FormValidation.formValidation(e, {
          fields: {
            password: {
              validators: {
                notEmpty: { message: "The password is required" },
                callback: {
                  message: "Please enter valid password",
                  callback: function (e) {
                    if (e.value.length > 0) return s();
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
          t.addEventListener("click", function (s) {
              var requestData = {
                  Email: e.querySelector('[name="email"]').value,
                Password: e.querySelector('[name="password"]').value,
                  ConfirmPassword: e.querySelector('[name="confirm-password"]').value,
                //get Code from url and pass it to the controller
                Code: window.location.href.split("?code=")[1]
            };
          s.preventDefault(),
            r.revalidateField("password"),
            r.validate().then(function (r) {
              "Valid" == r
                ? (t.setAttribute("data-kt-indicator", "on"),
                  (t.disabled = !0),
                  setTimeout(function () {
                    t.removeAttribute("data-kt-indicator"),
                      (t.disabled = !1),
                        $.ajax({
                            url: "/Account/ResetPassword",
                            type: "POST",
                            data: requestData,
                            xhrFields: {
                                // Disable SSL certificate verification
                                withCredentials: true,
                            },
                            success: function (data) {
                                console.log(data);
                                if (data.success) {
                                    Swal.fire({
                                        text: data.message,
                                        icon: "success",
                                        buttonsStyling: !1,
                                        confirmButtonText: "Ok, got it!",
                                        customClass: { confirmButton: "btn btn-primary" },
                                    }).then(function (e) {
                                        window.location.href = "/login";
                                    });
                                }
                                else {
                                    Swal.fire({
                                        text: data.message,
                                        icon: "error",
                                        buttonsStyling: !1,
                                        confirmButtonText: "Ok, got it!",
                                        customClass: { confirmButton: "btn btn-primary" },
                                    });
                                }
                            },
                            error: function (err) {
                                console.log(err);
                            }
                        });
                  }, 500))
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
              r.updateFieldStatus("password", "NotValidated");
          });
    },
  };
})();
KTUtil.onDOMContentLoaded(function () {
  KTPasswordResetNewPassword.init();
});
