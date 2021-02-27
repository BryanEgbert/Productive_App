function autoResize() {
    this.style.height = 'auto';
    this.style.height = this.scrollHeight + 'px';
}

function theRealAutoResize(id) {
    textarea = document.getElementById(id);
    textarea.addEventListener('input', autoResize, false);
}

function focusTextArea(id, desc) {
    document.getElementById(id).focus();
    document.getElementById(id).value = desc;
}

function editMode(iconClass, labelClass, textAreaClass, index) {
    document.getElementsByClassName(labelClass)[index].style.display = "none";
    document.getElementsByClassName(iconClass)[index].style.display = "none";
    document.getElementsByClassName(textAreaClass)[index].style.display = "initial";
}

function initialMode(iconClass, labelClass, textAreaClass, index) {
    document.getElementsByClassName(labelClass)[index].style.display = "initial";
    document.getElementsByClassName(iconClass)[index].style.display = "initial";
    document.getElementsByClassName(textAreaClass)[index].style.display = "none";
}

