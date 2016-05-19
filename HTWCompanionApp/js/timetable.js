function getTimetableStudent(StJh, St, StGr) {
    $.ajax({
        url: "https://www2.htw-dresden.de/~app/API/GetTimetable.php",
        data: { StgJhr: StJh, Stg: St, StgGrp: StGr},
        success: updateTimtable,
        error: function() {
            var localSettings = applicationData.localSettings;
            updateTimtable(JSON.parse(localSettings.values["backupTimetable"]));
        }
    });
}

function updateTimtable(data) {
    var selectorDay = { 1: "Mon_", 2: "Die_", 3: "Mit_", 4: "Don_", 5: "Fre_" };
    var selectorTime = { "07:30:00": "1", "09:20:00": "2", "11:10:00": "3", "13:20:00": "4", "15:10:00": "5", "17:00:00": "6", "18:40:00": "7"}
    //$("#timetable").html(data.responseText);

    /*
    lessonTag - Kürzeld:\development\htwcompanionapp\htwcompanionapp\js\timetable.js
    name - ganzer Name
    type - V Vorlesung Pr Praktikum
    week - 0 jede Woche 1 ungerade Woche 2 gerade Woche
    day - Wochentag beginnend mit 1
    beginTime - 11:10:00
    endTime - 12:40:00
    professor - Familienname
    WeeksOnly - Anzahl Wochen in denen Lehrveranstaltung stattfindet
    Rooms - Array mit Raumnummern ["S 128"]
    */

    // gerade oder ungerade Woche
    var week = (getCurrentWeekNumber() % 2) == 0 ? 2 : 1;

    $.each(data, function (index, lesson) {
        // Termin liegt in dieser Woche
        if (lesson.week == 0 || lesson.week == week) {
            // genauen Termin bestimmen
            var selector = "#" + selectorDay[lesson.day] + selectorTime[lesson.beginTime];
            // Feld füllen
            $(selector).html(lesson.type + " " + lesson.lessonTag + "<br/>" + lesson.Rooms[0]);
        }
    });

    // backup
    var localSettings = applicationData.localSettings;
    var backup = JSON.stringify(data);
    localSettings.values["backupTimetable"] = backup;

    $('#progressBar').hide();
}

function getTimetableRoom(room) {
    $.ajax({
        url: "https://www2.htw-dresden.de/~app/API/GetTimetable.php",
        data: {Room: room},
        complete: updateRooms
    });
}

function updateRooms(data) {
    var selectorDay = { 1: "roomsMon_", 2: "roomsDie_", 3: "roomsMit_", 4: "roomsDon_", 5: "roomsFre_" };
    var selectorTime = { "07:30:00": "1", "09:20:00": "2", "11:10:00": "3", "13:20:00": "4", "15:10:00": "5", "17:00:00": "6", "18:40:00": "7" }

    // gerade oder ungerade Woche
    var week = (getCurrentWeekNumber() % 2) == 0 ? 2 : 1;

    // Tabelle säubern
    $.each(selectorDay, function (index, day) {
        $.each(selectorTime, function (index, time) {
            $("#" + day + time).html("");
        })
    });

    $.each(data.responseJSON, function (index, lesson) {
        // Termin liegt in dieser Woche
        if (lesson.week == 0 || lesson.week == week) {
            // genauen Termin bestimmen
            var selector = "#" + selectorDay[lesson.day] + selectorTime[lesson.beginTime];
            // Feld füllen
            $(selector).html(lesson.type + " " + lesson.lessonTag);
        }
    });
}

function getCurrentWeekNumber() {
    var KWDatum = new Date();

    var DonnerstagDat = new Date(KWDatum.getTime() +
    (3 - ((KWDatum.getDay() + 6) % 7)) * 86400000);

    KWJahr = DonnerstagDat.getFullYear();

    var DonnerstagKW = new Date(new Date(KWJahr, 0, 4).getTime() +
    (3 - ((new Date(KWJahr, 0, 4).getDay() + 6) % 7)) * 86400000);

    var KW = Math.floor(1.5 + (DonnerstagDat.getTime() -
    DonnerstagKW.getTime()) / 86400000 / 7);

    return KW;
}