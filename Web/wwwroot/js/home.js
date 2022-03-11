$(document).ready(() => {

    // tooltips on
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

    $("#hideBackgroundWrapper").mousedown(function (e) {
        var container = $("#checkOutContainer");
        if (container.has(e.target).length === 0) {
            $("#hideBackgroundWrapper").addClass("d-none");
            $("body").removeClass("overflow-hidden");

            /*clear
            $("#loginWindow").addClass("d-none");
            $("#selectTagWindow").removeClass("selectTagContainer-prev");
            $("#signinWindow").removeClass("signin-window-animate");
            $("#signinWindow").addClass("d-none");
      
            $("#selectTagWindow").removeClass("selectTagContainer-animate");
            $("#loginWindow").removeClass("win-hide-active");
            $("#loginWindow").removeClass("z-m10");
            $("#selectTagWindow").removeClass("z-10");
            $("#selectTagWindow").addClass("d-none");
            */

            $("#hideBackgroundWrapper").addClass("d-none");
            $("body").removeClass("overflow-hidden");
            $("#repostWindow").addClass("d-none");
            $("#newChatWindow").addClass("d-none");
            $("#inviteWindow").addClass("d-none");
            $("#friendsWindow").addClass("d-none");

            //idea
            $("#newIdeaWindow").addClass("d-none");
            $("#goalWindowWrapper").addClass("d-none");
            $("#topicWindowWrapper").addClass("d-none");
            $("#newTopicWindow").addClass("d-none");
            $("#newGoalWindow").addClass("d-none");
            $("#membersWindow").addClass("d-none");            
            $("#reactWindow").addClass("d-none");
            $("#participationWindow").addClass("d-none");

            $(".topicMessage").remove();
            $(".topicRemoveBtn").remove();
            $(".goalMessage").remove();

            //profile
            $(".firendsWarning").remove();
            $(".repostWarning").remove();
            $(".repostToUser").remove();            

            //chat 
            //$(".chatWarning").remove();
        }
    });

    //set like
    $(".asyncSetLike").on("click", (e) => {
        e.target.classList.add("clr-blue");
        let idea = e.target.dataset.idea;
        $.post("/asyncload/idea/setlike", { idea }, resp => {
            console.log(resp);
        });
    });

    //set react
    $(".async-reaction-btn").on("click", (e) => {
        e.preventDefault();
        let elem = e.target;
        if (e.target.tagName === "SPAN") {
            elem = e.target.closest("button");
        }
        let thisReactVal = elem.dataset.react;
        let thisForm = elem.closest("form");
        let btns = thisForm.getElementsByTagName("button");
        for (let i = 0; i < btns.length; i++) {
            if (btns[i].dataset.react === thisReactVal) {
                btns[i].closest("div").classList.add("react-wrap-active");
                let val =
                    Number(btns[i].getElementsByTagName("span")[0].textContent) + 1;
                btns[i].getElementsByTagName("span")[0].textContent = val;
                btns[i].setAttribute("disabled", true);
            } else {
                btns[i].closest("div").classList.add("react-wrap-noactive");
            }
        }
        let idea = e.target.dataset.idea;
        $.post("/asyncload/idea/setreaction", { reaction: thisReactVal, idea }, resp => {
            console.log(resp);
        });
    });


    //REPOST - Close
    $(".closeRepostWindowBtn").on("click", (e) => {
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
        $("#repostWindow").addClass("d-none");
        $(".repostToUser").remove();
        $(".repostWarning").remove();
    });

    //REPOST - Show
    $(".showRepostWindowBtn").on("click", (e) => {

        sessionStorage.setItem("activeRepost", e.target.dataset.idea);

        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");
        $("#repostWindow").removeClass("d-none");
        $("repostWindowLoadPrev").removeClass("d-none");

        $.get("/asyncload/repostusers", {}, resp => {
            console.log(resp);
            if (resp.length == 0) {
                $("#repostWindowLoadPrev").addClass("d-none");
                $("#repostWindowLoad").append("<div class='repostWarning h-100 d-flex justify-content-center align-items-center text-center'><p class='t-md t-med text-muted'>Добавьте друзей, <br/> чтобы делиться с ними идеями</p></div>");
            } else if (resp.length > 0) {
                $("#repostWindowLoadPrev").addClass("d-none");
                resp.forEach(x => {
                    $("#repostWindowLoad").append(`<div class="mb-2 repostToUser"><a href='/user/${x.guid}'><img class="me-1" src="../media/userAvatars/${x.avatarName}" />${x.name}</a><button data-guid='${x.guid}' class="asyncRepostBtn btn">Отправить</button></div>`);
                });

                //REPOST - Send
                $(".asyncRepostBtn").on("click", (e) => {
                    e.preventDefault();
                    e.target.classList.add("clr-mute");
                    e.target.textContent = "Отправлено";
                    e.target.setAttribute("disabled", true);

                    let user = e.target.dataset.guid;
                    let idea = sessionStorage.getItem("activeRepost");

                    console.log(user);
                    console.log(idea);

                    $.post("/asyncload/chat/repostidea", { user, idea }, resp => {

                    })
                });
            }
        });
    });
   
   
    //New Idea window
    $(".showNewIdeaWindowBtn").on("click", (e) => {
        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");
        $("#newIdeaWindow").removeClass("d-none");
    });

    $(".closeNewIdeaWindowBtn").on("click", (e) => {
        e.preventDefault();
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
        $("#newIdeaWindow").addClass("d-none");
    });

    sessionStorage.setItem("stgidea", 0);
    $(".async-newidea-tag").on("click", (e) => {
        e.preventDefault();
        if (e.target.hasAttribute("selected") === true) {
            e.target.removeAttribute("selected");
            $("#choiceNewIdeaTags").append(e.target);
            let newVal = Number(sessionStorage.getItem("stgidea")) - 1;
            sessionStorage.setItem("stgidea", newVal);
            $("#selectedNewIdeaTagsCount").text(`${newVal}`);
        } else if (Number(sessionStorage.getItem("stgidea")) < 5) {
            let newVal = Number(sessionStorage.getItem("stgidea")) + 1;
            sessionStorage.setItem("stgidea", newVal);
            e.target.setAttribute("selected", true);
            $("#selectedNewIdeaTags").append(e.target);
            $("#selectedNewIdeaTagsCount").text(`${newVal}`);
        }
    });

});
