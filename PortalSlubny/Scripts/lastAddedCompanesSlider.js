var position = 1;
var columnWidth;

$(document).ready(function () {
    

    Initialize();

    setInterval(function () {
        slide();
    }, 3000);

});

$(window).resize(function () {
    Initialize();
});

function Initialize() {
    columnWidth = $("#lastAddedCompaniesSliderContainer").width();
    var lastAddedCompanyWidth = $(".lastAddedCompany").length * columnWidth;
    $("#lastAddedCompanies").width(lastAddedCompanyWidth);

    var lastAddedCompanies = document.getElementsByClassName("lastAddedCompany");
    var imgs = document.getElementsByClassName("lastAddedCompanyImg");

    // centruje i skaluje zdjęcia w sliderze
    for (var i = 0; i < lastAddedCompanies.length; i++) {
        lastAddedCompanies[i].style.width = columnWidth + "px";
        ScaleImageToContainer(imgs[i], lastAddedCompanies[i]);
        CenterImageInContainer(imgs[i], lastAddedCompanies[i]);
    }
}

function slide() {
    var distance = position * columnWidth;
    $("#lastAddedCompanies").animate({ left: '-' + distance.toString() + 'px' }, "slow");
    updatePosition();
}

function updatePosition() {
    var companiesCount = $(".lastAddedCompany").length;
    if (position < companiesCount - 1)
        position++;
    else
        position = 0;
}