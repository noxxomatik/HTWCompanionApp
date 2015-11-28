function getCanteenTable() {
    $.get("http://www.studentenwerk-dresden.de/feeds/speiseplan.rss?mid=9", function (data) {
        $(data).find("item").each(function () {
            var meal = $(this);
            $("#canteenContainerToday").append(meal.find("description").text() + "<br/>");
            $("#canteenContainerToday").append("<a href='" + meal.find("link").text() + "'>Link</a>" + "<br/><br/>");
        })
    });
    $.get("http://www.studentenwerk-dresden.de/feeds/speiseplan.rss?mid=9&tag=morgen", function (data) {
        $(data).find("item").each(function () {
            var meal = $(this);
            $("#canteenContainerTomorrow").append(meal.find("description").text() + "<br/>");
            $("#canteenContainerTomorrow").append("<a href='" + meal.find("link").text() + "'>Link</a>" + "<br/><br/>");
        })
    });
}