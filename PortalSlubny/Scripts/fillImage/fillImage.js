$(document).ready(function () {
    FillImages();
})

$(window).resize(function () {
    FillImages();
});

function FillImages() {
    $('.fill-container').each(function () {

        var img = $(this).find('img');
        var imgWidth = img.width();
        var imgHeight = img.height();
        var conWidth = $(this).width();
        var conHeight = $(this).height();

        var newImgHeight, newImgWidth;

        if (imgWidth / conWidth <= imgHeight / conHeight) {
            img.width(conWidth);
            newImgHeight = conWidth * imgHeight / imgWidth;
            img.height(newImgHeight);
        } else {
            img.height(conHeight);
            newImgWidth = conHeight * imgWidth / imgHeight;
            img.width(newImgWidth);
        }

        if (conWidth < newImgWidth) {
            var left = -(newImgWidth - conWidth) / 2;
            img.css({ left: left });
        }
        if (conHeight < newImgHeight) {
            var top = -(newImgHeight - conHeight) / 2;
            img.css({ top: top });
        }

    })
}