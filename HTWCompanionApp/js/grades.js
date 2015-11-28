function getGrades(sNum, RZLog) {
    $.ajax({
        url: "https://wwwqis.htw-dresden.de/appservice/getcourses",
        method: "POST",
        data: {"sNummer": sNum, "RZLogin": RZLog},
        success: function (data) {
            var context = data[0];
            $.ajax({
                url: "https://wwwqis.htw-dresden.de/appservice/getgrades",
                method: "POST",
                data: {"sNummer": sNum, "RZLogin": RZLog, "AbschlNr": context.AbschlNr, "StgNr": context.StgNr, "POVersion": context.POVersion},
                success: updateGradesView
            })
        }
    });
}

function updateGradesView(grades) {
    $.each(grades, function (index, entry) {
        /*
        PrText
        PrNote
        Semester
        */
        $("#gradesContainer").append(entry.Semester + " " + entry.PrTxt + " " + (entry.PrNote/100) + "<br/>");
    })    
}