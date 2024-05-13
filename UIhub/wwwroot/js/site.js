﻿
function newFigmaField() {
    let container = document.getElementById("layoutsFields");
    let fieldCount = container.getElementsByTagName("input").length;
    let nextFieldId = fieldCount + 1;

    let div = document.createElement("div");
    div.setAttribute("class", "form-group");

    // создаем новое поле с новым id, name ДОЛЖЕН СОВПАДАТЬ С ИМЕНЕМ ПОЛЯ В МОДЕЛИ!!!
    let field = document.createElement("input");
    field.setAttribute("class", "form-control");
    field.setAttribute("id", "InterfaceLayoutsSrc[" + nextFieldId + "]");
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
    field.setAttribute("id", "InterfaceLayoutsSrc[" + nextFieldId + "]");
    field.setAttribute("name", "InterfaceLayoutsSrc");
    field.setAttribute("type", "file");
    field.setAttribute("placeholder", "Загрузите файл-изображения");
    field.setAttribute("asp-for", "InterfaceLayoutsSrc[" + nextFieldId + "]");
    div.appendChild(field);
    container.appendChild(div);
}

function newEstimateFields(button_id) {
    document.getElementById("formatContent").innerHTML = "";
    switch (button_id) {
        case 'scale':
            newScaleFields();
            break;
        case 'voting':
            newVotingFields();
            break;
        case 'ranging':
            newRangingFields();
            break;
    }
}

function newScaleFields() {
    let container = document.getElementById("formatContent");

    container.innerHTML = '<div class="col-auto"> <p>Добавить шкалу</p> </div>'
    container.innerHTML += '<div class="col-auto"> <button type="button" id="newScaleBtn" onclick="newScale()">+</button> </div>' 

    let div = document.createElement("div");
    div.setAttribute("class", "form-group");

    let field = document.createElement("input");
    field.setAttribute("class", "form-control");
    field.setAttribute("id", "EstimatesScale[0]");
    //field.setAttribute("name", "Estimates[1].Characteristic");
    field.setAttribute("name", "EstimatesScale[0].Characteristic");
    field.setAttribute("type", "text");
    field.setAttribute("placeholder", "Характеристика");
    field.setAttribute("asp-for", "EstimatesScale[0].Characteristic");
    div.appendChild(field);
    container.appendChild(div);
}
function printSmt(data) {
    console.log(data);
}

function newScale() {
    let container = document.getElementById("formatContent");
    let fieldCount = container.getElementsByTagName("input").length;
    let nextFieldId = fieldCount;
    if (fieldCount == 9) {
        document.getElementById("newScaleBtn").setAttribute('disabled', true);
    }

    let div = document.createElement("div");
    div.setAttribute("class", "form-group");

    let field = document.createElement("input");
    field.setAttribute("class", "form-control");
    field.setAttribute("id", "EstimatesScale[" + nextFieldId + "]");
    field.setAttribute("name", "EstimatesScale[" + nextFieldId + "].Characteristic");
    field.setAttribute("type", "text");
    field.setAttribute("placeholder", "Характеристика");
    field.setAttribute("asp-for", "EstimatesScale["+ nextFieldId +"].Characteristic");
    div.appendChild(field);
    container.appendChild(div);

}

function newVotingFields() {
    let container = document.getElementById("formatContent");
    container.innerHTML = '<div class="col-auto"> <p>Добавить голосование</p> </div>'
    container.innerHTML += '<div class="col-auto"> <button type="button" id="newVotingBtn" onclick="newVoting()">+</button> </div>'

    let divBlock = document.createElement("div");
    divBlock.setAttribute("id", "block0");

    let divRow = createDiv("row")
    let divCol = createDiv("col-auto")
    let field = createTextInput("EstimatesVoting[0].Characteristic", "Характеристика", "form-control vote")
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    divCol = createDiv("col-auto")
    field = document.createElement("p");
    field.innerHTML = "<p>Добавить вариант ответа</p>";
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    divCol = createDiv("col-auto addBtn")
    field = document.createElement("button");
    field.setAttribute("type", "button");
    field.innerHTML = "+";
    field.setAttribute("onclick", "newVoteVariant(0)");
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    field = createTextInput("EstimatesVoting[0].VotingObjects[0].Content", "Вариант ответа", "form-control voteObj");
    div = createDiv("form-group");
    div.appendChild(field);
    divBlock.appendChild(div);

    field = createTextInput("EstimatesVoting[0].VotingObjects[1].Content", "Вариант ответа", "form-control voteObj");
    div = createDiv("form-group");
    div.appendChild(field);
    divBlock.appendChild(div);

    container.appendChild(divBlock);
}

function createTextInput(name, placeholder, fieldClass) {
    field = document.createElement("input");
    field.setAttribute("class", fieldClass);
    field.setAttribute("id", name);
    field.setAttribute("name", name);
    field.setAttribute("type", "text");
    field.setAttribute("placeholder", placeholder);
    field.setAttribute("asp-for", name);
    return field;
}

function createDiv(divClass){
    let div = document.createElement("div");
    div.setAttribute("class", divClass);
    return div;
}

function newVoteVariant(voteId) {
    let block = document.getElementById("block" + voteId);
    let voteObjCount = block.getElementsByClassName("voteObj").length;
    if (voteObjCount == 9) {
        block.getElementsByTagName("button")[0].disabled = true;
    }
    let div = createDiv("form-group")
    let field = createTextInput("EstimatesVoting[" + voteId + "].VotingObjects[" + voteObjCount + "].Content", "Вариант ответа", "form-control voteObj")
    div.appendChild(field);
    block.appendChild(div);
}

