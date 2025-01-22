$(document).ready(function () {
    $("#registrationForm").on("submit", function (e) {
        e.preventDefault();

        const form = $(this);
        const url = form.attr("action"); 

        form.find(".is-invalid").removeClass("is-invalid");
        form.find(".invalid-feedback").remove();

        
        $.ajax({
            url: url,
            type: "POST",
            data: form.serialize(),
            success: function (response) {
                console.log("info:", response);

                
                if (response.success) {
                    const emailInput = $("[name='Email']");
                    const usernameInput = $("[name='UserName']");
                    const passwordInput = $("[name='Password']");
                    const confirmPasswordInput = $("[name='СonfirmPassword']");

                    emailInput.addClass("successfully");
                    usernameInput.addClass("successfully");
                    passwordInput.addClass("successfully");
                    confirmPasswordInput.addClass("successfully");

                    confirmPasswordInput.after(`<div class="successfully-feedback">Ви успійшно залогінились</div>`);
                    setTimeout(function () {
                        location.href = "/Account/Login";
                    }, 1500);
                    }   
                    else {

                        if (response.message) {
                            console.log("Error message:", response.message); 

                            if (response.message.includes("User with this email already exist")) {
                                const emailInput = $("[name='Email']");
                                emailInput.addClass("is-invalid");
                                emailInput.after(`<div class="invalid-feedback">Користувач з таким email вже існує.</div>`);
                            } 
                            
                            else if (response.message.includes("This username is used")) {
                                const usernameInput = $("[name='UserName']");
                                usernameInput.addClass("is-invalid");
                                usernameInput.after(`<div class="invalid-feedback">Користувач з таким UserName вже існує.</div>`);
                            } 
                            else {
                                alert(response.message); 
                            }
                        }
                    if (response.errors && response.errors.length > 0) {
                        response.errors.forEach(function (errorMessage) {
                            console.log("Помилка сервера:", errorMessage);

                            if (errorMessage.includes("email") || errorMessage.includes("Email")) {
                                const emailInput = $("[name='Email']");
                                emailInput.addClass("is-invalid");
                                emailInput.after(`<div class="invalid-feedback">${errorMessage}</div>`);
                                
                            } 
                            else if (errorMessage.includes("username") || errorMessage.includes("UserName")) {
                                const usernameInput = $("[name='UserName']");
                                usernameInput.addClass("is-invalid");
                                usernameInput.after(`<div class="invalid-feedback">${errorMessage}</div>`);
                                
                            } 
                            else if (errorMessage.includes("Password don't matches")) {
                                const passwordInput = $("[name='Password']");
                                const confirmPasswordInput = $("[name='СonfirmPassword']");
                            
                                // Додаємо клас для підсвічування помилки
                                passwordInput.addClass("is-invalid");
                                confirmPasswordInput.addClass("is-invalid");
                            
                                // Виводимо лише одну помилку: "Паролі не співпадають"
                                confirmPasswordInput.after(`<div class="invalid-feedback">Паролі не співпадають</div>`);
                            } 

                            else if (errorMessage.includes("The Password field is required.") || errorMessage.includes("The ConfirmPassword field is required.")) {
                                const passwordInput = $("[name='Password']");
                                const confirmPasswordInput = $("[name='СonfirmPassword']");
                            
                                // Виводимо окрему помилку для відсутнього пароля або підтвердження пароля
                                if (errorMessage.includes("The Password field is required.")) {
                                    passwordInput.addClass("is-invalid");
                                    passwordInput.after(`<div class="invalid-feedback">Поле пароля є обов'язковим.</div>`);
                                }
                                if (errorMessage.includes("The ConfirmPassword field is required.")) {
                                    confirmPasswordInput.addClass("is-invalid");
                                    confirmPasswordInput.after(`<div class="invalid-feedback">Поле підтвердження пароля є обов'язковим.</div>`);
                                }
                            }else {
                                console.log("Unknown error:", errorMessage); 
                            }
                        });
                    } 
                }
            },
            error: function (xhr, status, error) {
                console.error("Ошибка AJAX-запроса:", status, error);
                alert("Щось пішло не так. Спробуйте ще раз.");
            }
        });
    });
});



$(document).ready(function () {
    $("#signInLink").on("click", function (e) {
        e.preventDefault();

        $.ajax({
            url: '/Account/Register', 
            type: 'GET',
            success: function (response) {
                window.location.href = '/Account/Login';
            },
            error: function () {
                alert("Помилка при перенаправленні.");
            }
        });
    });
});