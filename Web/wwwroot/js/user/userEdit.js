﻿$(document).ready(() => {

    $("#formAvatarInput").on("input", (e) => {
        e.preventDefault();
        $("#avatarUpdateSuccess").removeClass("d-none");
    });

    $("#editAccountUserForm").on("submit", (e) => {
        e.preventDefault();

        let tags = [];
        $("#updatedTagsContainer button").each((e, elem) => {
            tags.push(elem.dataset.tag);
        });

        let model = {
            Username: e.target.getElementsByTagName("input")[0].value,
            tags,
            oldPassword: e.target.getElementsByTagName("input")[1].value,
            newPassword: e.target.getElementsByTagName("input")[2].value,
            newPasswordConfirm: e.target.getElementsByTagName("input")[3].value,
        };

        $.post("/user/im/account", { model }, resp => {
            console.log(resp);
            window.location.reload();
        });
    });

    $("#editRemoveUserForm").on("submit", (e) => {
        e.preventDefault();
        $.post("/user/im/remove", { password: e.target.getElementsByTagName("input")[0].value }, resp => {
            console.log(resp);
        });
    });

    // TAGS - Switcher
    sessionStorage.setItem("etca", $("#updatedTagsContainer button").length);
    $(".asyncSelectBtnTag").on("click", (e) => {
        e.preventDefault();
        if (e.target.classList.contains("updatedTag")) {
            e.target.classList.remove("updatedTag");
            $("#updatedTagsContainer").remove(e.target);
            $("#selectTagsContainer").append(e.target);
            let newEtc = Number(sessionStorage.getItem("etca")) - 1;
            sessionStorage.setItem("etca", newEtc);
        } else {
            let currentVal = Number(sessionStorage.getItem("etca"));
            if (currentVal < 5) {
                e.target.classList.add("updatedTag");
                $("#updatedTagsContainer").append(e.target);
                $("#selectTagsContainer").remove(e.target);
                let newEtc = currentVal + 1;
                sessionStorage.setItem("etca", newEtc);
            }
        }
    });
})