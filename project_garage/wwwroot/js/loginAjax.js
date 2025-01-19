$(document).ready(function () {
    // Обробка форми логіну
    $("#registrationForm").on("submit", function (e) {
        e.preventDefault(); // Отменяем стандартную отправку формы

        const form = $(this);
        const url = form.attr("action"); // Получаем URL из атрибута формы

        // Убираем предыдущие подсветки ошибок и сообщения
        form.find(".is-invalid").removeClass("is-invalid");
        form.find(".invalid-feedback").remove();

        // Отправляем данные через AJAX
        $.ajax({
            url: url,
            type: "POST",
            data: form.serialize(),
            success: function (response) {
                console.log("Response:", response); // Логуємо відповідь

                if (response.success) {
                    const usernameInput = $("[name='Email']");
                    const passwordInput = $("[name='Password']");
                    
                    // Додаємо клас "successfully"
                    usernameInput.addClass("successfully");
                    passwordInput.addClass("successfully");
                    passwordInput.after(`<div class="successfully-feedback">Ви успійшно залогінились</div>`);
                
                    // Перенаправляємо через 3 секунди (3000 мілісекунд)
                    setTimeout(function () {
                        location.href = "/Profile/ProfileIndex/" + response.userId; // Перенаправлення на профіль
                    }, 1500);
                }
                else {
                    // Підсвічуємо інпути при неправильному логіні або паролі
                    const usernameInput = $("[name='Email']");
                    const passwordInput = $("[name='Password']");
                    
                    usernameInput.addClass("is-invalid");
                    passwordInput.addClass("is-invalid");
                    passwordInput.after(`<div class="invalid-feedback">Неправильний логін або пароль</div>`);

                    // Виводимо повідомлення про неправильний логін або пароль
                }
            },
            error: function () {
                alert("Щось пішло не так! Спробуйте ще раз.");
            }
        });
    });
});
  // Обробка переходу на сторінку реєстрації
  $("#signUpLink").on("click", function (e) {
    e.preventDefault();

    $.ajax({
        url: '/Account/Register', 
        type: 'GET',
        success: function () {
            window.location.href = '/Account/Register'; // Перенаправлення на реєстрацію
        },
        error: function () {
            alert("Помилка при перенаправленні.");
        }
    });
});