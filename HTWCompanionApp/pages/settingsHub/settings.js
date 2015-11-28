(function () {
    "use strict";

    var ControlConstructor = WinJS.UI.Pages.define("/pages/settingsHub/settings.html", {
        // Diese Funktion wird aufgerufen, nachdem die Inhalte der Seitensteuerelemente 
        // geladen wurden, die Steuerelemente aktiviert wurden und 
        // die resultierenden Elemente dem DOM untergeordnet wurden. 
        ready: function (element, options) {
            options = options || {};
            loadSettings();
        },        
    });
})();