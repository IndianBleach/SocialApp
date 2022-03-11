$(document).ready(() => {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });

    sessionStorage.setItem("stg", 0);

    const hideBackground = () => {
        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");
    };

    $("#hideBackgroundWrapper").mousedown(function (e) {
        var container = $("#checkOutContainer");
        if (container.has(e.target).length === 0) {
            $("#hideBackgroundWrapper").addClass("d-none");
            $("body").removeClass("overflow-hidden");

            //clear
            $("#loginWindow").addClass("d-none");
            $("#selectTagWindow").removeClass("selectTagContainer-prev");
            $("#signinWindow").removeClass("signin-window-animate");
            $("#signinWindow").addClass("d-none");

            $("#selectTagWindow").removeClass("selectTagContainer-animate");
            $("#loginWindow").removeClass("win-hide-active");
            $("#loginWindow").removeClass("z-m10");
            $("#selectTagWindow").removeClass("z-10");
            $("#selectTagWindow").addClass("d-none");
        }
    });

    // prev to signin
    $("#prevLoginWindowBtn").on("click", (e) => {
        e.preventDefault();
        $("#selectTagWindow").addClass("selectTagContainer-prev");
        $("#loginWindow").removeClass("win-hide-active");
        $("#loginWindow").removeClass("z-m10");
        $("#loginWindow").addClass("z-10");
        $("#selectTagWindow").removeClass("z-10");
        $("#selectTagWindow").addClass("z-m10");
    });

    //to sign Up
    $("#toSigninWindowBtn").on("click", (e) => {
        e.preventDefault();
        $("#loginWindow").addClass("z-m10");
        $("#selectTagWindow").addClass("d-none");
        $("#loginWindow").addClass("win-hide-active");
        $("#signinWindow").addClass("signin-window-animate");
        $("#signinWindow").removeClass("d-none");
        $("#loginWindow").addClass("z-m10");
        $("#loginWindow").removeClass("z-10");
        $("#signinWindow").removeClass("z-m10");
        $("#signinWindow").addClass("z-10");
    });

    // to sign In
    $(".async-open-login ").on("click", (e) => {
        hideBackground();
        $("#loginWindow").removeClass("d-none");
    });

    // prev to Sign Up
    $("#prevSignupWindowBtn").on("click", (e) => {
        e.preventDefault();
        //$("#selectTagWindow").addClass("selectTagContainer-prev");
        $("#signinWindow").addClass("selectTagContainer-prev");
        $("#loginWindow").removeClass("win-hide-active");
        $("#loginWindow").removeClass("z-m10");
        $("#loginWindow").addClass("z-10");
        $("#signinWindow").removeClass("z-10");
        $("#signinWindow").addClass("z-m10");
        $("#selectTagWindow").removeClass("d-none");
        $("#selectTagWindow").addClass("win-hide-active");
    });
   
    //search by tag
    $(".async-search-bytag").on("click", (e) => {
        e.preventDefault();
        let thisTag = e.target.dataset.tag;
        $(`.async-tag-avatar[data-fortag != ${thisTag}]`).toggleClass(
            "user-avatar-hide"
        );
    });

    // to select Tags
    $("#asyncSelectTagsBtn").on("click", (e) => {
        e.preventDefault();
        $("#selectTagWindow").addClass("selectTagContainer-animate");
        //$("#selectTagWindow").removeClass("z-m10");
        $("#loginWindow").addClass("win-hide-active");
        $("#loginWindow").addClass("z-m10");
        $("#selectTagWindow").addClass("z-10");
        $("#selectTagWindow").removeClass("selectTagContainer-prev");
        $("#selectTagWindow").removeClass("win-hide-active");
        $("#signinWindow").addClass("d-none");

        $("#selectTagWindow").removeClass("d-none");
    });

    //select tags
    $(".async-select-tag").on("click", (e) => {
        e.preventDefault();
        if (e.target.hasAttribute("selected")) {
            e.target.removeAttribute("selected");
            $("#choiceTags").append(e.target);
            let newVal = Number(sessionStorage.getItem("stg")) - 1;
            sessionStorage.setItem("stg", newVal);
            $("#selectedTagsCount").text(`${newVal}`);
        } else if (Number(sessionStorage.getItem("stg")) < 5) {
            let newVal = Number(sessionStorage.getItem("stg")) + 1;
            sessionStorage.setItem("stg", newVal);
            e.target.setAttribute("selected", true);
            $("#selectedTags").append(e.target);
            $("#selectedTagsCount").text(`${newVal}`);
        }
    });

    //set blue
    $(".begin-validate").on("input", (e) => {
        let elems = $(".need-next-input");
        let next = true;
        for (let i = 0; i < elems.length; i++) {
            if ((elems[i].value == "") | (elems[i].value == " ")) {
                next = false;
            }
        }
        if (next == true) {
            $("#asyncSelectTagsBtn")
                .removeAttr("disabled")
                .removeClass("icon-btn-disabled");
        } else {
            $("#asyncSelectTagsBtn").attr("disabled");
            $("#asyncSelectTagsBtn").addClass("icon-btn-disabled");
        }
    });
});
