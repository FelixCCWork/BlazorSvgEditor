// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function showPrompt(message) {
  return prompt(message, 'Type anything here');
}

export function getBoundingBox(element) { return element ? element.getBoundingClientRect() : {}; }
export function getElementWidth(element) { return element ? element.clientWidth : 0; }
export function getElementHeight(element) { return element ? element.clientHeight : 0; }


export function getElementWidthAndHeight(element) {
  return { width: getElementWidth(element), height: getElementHeight(element) };
}

export function svgToBase64(baseImage, annotationsSvg, width, height) {
    return new Promise((resolve, reject) => {
        var serializer = new XMLSerializer();
        var source = serializer.serializeToString(annotationsSvg);
        // Replace the outer <g> node with a <svg> node (image.src does not work with a outermost <g> tag)
        source = source.replace(/^<g/, "<svg").replace(/<\/g>$/, "</svg>");

        var image = new Image();
        image.onload = function () {
            var canvas = document.createElement("canvas");
            canvas.width = width;
            canvas.height = height;
            var context = canvas.getContext("2d");
            // We need to set width and height here to scale all svg annotations to the same size as the base image
            context.drawImage(baseImage, 0, 0, width, height);
            context.drawImage(image, 0, 0);

            var img = canvas.toDataURL("image/png");
            resolve(img); // return the base64 string
        };

        image.onerror = function () {
            reject("Error loading image");
        };

        image.src = "data:image/svg+xml;base64," + btoa(source);
    });
}