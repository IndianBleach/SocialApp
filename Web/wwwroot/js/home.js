$(document).ready(() => {
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

            //profile
            $(".firendsWarning").remove();
            $(".repostWarning").remove();
            $(".repostToUser").remove();

            //chat 
            $(".chatWarning").remove();
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
                });
            }
        });
    });
   
    

    //chat
    $(".showNewChatBtn").on("click", (e) => {
        e.preventDefault();
        $("#newChatWindowLoadPrev").removeClass("d-none");

        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");
        $("#newChatWindow").removeClass("d-none");

        let userGuid = e.target.dataset.guid;

        $.get("/asyncload/chat/new", { userGuid }, resp => {
            if (resp.length == 0) {
                $("#newChatWindowLoadPrev").addClass("d-none");
                $("#newChatWindowLoad").append("<div class='chatWarning h-100 d-flex justify-content-center align-items-center text-center'><p class='t-md t-med text-muted'>Добавьте друзей, <br/> чтобы общаться с ними</p></div>");
            }
            else if (resp.length > 0) {
                $("#newChatWindowLoadPrev").addClass("d-none");
                resp.forEach(x => {
                    $("#newChatWindowLoad").append(`<div class="mb-2 repostToUser"><a href='/user/${x.guid}'><img class="me-1" src="../media/userAvatars/${x.avatarName}" />${x.name}</a><button data-guid='${x.guid}' data-avatar='${x.avatarName}' data-name='${x.name}' class="asyncNewChatBtn btn">Отправить</button></div>`);
                });                
            }

            // CHAT - Create new
            $(".asyncNewChatBtn").on("click", (e) => {
                e.preventDefault();
                $(".repostToUser").remove();
                
                $("#hideBackgroundWrapper").addClass("d-none");
                $("body").removeClass("overflow-hidden");
                $("#newChatWindow").addClass("d-none");
                $("#defaultChatContainer").addClass("d-none");

                let userGuid = e.target.dataset.guid;
                let avatar = e.target.dataset.avatar;
                let name = e.target.dataset.name;

                sessionStorage.setItem("activeChat", null);
                sessionStorage.setItem("chatWith", userGuid);

                $("#activeChatContainer").removeClass("d-none");
                $("#activeChatUserAvatar").attr("src", "../media/userAvatars/" + avatar)
                $("#activeChatUserName").text(name);
                $("#activeChatWarning").removeClass("d-none");
            });
        })
    });


    // CHAT - Send message
    $("#chatMessageForm").on("submit", (e) => {
        e.preventDefault();
        let message = e.target.getElementsByTagName("input")[0].value;
        let chatUser = sessionStorage.getItem("chatWith");
        let chatGuid = sessionStorage.getItem("activeChat");
        if (sessionStorage.getItem("activeChat") == "null") {
            chatGuid = null;
        }

        //$("#activeChatLoad").prepend(`<div class="chatMessage myMessage">${message}</div>`);
        $("#chatMessageInput").val("");

        joinChat(chatGuid);

        $.post("/asyncload/chat/sendmessage", { message, chatUser, chatGuid }, resp => {
        })
    })

    // CHAT - Select
    $(".asyncSelectChatBtn").on("click", (e) => {
        $(".chatMessage").remove();

        $("#activeChatContainer").removeClass("d-none");
        $("#defaultChatContainer").addClass("d-none");
        $("#activeChatWindowLoadPrev").removeClass("d-none");

        let chatGuid = e.target.closest("button").dataset.guid;
        let avatar = e.target.closest("button").getElementsByTagName("img")[0].src;
        let userGuid = e.target.closest("button").dataset.user;

        sessionStorage.setItem("activeChat", chatGuid);
        sessionStorage.setItem("chatWith", userGuid);

        $("#activeChatUserAvatar").attr("src", avatar)
        $("#activeChatUserName").text(e.target.closest("button").dataset.name);

        joinChat(chatGuid);

        $.get("/asyncload/chat/getchat", { chatGuid }, resp => {
            $("#activeChatWindowLoadPrev").addClass("d-none");
            if (resp.messages.length > 0) {
                resp.messages.forEach(x => {
                    if (x.isMy == true) {
                        if (x.isRepeat) {
                            $("#activeChatLoad").prepend(`<div class="chatMessage myMessage msgRepeat">${x.message}</div>`);
                        }
                        else {
                            $("#activeChatLoad").prepend(`<div class="chatMessage myMessage">${x.message}<br><span class="mesTime">${x.datePublish}</span></div>`);
                        }
                    }
                    else if (x.isMy == false) {
                        if (x.isRepeat == true) {
                            $("#activeChatLoad").prepend(`<div class="chatMessage msgRepeat"><img src="../media/userAvatars/${x.authorAvatar}"/>${x.message}</div>`);
                        } else {
                            $("#activeChatLoad").prepend(`<div class="chatMessage"><img src="../media/userAvatars/${x.authorAvatar}"/>${x.message}<br><span class="mesTime">${x.datePublish}</span></div>`);
                        }
                    }
                })
            }
        });
    });


    $(".closeNewChatWindowBtn").on("click", (e) => {
        e.preventDefault();
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
        $("#newChatWindow").addClass("d-none");
    });

    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    let _connectionId = '';

    connection.start()
        .then(async () => {
            let id = await connection.invoke("GetConnectionId");
            _connectionId = id;
        })
        .catch(() => {
        });

    // Chat - Recieve
    connection.on("RecieveMessage", (x) => {
        let isMy = sessionStorage.getItem("chatWith") !== x.authorGuid;
        if (isMy == true) {
            if (x.isRepeat) {
                $("#activeChatLoad").prepend(`<div class="chatMessage myMessage msgRepeat">${x.message}</div>`);
            }
            else {
                console.log(1);
                $("#activeChatLoad").prepend(`<div class="chatMessage myMessage">${x.message}<br><span class="mesTime">сегодня</span></div>`);
            }
        }
        else if (isMy == false) {
            if (x.isRepeat == true) {
                $("#activeChatLoad").prepend(`<div class="chatMessage msgRepeat"><img src="../media/userAvatars/${x.authorAvatar}"/>${x.message}</div>`);
            } else {
                $("#activeChatLoad").prepend(`<div class="chatMessage"><img src="../media/userAvatars/${x.authorAvatar}"/>${x.message}<br><span class="mesTime">сегодня</span></div>`);
            }
        }
    });

    const joinChat = (chatGuid) => {
        let url = "/chat/join/" + _connectionId + "/" + chatGuid;
        $.post(url, null, (resp) => {
        })
    };
    

    // Emoji
    $("#showMessageEmojiBtn").on("click", (e) => {
        e.preventDefault();
        $("#choiceEmojiWindow").removeClass("d-none");
    });

    $(".asyncSendEmojiBtn").on("click", (e) => {
        let messageNow = $("#chatMessageInput").val();
        let mess = `${messageNow + e.target.textContent.replace(/ /g, "")}`;
        $("#chatMessageInput").val(mess);
    });

    $("#closeEmojiWindowBtn").on("click", (e) => {
        $("#choiceEmojiWindow").addClass("d-none");
    });

    

    //invite
    $(".showInviteWindowBtn").on("click", (e) => {
        $("#inviteWindow").removeClass("d-none");
        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");
    });

    //asyncInviteBtn
    $(".asyncInviteBtn").on("click", (e) => {
        e.preventDefault();
        e.target.classList.add("clr-mute");
        e.target.textContent = "Отправлено";
        e.target.setAttribute("disabled", true);
    });

    $(".closeInviteWindowBtn").on("click", (e) => {
        e.preventDefault();
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
        $("#inviteWindow").addClass("d-none");
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
    })


    //selectedNewIdeaTagsCount, selectedNewIdeaTags, choiceNewIdeaTags, async-select-tag
    //select tags
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
