"use strict";
var KTSigninGeneral = (function () {
  var t, e, i;
  return {
    init: function () {
      (t = document.querySelector("#kt_sign_in_form")),
        (e = document.querySelector("#kt_sign_in_submit")),
        (i = FormValidation.formValidation(t, {
          fields: {
            email: {
              validators: {
                notEmpty: { message: "Email address is required" },
                // emailAddress: {
                //   message: "The value is not a valid email address",
                // },
              },
            },
            password: {
              validators: { notEmpty: { message: "The password is required" } },
            },
          },
          plugins: {
            trigger: new FormValidation.plugins.Trigger(),
            bootstrap: new FormValidation.plugins.Bootstrap5({
              rowSelector: ".fv-row",
            }),
          },
        })),
        e.addEventListener("click", function (n) {
            var requestData = {
                UserNameOrEmail: t.querySelector('[name="email"]').value,
                Password: t.querySelector('[name="password"]').value,
                RememberMe: t.querySelector('[name="remember"]').value,
                returnUrl: "/",
            };
          n.preventDefault(),
            i.validate().then(function (i) {
              "Valid" == i
                ? (e.setAttribute("data-kt-indicator", "on"),
                  (e.disabled = !0),
                  setTimeout(function () {
                    e.removeAttribute("data-kt-indicator"),
                      (e.disabled = !1),
                    //   Swal.fire({
                    //     text: "You have successfully logged in!",
                    //     icon: "success",
                    //     buttonsStyling: !1,
                    //     confirmButtonText: "Ok, got it!",
                    //     customClass: { confirmButton: "btn btn-primary" },
                    //   }).then(function (e) {
                    //     e.isConfirmed &&
                    //       ((t.querySelector('[name="email"]').value = ""),
                    //       (t.querySelector('[name="password"]').value = ""));
                    //   });
                        $.ajax({
                            url: "/login",
                            type: "POST",
                            data: requestData,
                            xhrFields: {
                                // Disable SSL certificate verification
                                rejectUnauthorized: false
                            },
                            console: console.log(requestData),
                            //console log data
                            success: function (res) {
                                console.log(res);
                                if (res.success) {
                                    // Đăng nhập thành công
                                    console.log(res);
                                    Swal.fire({
                                        text: "You have successfully logged in!",
                                        icon: "success",
                                        buttonsStyling: false,
                                        confirmButtonText: "Ok, got it!",
                                        customClass: { confirmButton: "btn btn-primary" },
                                    }).then(function (e) {
                                        // Chuyển hướng đến returnUrl
                                        window.location.href = res.returnUrl;
                                    });
                                } else {
                                    if (res.requiresTwoFactor) {
                                        // Yêu cầu xác thực hai yếu tố
                                        // Chuyển hướng đến hành động SendCode
                                        window.location.href = "/Account/SendCode?returnUrl=" + encodeURIComponent(res.returnUrl) + "&rememberMe=" + res.rememberMe;
                                    } else if (res.isLockedOut) {
                                        // Tài khoản bị khóa
                                        console.log(res);
                                        Swal.fire({
                                            text: "Sorry, this account has been locked out.",
                                            icon: "error",
                                            buttonsStyling: false,
                                            confirmButtonText: "Ok, got it!",
                                            customClass: { confirmButton: "btn btn-primary" },
                                        });
                                    } else {
                                        console.log(res);
                                        Swal.fire({
                                            text: "Sorry, your username or password is incorrect, please try again.",
                                            icon: "error",
                                            buttonsStyling: false,
                                            confirmButtonText: "Ok, got it!",
                                            customClass: { confirmButton: "btn btn-primary" },
                                        });
                                    }
                                }
                            },
                            error: function (err) {
                                Swal.fire({
                                    text: "Sorry, looks like there are some errors detected, please try again.",
                                    icon: "error",
                                    buttonsStyling: false,
                                    confirmButtonText: "Ok, got it!",
                                    customClass: { confirmButton: "btn btn-primary" },
                                });
                            }
                        });
                  }, 2e3))
                : Swal.fire({
                    text: "Sorry, looks like there are some errors detected, please try again.",
                    icon: "error",
                    buttonsStyling: !1,
                    confirmButtonText: "Ok, got it!",
                    customClass: { confirmButton: "btn btn-primary" },
                  });
            });
        });
    },
  };
})();
KTUtil.onDOMContentLoaded(function () {
  KTSigninGeneral.init();
});
