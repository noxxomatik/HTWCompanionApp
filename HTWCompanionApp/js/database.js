var applicationData = Windows.Storage.ApplicationData.current;

function getUserGroup() {
    var localSettings = applicationData.localSettings;
    return [localSettings.values["StgJhr"], localSettings.values["Stg"], localSettings.values["StgGrp"]];
}

function setUserGroup(StJh, St, StGr) {
    var localSettings = applicationData.localSettings;
    localSettings.values["StgJhr"] = StJh;
    localSettings.values["Stg"] = St;
    localSettings.values["StgGrp"] = StGr;
}

function getUserLogin() {
    var localSettings = applicationData.localSettings;
    return [localSettings.values["sNummer"], localSettings.values["RZLogin"]];
}

function setUserLogin(sNum, RZLog) {
    var localSettings = applicationData.localSettings;
    localSettings.values["sNummer"] = sNum;
    localSettings.values["RZLogin"] = RZLog;
}

function addRoom(room) {
    var localSettings = applicationData.localSettings;
    if (room.indexOf(" ") != 1)  {
        room = room.substring(0, 1) + " " + room.substring(1)
    }
    if (localSettings.values["rooms"] == null) {
        localSettings.values["rooms"] = JSON.stringify([room]);
    }
    else {
        var array = JSON.parse(localSettings.values["rooms"]);
        array.push(room);
        localSettings.values["rooms"] = JSON.stringify(array);
    }    
}

function getAllRooms() {
    var localSettings = applicationData.localSettings;
    var rooms = localSettings.values["rooms"];
    if (rooms != null) {
        return JSON.parse(rooms);
    }
    else return null;
}