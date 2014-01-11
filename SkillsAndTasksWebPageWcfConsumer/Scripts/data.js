function error(html) {
    $('#data').html(html);
    title('Umiejętności i zadania');
}
function data(html) {
    $('#data').html(html);
}
function title(text) {
    $('#titleText').text(text);
}
function getSkills() {
    try
    {
        $.ajax({
            type: 'POST',
            url: '/Data/Skills',
            data:
            {
                filter: $('#filter').val()
            },
            async: false,
            beforeSend: function ()
            {
                loadingStart();
                title('Umiejętności');
            },
            success: function (response)
            {
                if (response.result) {
                    var html = '<table>';
                    var row = '<tr style="background-color: #{0};"><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>';
                    html += row.format('5C87B2', 'LP', 'Id', 'Nazwa', 'Opis');
                    $.each(response.data, function (index, skill) {
                        html += row.format((index %2 == 0 ? 'eee' : 'fff'), index+1, skill.Id, skill.Name, skill.Description);
                    });

                    html += '</html>';
                    data(html);
                    loadingStop();
                }
                else {
                    loadingStop();
                    error(response.error);
                }
            },
            error: function (xhr, status, message)
            {
                loadingStop();
                error(message);
            }
        });
    }
    catch (e) {
        data(e.message);
}
};

function getTasks() {
    try {
        $.ajax({
            type: 'POST',
            url: '/Data/Tasks',
            data:
            {
                filter: $('#filter').val()
            },
            async: false,
            beforeSend: function () {
                loadingStart();
                title('Zadania');
            },
            success: function (response) {
                if (response.result) {
                    var html = '<table>';
                    var row = '<tr style="background-color: #{0};"><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td><td>{9}</td><td>{10}</td><td>{11}</td></tr>';
                    html += row.format('5C87B2', 'LP', 'Id', 'Nazwa', 'Opis', 'Status', 'Czy skończone', 'Data utworzenia' ,'Utworzone przez', 'Przypisane do', 'Data modyfikacji', 'Modyfikacja przez');
                    $.each(response.data, function (index, task)
                    {
                        html += row.format(
                            (index % 2 == 0 ? 'eee' : 'fff'),
                            index + 1,
                            task.UserTask.Id,
                            task.UserTask.TaskName,
                            task.UserTask.Description,
                            task.UserTask.Status,
                            task.UserTask.IsFinished ? "Tak" : "Nie",
                            task.UserCreate != null ? task.DateCreate : "",
                            task.UserCreate != null ? task.UserCreate : "",
                            task.UserAssigned != null ? task.UserAssigned : "nie jest przypisane",
                            task.UserModification != null ? task.DateModification : "",
                            task.UserModification != null ? task.UserModification : "nikt nie modyfikował"
                            );
                    });

                    html += '</html>';
                    data(html);
                    loadingStop();
                }
                else {
                    loadingStop();
                    error(response.error);
                }
            },
            error: function (xhr, status, message) {
                loadingStop();
                error(message);
            }
        });
    }
    catch (e) {
        data(e.message);
    }
};

function getUsers() {
    try {
        $.ajax({
            type: 'POST',
            url: '/Data/Users',
            data:
            {
                filter: $('#filter').val()
            },
            async: false,
            beforeSend: function () {
                loadingStart();
                title('Użytkownicy');
            },
            success: function (response) {
                if (response.result) {
                    var html = '<table>';
                    var row = '<tr style="background-color: #{0};"><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td></tr>';
                    html += row.format('5C87B2', 'LP', 'Login', 'Imię', 'Nazwisko', 'Miasto', 'Mail', 'Telefon');
                    $.each(response.data, function (index, user) {
                        html += row.format(
                            (index % 2 == 0 ? 'eee' : 'fff'),
                            index + 1,
                            user.Login,
                            user.Name,
                            user.Surname,
                            user.Town,
                            user.Mail,
                            user.Phone
                            );
                    });

                    html += '</html>';
                    data(html);
                    loadingStop();
                }
                else {
                    loadingStop();
                    error(response.error);
                }
            },
            error: function (xhr, status, message) {
                loadingStop();
                error(message);
            }
        });
    }
    catch (e) {
        data(e.message);
    }
};

function getUS()
{
    try {
        $.ajax({
            type: 'POST',
            url: '/Data/UserSkills',
            data:
            {
                filter: $('#filter').val()
            },
            async: false,
            beforeSend: function () {
                loadingStart();
                title('Umiejętności użytkowników');
            },
            success: function (response) {
                if (response.result) {
                    var html = '<table>';
                    var row = '<tr style="background-color: #{0};"><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td><td>{9}</td></tr>';
                    html += row.format('5C87B2', 'LP', 'Umiejętność', 'Opis umiejętności', 'Login', 'Imię', 'Nazwisko', 'Miasto', 'Mail', 'Telefon');
                    $.each(response.data, function (index, us) {
                        html += row.format(
                            (index % 2 == 0 ? 'eee' : 'fff'),
                            index + 1,
                            us.Skill.Name,
                            us.Skill.Description,
                            us.User.Login,
                            us.User.Name,
                            us.User.Surname,
                            us.User.Town,
                            us.User.Mail,
                            us.User.Phone
                            );
                    });

                    html += '</html>';
                    data(html);
                    loadingStop();
                }
                else {
                    loadingStop();
                    error(response.error);
                }
            },
            error: function (xhr, status, message) {
                loadingStop();
                error(message);
            }
        });
    }
    catch (e) {
        data(e.message);
    }
};

$(function () {
    loadingStop();
    setTimeout(function () {
        getSkills();
    }, 5000);
});