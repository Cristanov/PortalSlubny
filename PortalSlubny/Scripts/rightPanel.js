var buttonDiv;
var doit;
var transitionClassName = "transition-all1s";
var isClosed = false;

$(document).ready(function () {
    buttonDiv = $(".right-panel-button");

    buttonDiv.addClass(transitionClassName);

    setTimeout(ShowButton, 2000);

    // zamykanie panelu
    $("#right-panel-cross").click(function () {
        HideButton();
        isClosed = true;
    });

})

window.onresize = function () {
    clearTimeout(doit);
    doit = setTimeout(ShowButton, 2000);
    ResetButton();
};

function ShowButton() {
    if (!isClosed) {
        var windowWidth = document.body.clientWidth;

        buttonDiv.css("left", windowWidth + "px");

        buttonDiv.css("visibility", "visible");
        buttonDiv.addClass(transitionClassName);

        var buttonHeight = buttonDiv.height();
        //alert(buttonHeight);
        buttonDiv.css("left", "-=" + buttonHeight);
    }
}

function HideButton() {
    var windowWidth = document.body.clientWidth;
    buttonDiv.css("left", windowWidth + "px");
}

function ResetButton() {
    var windowWidth = document.body.clientWidth;

    buttonDiv.removeClass(transitionClassName);
    buttonDiv.css("visibility", "hidden");

    buttonDiv.css("left", windowWidth + "px");
}