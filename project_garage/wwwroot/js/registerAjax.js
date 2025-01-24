
$(document).ready(function () {
    $("#registrationForm").on("submit", function (e) {
        e.preventDefault(); // Останавливаем отправку формы

        const form = $(this);
        const url = form.attr("action");

        $.ajax({
            url: url,
            type: "POST",
            data: form.serialize(), 
            success: function (response) {
                if (response.success) {
                    alert("Реєстрація пройшла успішно!");
                    location.href = "/Account/Login";
                } else {
                    const errors = response.errors ? response.errors.join("\n") : "Щось пішло не так.";
                    alert(errors);
                }
            },
            error: function () {
                alert("Щось пішло не так! Спробуйте ще раз.");
            }
        });
    });
});