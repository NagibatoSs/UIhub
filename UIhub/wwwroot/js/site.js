
function newFigmaField() {
    let container = document.getElementById("layoutsFields");
    let fieldCount = container.getElementsByTagName("input").length;
    let nextFieldId = fieldCount + 1;

    let div = document.createElement("div");
    div.setAttribute("class", "form-group");

    // создаем новое поле с новым id, name ДОЛЖЕН СОВПАДАТЬ С ИМЕНЕМ ПОЛЯ В МОДЕЛИ!!!
    let field = document.createElement("input");
    field.setAttribute("class", "form-control");
    field.setAttribute("id", "VisualContents[" + nextFieldId + "]");
    field.setAttribute("name", "InterfaceLayoutsSrc");
    field.setAttribute("type", "text");
    field.setAttribute("placeholder", "Укажите ссылку на Figma-проект");
    field.setAttribute("asp-for", "InterfaceLayoutsSrc[" + nextFieldId + "]");
    div.appendChild(field);
    container.appendChild(div);
}

function newImgField() {
    let container = document.getElementById("layoutsFields");
    let fieldCount = container.getElementsByTagName("input").length;
    let nextFieldId = fieldCount + 1;

    let div = document.createElement("div");
    div.setAttribute("class", "form-group");

    // создаем новое поле с новым id, name ДОЛЖЕН СОВПАДАТЬ С ИМЕНЕМ ПОЛЯ В МОДЕЛИ!!!
    let field = document.createElement("input");
    field.setAttribute("class", "form-control");
    field.setAttribute("id", "VisualContents[" + nextFieldId + "]");
    field.setAttribute("name", "InterfaceLayoutsSrc");
    field.setAttribute("type", "file");
    field.setAttribute("placeholder", "Загрузите файл-изображения");
    field.setAttribute("asp-for", "InterfaceLayoutsSrc[" + nextFieldId + "]");
    div.appendChild(field);
    container.appendChild(div);
}
