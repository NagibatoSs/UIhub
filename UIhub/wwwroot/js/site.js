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

    let divRow = document.createElement("div");
    divRow.setAttribute("class", "row");

    let divCol = document.createElement("div");
    divCol.setAttribute("class", "col-auto");
    let field = document.createElement("input");
    field.setAttribute("class", "form-control vote");
    field.setAttribute("id", "EstimatesVoting[0]");
    field.setAttribute("name", "EstimatesVoting[0].Characteristic");
    field.setAttribute("type", "text");
    field.setAttribute("placeholder", "Характеристика");
    field.setAttribute("asp-for", "EstimatesVoting[0].Characteristic");
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    divCol = document.createElement("div");
    divCol.setAttribute("class", "col-auto");
    field = document.createElement("p");
    field.innerHTML = "<p>Добавить вариант ответа</p>";
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);


    divCol = document.createElement("div");
    divCol.setAttribute("class", "col-auto addBtn");
    field = document.createElement("button");
    field.setAttribute("type", "button");
    field.innerHTML = "+";
    field.setAttribute("onclick", "newVoteVariant(0)");
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);


    let div = document.createElement("div");
    div.setAttribute("class", "form-group");
    field = document.createElement("input");
    field.setAttribute("class", "form-control voteObj");
    field.setAttribute("id", "EstimatesVoting[0].VotingObjects[0]");
    field.setAttribute("name", "EstimatesVoting[0].VotingObjects[0].Content");
    field.setAttribute("type", "text");
    field.setAttribute("placeholder", "Вариант ответа");
    field.setAttribute("asp-for", "EstimatesVoting[0].VotingObjects[0].Content");
    div.appendChild(field);
    divBlock.appendChild(div);

    div = document.createElement("div");
    div.setAttribute("class", "form-group");
    field = document.createElement("input");
    field.setAttribute("class", "form-control voteObj");
    field.setAttribute("id", "EstimatesVoting[0].VotingObjects[1]");
    field.setAttribute("name", "EstimatesVoting[0].VotingObjects[1].Content");
    field.setAttribute("type", "text");
    field.setAttribute("placeholder", "Вариант ответа");
    field.setAttribute("asp-for", "EstimatesVoting[0].VotingObjects[1].Content");
    div.appendChild(field);
    divBlock.appendChild(div);

    container.appendChild(divBlock);
}

function newVoteVariant(voteId) {
    let container = document.getElementById("formatContent");
    let block = document.getElementById("block" + voteId);
    let voteObjCount = block.getElementsByClassName("voteObj").length;
    if (voteObjCount == 9) {
        block.getElementsByTagName("button")[0].disabled = true;
    }
    let div = document.createElement("div");
    div.setAttribute("class", "form-group");
    field = document.createElement("input");
    field.setAttribute("class", "form-control voteObj");
    field.setAttribute("id", "Estimates[" + voteId + "].VotingObjects[" + voteObjCount + "]");
    field.setAttribute("name", "EstimatesVoting[" + voteId + "].VotingObjects[" + voteObjCount + "].Content");
    field.setAttribute("type", "text");
    field.setAttribute("placeholder", "Вариант ответа");
    field.setAttribute("asp-for", "EstimatesVoting[" + voteId + "].VotingObjects[" + voteObjCount + "].Content");
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
    divBlock.setAttribute("id", "block"+voteCount);

    let divRow = document.createElement("div");
    divRow.setAttribute("class", "row");

    let divCol = document.createElement("div");
    divCol.setAttribute("class", "col-auto");
    let field = document.createElement("input");
    field.setAttribute("class", "form-control vote");
    field.setAttribute("id", "EstimatesVoting[" + voteCount + "]");
    field.setAttribute("name", "EstimatesVoting[" + voteCount + "].Characteristic");
    field.setAttribute("type", "text");
    field.setAttribute("placeholder", "Характеристика");
    field.setAttribute("asp-for", "EstimatesVoting[" + voteCount + "].Characteristic");
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    divCol = document.createElement("div");
    divCol.setAttribute("class", "col-auto");
    field = document.createElement("p");
    field.innerHTML = "<p>Добавить вариант ответа</p>";
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);

    divCol = document.createElement("div");
    divCol.setAttribute("class", "col-auto addBtn");
    field = document.createElement("button");
    field.setAttribute("type", "button");
    field.innerHTML = "+";
    field.setAttribute("onclick", "newVoteVariant(" + voteCount + ")");
    divCol.appendChild(field);
    divRow.appendChild(divCol);
    divBlock.appendChild(divRow);
   

    let div = document.createElement("div");
    div.setAttribute("class", "form-group");
    field = document.createElement("input");
    field.setAttribute("class", "form-control voteObj");
    field.setAttribute("id", "EstimatesVoting[" + voteCount + "].VotingObjects[0]");
    field.setAttribute("name", "EstimatesVoting[" + voteCount + "].VotingObjects[0].Content");
    field.setAttribute("type", "text");
    field.setAttribute("placeholder", "Вариант ответа");
    field.setAttribute("asp-for", "EstimatesVoting[" + voteCount + "].VotingObjects[0].Content");
    div.appendChild(field);
    divBlock.appendChild(div);

    div = document.createElement("div");
    div.setAttribute("class", "form-group");
    field = document.createElement("input");
    field.setAttribute("class", "form-control voteObj");
    field.setAttribute("id", "EstimatesVoting[" + voteCount + "].VotingObjects[1]");
    field.setAttribute("name", "EstimatesVoting[" + voteCount + "].VotingObjects[1].Content");
    field.setAttribute("type", "text");
    field.setAttribute("placeholder", "Вариант ответа");
    field.setAttribute("asp-for", "EstimatesVoting[" + voteCount + "].VotingObjects[1].Content");
    div.appendChild(field);
    divBlock.appendChild(div);

    container.appendChild(divBlock);
}

function newRangingFields() {
    alert("ranging")

}

