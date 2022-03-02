﻿$(document).ready(() => {
    const Validate = (str) => {
        return str.replace(/^(\s|\.|\,|\;|\:|\?|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\_|\~|\`|\'|\\|\-|\/|\+)*?$/, '');
    }

    $("#asyncNewIdeaForm").on("submit", e => {
        e.preventDefault();

        let selectedTags = [];
        $("#selectedNewIdeaTags button").each((e, elem) => {
            selectedTags.push(elem.dataset.tag);
        });

        let model = {
            authorGuid: "",
            name: Validate(e.target.getElementsByTagName("input")[0].value),
            description: Validate(e.target.getElementsByTagName("textarea")[0].value),
            tags: selectedTags,
            isPrivate: e.target.getElementsByTagName("input")[1].checked,
        };

        $.post("/create/idea", { model }, resp => {
            if (resp != null) {
                if (resp.isSuccess == false) {
                    $("#newIdeaWindowError").text(resp.message);
                }
                else if (resp.isSuccess) {
                    window.location.href = `/idea/${resp.createdObjectGuid}`;
                }
            }
            else {
                $("#newIdeaWindowError").text("Что-то пошло не так, попробуйте позже");
            }
        });
    });
})