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
    showAppBarButton("save");
}

function hideSettings() {
    setUserGroup($("#StJhInput").val(), $("#StInput").val(), $("#StGrInput").val());
    setUserLogin($("#sNumInput").val(), $("#rzLogInput").val());
    WinJS.Navigation.navigate("pages/hub/hub.html");
    hideAllAppBarButtons();
    showAppBarButton("settings");
}