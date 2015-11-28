(function () {
    "use strict";

    var ControlConstructor = WinJS.UI.Pages.define("/pages/hub/section2Page.html", {
        // Diese Funktion wird aufgerufen, nachdem die Inhalte der Seitensteuerelemente 
        // geladen wurden, die Steuerelemente aktiviert wurden und 
        // die resultierenden Elemente dem DOM untergeordnet wurden. 
        ready: function (element, options) {
            options = options || {};
            hideAllAppBarButtons();
            showAppBarButton("settings");

            $("#add").on("click", function () {
                $("#selectRoom").hide();
                $("#add").hide();
                $("#inputRoom").show();
                $("#inputRoom").focus().blur(function () {
                    $("#selectRoom").show();
                    $("#add").show();
                    $("#inputRoom").hide();
                    $("#inputRoom").val("");
                });
            });

            $("#formRoom").on("submit", function () {
                event.preventDefault();
                addRoom($("#inputRoom").val());

                var rooms = getAllRooms();
                $.each(rooms, function (index, room) {
                    $("#selectRoom").append(
                        "<option value='" + room + "'>" + room + "</option>"
                        );
                });

                $("#selectRoom").show();
                $("#add").show();
                $("#inputRoom").hide();
                $("#inputRoom").val("");

                getTimetableRoom(selectRoom.value);
            });

            var rooms = getAllRooms();
            if (rooms != null) {
                $.each(rooms, function (index, room) {
                    $("#selectRoom").append(
                        "<option value='" + room + "'>" + room + "</option>"
                        );
                });
            }            
            if (selectRoom.value != "") {
                getTimetableRoom(selectRoom.value);
            }
        },
    });

    // Mit den folgenden Zeilen wird dieser Steuerelementkonstruktur global verfügbar gemacht. 
    // Dadurch können Sie das Steuerelement als deklaratives Steuerelement innerhalb 
    // innerhalb des "data-win-control"-Attributs verwenden. 

    WinJS.Namespace.define("HubApps_SectionControls", {
        Section2Control: ControlConstructor
    });
})();