// funkcja ustawia maksymalny rozmiar obrazka w kontenerze nie zniekształcając go
function ScaleImageToContainer(img, container) {
    var imgHeight = eval(img).height;
    var imgWidth = eval(img).width;
    var conHeight = container.offsetHeight;
    var conWidth = container.offsetWidth;
    var res = GetSize(imgHeight, imgWidth, conHeight, conWidth);
    var Height = res[0];
    var Width = res[1];
    img.style.height = Height + "px";
    img.style.width = Width + "px";
}

//funkcja zwraca rozmiary obrazka maksymalne dla rozmiarów kontenera
function GetSize(imgH, imgW, conH, conW) {
    var result = Array();
    if (imgH >= conH || imgW >= conW) {
        var ratio1 = conH / imgH;
        var ratio2 = conW / imgW;
        var ratio;
        if (ratio1 <= ratio2) {
            ratio = ratio1;
        } else {
            ratio = ratio2;
        }
        var Height = (imgH * ratio);
        var Width = (imgW * ratio);
        result[0] = Height;
        result[1] = Width;
    } else {
        result[0] = imgH;
        result[1] = imgW;
    }
    return result;
}

//funkcja wyśrodkowywuje obraz w kontenerze
function CenterImageInContainer(img, container) {
    var imgHeight = eval(img).height;
    var imgWidth = eval(img).width;
    var conHeight = container.offsetHeight;
    var conWidth = container.offsetWidth;

    var res = GetSize(imgHeight, imgWidth, conHeight, conWidth);
    imgHeight = res[0];
    imgWidth = res[1];
    if (conHeight >= imgHeight) {
        var margT = (conHeight - imgHeight) / 2;
        img.style.marginTop = margT + "px";
    }
    if (conWidth >= imgWidth) {
        var margL = (conWidth - imgWidth) / 2;
        img.style.marginLeft = margL + "px";
    }
}
