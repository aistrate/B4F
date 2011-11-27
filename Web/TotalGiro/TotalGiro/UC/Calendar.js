function emptyCalendar(sender) {
    var parentName = sender.id.substring(0, sender.id.lastIndexOf('_') + 1);
    var txtCalendar = document.getElementById(parentName + 'txtCalendar');

    txtCalendar.value = "";
    txtCalendar.fireEvent("onchange");
}

function showStuff(sender, args) {
    var c = Sys.Application.getComponents();
    for (var i = 0; i < c.length; i++) {
        var id = c[i].get_id();
        var type = Object.getType(c[i]).getName();
        if (type == "Sys.Extended.UI.CalendarBehavior" && id != sender._id)
            c[i].hide();
    }
}