function newVoting() {
    let container = document.getElementById("formatContent");
    let voteCount = container.getElementsByClassName("vote").length;
    if (voteCount == 9) {
        document.getElementById("newVotingBtn").setAttribute('disabled', true);
    }
    let divBlock = document.createElement("div");
    divBlock.setAttribute("id", "block" + voteCount);

    let divRow = createDiv("row");
    let divCol = createDiv("col-auto");
    let field = createTextInput("EstimatesVoting[" + voteCount + "].Characteristic", "Характеристика", "form-control vote")
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    divCol = createDiv("col-auto");
    field = document.createElement("p");
    field.innerHTML = "<p>Добавить вариант ответа</p>";
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    divCol = createDiv("col-auto addBtn");
    field = document.createElement("button");
    field.setAttribute("type", "button");
    field.innerHTML = "+";
    field.setAttribute("onclick", "newVoteVariant(" + voteCount + ")");
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);
   
    let div = createDiv("form-group");
    field = createTextInput("EstimatesVoting[" + voteCount + "].VotingObjects[0].Content", "Вариант ответа", "form-control voteObj");
    div.appendChild(field);
    divBlock.appendChild(div);

    div = createDiv("form-group");
    field = createTextInput("EstimatesVoting[" + voteCount + "].VotingObjects[1].Content", "Вариант ответа", "form-control voteObj");
    div.appendChild(field);
    divBlock.appendChild(div);

    container.appendChild(divBlock);
}

function newRangingFields() {
    let container = document.getElementById("formatContent");
    container.innerHTML = '<div class="col-auto"> <p>Добавить ранжирование</p> </div>'
    container.innerHTML += '<div class="col-auto"> <button type="button" id="newRangingBtn" onclick="newRanging()">+</button> </div>'

    let divBlock = document.createElement("div");
    divBlock.setAttribute("id", "block0");

    let divRow = createDiv("row")
    let divCol = createDiv("col-auto")
    let field = createTextInput("EstimatesRanging[0].Characteristic", "Характеристика для ранжирования", "form-control vote")
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    divCol = createDiv("col-auto")
    field = document.createElement("p");
    field.innerHTML = "<p>Добавить объект ранжирования</p>";
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    divCol = createDiv("col-auto addBtn")
    field = document.createElement("button");
    field.setAttribute("type", "button");
    field.innerHTML = "+";
    field.setAttribute("onclick", "newRangeObj(0)");
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    field = createTextInput("EstimatesRanging[0].RangingObjects[0].Content", "Объект ранжирования", "form-control voteObj");
    div = createDiv("form-group");
    div.appendChild(field);
    divBlock.appendChild(div);

    field = createTextInput("EstimatesRanging[0].RangingObjects[1].Content", "Объект ранжирования", "form-control voteObj");
    div = createDiv("form-group");
    div.appendChild(field);
    divBlock.appendChild(div);

    field = createTextInput("EstimatesRanging[0].RangingObjects[2].Content", "Объект ранжирования", "form-control voteObj");
    div = createDiv("form-group");
    div.appendChild(field);
    divBlock.appendChild(div);

    container.appendChild(divBlock);
}
function newRangeObj(rangeId) {
    let block = document.getElementById("block" + rangeId);
    let rangeObjCount = block.getElementsByClassName("voteObj").length;
    if (rangeObjCount == 9) {
        block.getElementsByTagName("button")[0].disabled = true;
    }
    let div = createDiv("form-group")
    let field = createTextInput("EstimatesRanging[" + rangeId + "].RangingObjects[" + rangeObjCount + "].Content", "Объект ранжирования", "form-control voteObj")
    div.appendChild(field);
    block.appendChild(div);
}

function newRanging() {
    let container = document.getElementById("formatContent");
    let rangeCount = container.getElementsByClassName("vote").length;
    if (rangeCount == 9) {
        document.getElementById("newRangingBtn").setAttribute('disabled', true);
    }
    let divBlock = document.createElement("div");
    divBlock.setAttribute("id", "block" + rangeCount);

    let divRow = createDiv("row");
    let divCol = createDiv("col-auto");
    let field = createTextInput("EstimatesRanging[" + rangeCount + "].Characteristic", "Характеристика для ранжирования", "form-control vote")
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    divCol = createDiv("col-auto");
    field = document.createElement("p");
    field.innerHTML = "<p>Добавить объект ранжирования</p>";
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    divCol = createDiv("col-auto addBtn");
    field = document.createElement("button");
    field.setAttribute("type", "button");
    field.innerHTML = "+";
    field.setAttribute("onclick", "newRangeObj(" + rangeCount + ")");
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    let div = createDiv("form-group");
    field = createTextInput("EstimatesRanging[" + rangeCount + "].RangingObjects[0].Content", "Объект ранжирования", "form-control voteObj");
    div.appendChild(field);
    divBlock.appendChild(div);

    div = createDiv("form-group");
    field = createTextInput("EstimatesRanging[" + rangeCount + "].RangingObjects[1].Content", "Объект ранжирования", "form-control voteObj");
    div.appendChild(field);
    divBlock.appendChild(div);

    div = createDiv("form-group");
    field = createTextInput("EstimatesRanging[" + rangeCount + "].RangingObjects[2].Content", "Объект ранжирования", "form-control voteObj");
    div.appendChild(field);
    divBlock.appendChild(div);

    container.appendChild(divBlock);
}

function selectRadio(radioNumber) {
    result = document.getElementById("scaleResult");
    //result.setAttribute("name", "EstimatesScale["+0+"].Count");
    //result.setAttribute("asp-for", "");
    result.setAttribute("value", radioNumber);
}

