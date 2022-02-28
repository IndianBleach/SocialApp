$(document).ready(() => {
       
    // FRIEND theme
    const switchToFriendsWindow = (userGuid) => {
        $(".repostToUser").remove();
        $(".firendsWarning").remove();
        $("#friendsWindowLoadPrev").removeClass("d-none");
        $("#friendsWindowLoad").removeClass("d-none");
        $("#friendRequestsWindowLoad").addClass("d-none");
        $("#friendRequestsWindowLoadPrev").addClass("d-none");

        $.get("/asyncload/user/getfriends", { userGuid }, resp => {
            console.log(resp);
            if (resp.length == 0) {
                $("#friendsWindowLoadPrev").addClass("d-none");
                $("#friendsWindowLoad").append("<div class='firendsWarning h-100 d-flex justify-content-center align-items-center text-center'><p class='t-md t-med text-muted'>Список друзей пуст</p></div>");
            }
            else if (resp.length > 0) {

                $.post("/asyncload/user/isme", { userGuid }, isMyProfile => {

                    $("#friendsWindowLoadPrev").addClass("d-none");
                    if (isMyProfile == true) {
                        resp.forEach(x => {
                            $("#friendsWindowLoad").append(`<div class="mb-2 repostToUser"><a href='/user/${x.guid}'><img class="me-1" src="../media/userAvatars/${x.avatarName}" />${x.name}</a><button data-guid='${x.friendGuid}' class="asyncRemoveFriendBtn btn clr-mute t-med">Удалить</button></div>`);
                        });
                    }
                    else if (isMyProfile == false) {
                        resp.forEach(x => {
                            $("#friendsWindowLoad").append(`<div class="mb-2 repostToUser"><a href='/user/${x.guid}'><img class="me-1" src="../media/userAvatars/${x.avatarName}" />${x.name}</a></div>`);
                        });
                    }

                    // FRIEND - Remove friend
                    $(".asyncRemoveFriendBtn").on("click", (e) => {
                        e.target.classList.add("text-muted");
                        e.target.classList.remove("clr-mute");
                        e.target.textContent = "Удален";
                        e.target.setAttribute("disabled", true);

                        let guid = e.target.dataset.guid;
                        $.post("/asyncload/user/removefriend", { guid }, resp => {
                            console.log(resp);
                        });
                    });
                });
                
                
            }
        });
    };

    const switchToFriendRequestsWindow = (userGuid) => {
        $(".repostToUser").remove();
        $(".firendsWarning").remove();
        $("#friendWindowLoadPrev").addClass("d-none");
        $("#friendsWindowLoad").addClass("d-none");
        $("#friendRequestsWindowLoad").removeClass("d-none");
        $("#friendRequestsWindowLoadPrev").removeClass("d-none");

        $.get("/asyncload/user/getfriendrequests", { userGuid }, resp => {
            console.log(resp);
            if (resp.length == 0) {
                $("#friendRequestsWindowLoadPrev").addClass("d-none");
                $("#friendRequestsWindowLoad").append("<div class='firendsWarning h-100 d-flex justify-content-center align-items-center text-center'><p class='t-md t-med text-muted'>У вас нет заявок в друзья</p></div>");
            }
            else if (resp.length > 0) {                
                $("#friendRequestsWindowLoadPrev").addClass("d-none");
                resp.forEach(x => {
                    $("#friendRequestsWindowLoad").append(`<div class="mb-2 repostToUser"><a href='/user/${x.guid}'><img class="me-1" src="../media/userAvatars/${x.avatarName}" />${x.name}</a><button data-guid='${x.friendGuid}' class="asyncAcceptFriendBtn btn clr-blue t-med">Принять</button></div>`);
                });

                // FRIEND - Accept friend
                $(".asyncAcceptFriendBtn").on("click", (e) => {
                    let guid = e.target.dataset.guid;
                    e.target.classList.add("text-muted");
                    e.target.classList.remove("clr-blue");
                    e.target.textContent = "Запрос принят";
                    e.target.setAttribute("disabled", true);

                    $.post("/asyncload/user/acceptfriend", { guid }, resp => {
                        console.log(resp);
                    });
                });
            }
        });
    };

    // INVITE - Show
    $(".showInviteWindowBtn").on("click", (e) => {
        $(".inviteWarning").remove();
        $("#inviteWindow").removeClass("d-none");
        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");

        $("#inviteWindowLoadPrev").removeClass("d-none");

        $.get("/asyncload/user/getinvite", {}, resp => {
            if (resp.length > 0) {
                $("#inviteWindowLoadPrev").addClass("d-none");
                resp.forEach(x => {
                    $("#inviteWindowLoad").append(`<div class="mb-2 repostToUser"><a class="inviteIdeaLink text-truncate" href="/idea/${x.ideaGuid}"><span class="ideaInviteLink hover-white">${x.ideaName}</span></a><button data-idea="${x.ideaGuid}" class="asyncInviteBtn btn">Отправить</button></div>`)
                });

                // INVITE - Send
                $(".asyncInviteBtn").on("click", (e) => {
                    e.preventDefault();
                    e.target.classList.add("clr-mute");
                    e.target.textContent = "Отправлено";
                    e.target.setAttribute("disabled", true);

                    let user = sessionStorage.getItem("inviteUser");
                    let idea = e.target.dataset.idea;
                    $.post("/asyncload/user/sendinvite", { user, idea }, resp => {
                        console.log(resp);
                    });
                });
            }
            else if (resp.length == 0) {
                $("#inviteWindowLoadPrev").addClass("d-none");
                $("#inviteWindowLoad").append("<div class='inviteWarning h-100 d-flex justify-content-center align-items-center text-center'><p class='t-md t-med text-muted'>Список идей пуст</p></div>");
            }
        })
    });

    $(".closeInviteWindowBtn").on("click", (e) => {
        e.preventDefault();
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
        $("#inviteWindow").addClass("d-none");
    });

    // FRIEND - Send
    $(".asyncSendFriendBtn").on("click", (e) => {
        e.preventDefault();
        e.target.classList.add("text-muted");
        e.target.textContent = "Отправлено";
        e.target.setAttribute("disabled", true);
        let userGuid = e.target.dataset.guid;
        $.post("/asyncload/sendfriend", { userGuid }, resp => {
            console.log(resp);
        });
    });

    // FRIEND - Show
    $(".showFriendsWindowBtn").on("click", (e) => {
        $("#toFriendsWindowBtn").addClass("btn-link-active");
        $("#toFriendRequestsWindowBtn").removeClass("btn-link-active");
        e.preventDefault();
        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");
        $("#friendsWindow").removeClass("d-none");
        let userGuid = e.target.dataset.guid;
        sessionStorage.setItem("friendsLoadGuid", userGuid);

        switchToFriendsWindow(userGuid);
    });

    // FRIEND - To friends
    $("#toFriendsWindowBtn").on("click", (e) => {
        $(".firendsWarning").remove();
        e.target.classList.add("btn-link-active");
        $("#toFriendRequestsWindowBtn").removeClass("btn-link-active");

        let userGuid = sessionStorage.getItem("friendsLoadGuid");
        switchToFriendsWindow(userGuid);
    });

    // FRIEND - To friend Reqs
    $("#toFriendRequestsWindowBtn").on("click", (e) => {
        $(".firendsWarning").remove();
        e.target.classList.add("btn-link-active");
        $("#toFriendsWindowBtn").removeClass("btn-link-active");
        let userGuid = sessionStorage.getItem("friendsLoadGuid");
        switchToFriendRequestsWindow(userGuid);
    });

    // FRIEND - Close
    $(".closeFriendsWindowBtn").on("click", (e) => {
        $("#friendsWindow").addClass("d-none");
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
        $("#friendsWindowLoadPrev").addClass("d-none");
        $("#friendRequestsWindowLoadPrev").addClass("d-none");
    });


})