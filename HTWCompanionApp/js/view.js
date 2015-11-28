function hideAllAppBarButtons() {
    appbar.winControl.hideCommands("settings");
    appbar.winControl.hideCommands("save");
}

function showAppBarButton(id) {
    appbar.winControl.showCommands(id);
}

function addButtonHandler() {
    $("#settings").on("click", showSettings);
    $("#save").on("click", hideSettings);
}

function showSettings() {
    WinJS.Navigation.navigate("pages/settingsHub/settings.html");
    hideAllAppBarButtons();

    // TODO: zusammengehackt, sollte aufgerufen werden wenn settings.html erneut angezeigt wird
    loadSettings();

    showAppBarButton("save");
}

function loadSettings() {
    // load current settings
    var userGroup = getUserGroup();
    if (userGroup[0] != undefined &&
        userGroup[1] != undefined &&
        userGroup[2] != undefined) {
        $('#StJhInput').val(userGroup[0]);
        $('#StInput').val(userGroup[1]);
        $('#StGrInput').val(userGroup[2]);
    }

    var userLogin = getUserLogin();
    if (userLogin[0] != undefined &&
        userLogin[1] != undefined) {
        $('#sNumInput').val(userLogin[0]);
        $('#rzLogInput').val(userLogin[1]);
    }
}

function hideSettings() {
    if ($("#StJhInput").val() != undefined &&
        $("#StInput").val() != undefined &&
         $("#StGrInput").val() != undefined) {
        setUserGroup($("#StJhInput").val(), $("#StInput").val(), $("#StGrInput").val());
    }

    if ($("#sNumInput").val() != undefined &&
        $("#rzLogInput").val() != undefined) {
        setUserLogin($("#sNumInput").val(), $("#rzLogInput").val());
    }

    WinJS.Navigation.navigate("pages/hub/hub.html");
    hideAllAppBarButtons();
    showAppBarButton("settings");
}