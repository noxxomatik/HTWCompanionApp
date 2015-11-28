(function () {
    "use strict";

    var ControlConstructor = WinJS.UI.Pages.define("/pages/hub/section1Page.html", {
        // Diese Funktion wird aufgerufen, nachdem die Inhalte der Seitensteuerelemente 
        // geladen wurden, die Steuerelemente aktiviert wurden und 
        // die resultierenden Elemente dem DOM untergeordnet wurden. 
        ready: function (element, options) {
            options = options || {};
            hideAllAppBarButtons();
            showAppBarButton("settings");
            var stgroup = getUserGroup();
            if (stgroup[0] != null || stgroup[0] == "") {
                getTimetableStudent(stgroup[0], stgroup[1], stgroup[2]);
            }
        },
    });

    // Mit den folgenden Zeilen wird dieser Steuerelementkonstruktur global verfügbar gemacht. 
    // Dadurch können Sie das Steuerelement als deklaratives Steuerelement innerhalb 
    // innerhalb des "data-win-control"-Attributs verwenden. 

    WinJS.Namespace.define("HubApps_SectionControls", {
        Section1Control: ControlConstructor
    });
})();