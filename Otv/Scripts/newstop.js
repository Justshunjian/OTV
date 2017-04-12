var App = App || {};
App.ui = App.ui || {},

App.calendar = function () {
    var e = $(".calendar-day"),
	t = $(".calendar-week"),
	n = new Date,
	r = n.getMonth() + 1,
	i = n.getDate(),
	s = ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"];
    e.html(r + "月" + i + "日"),
	t.html(s[n.getDay()])
},

$(function () {
    App.calendar()
});