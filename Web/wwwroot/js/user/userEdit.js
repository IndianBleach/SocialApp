$(document).ready(() => {

    const Validate = (str) => {
        return str.replace(/^(\s|\.|\,|\;|\:|\?|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\_|\~|\`|\'|\\|\-|\/|\+)*?$/, '');
    }

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
            Username: Validate(e.target.getElementsByTagName("input")[0].value),
            tags,
            oldPassword: Validate(e.target.getElementsByTagName("input")[1].value),
            newPassword: Validate(e.target.getElementsByTagName("input")[2].value),
            newPasswordConfirm: Validate(e.target.getElementsByTagName("input")[3].value),
        };

        $.post("/user/im/account", { model }, resp => {            
            if (resp != null)
                if (resp.isSuccess == true) {
                    window.location.reload();
                }
                else {
                    $("#updateAccountError").text("При сохранении пошло не так");
                }
            else {
                $("#updateAccountError").text("При сохранении пошло не так");
            }
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