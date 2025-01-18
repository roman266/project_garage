$(document).ready(function () {
    $("#registrationForm").on("submit", function (e) {
        e.preventDefault(); // Останавливаем отправку формы

        const form = $(this);
        const url = form.attr("action");

        $.ajax({
            url: url,
            type: "POST",
            data: form.serialize(), // Отправка данных формы
            success: function (response) {
                if (response.success) {
                    alert(response.message);

                    // Перенаправление на профиль, используя userId
                    location.href = "/Profile/ProfileIndex/" + response.userId;
                } else {
                    alert(response.message || "Неправильний логін або пароль.");
                }
            },
            error: function () {
                alert("Щось пішло не так! Спробуйте ще раз.");
            }
        });
    });
});