$(document).ready(() => {
    $("#hideBackgroundWrapper").mousedown(function (e) {
        var container = $("#checkOutContainer");
        if (container.has(e.target).length === 0) {
            $("#hideBackgroundWrapper").addClass("d-none");
            $("body").removeClass("overflow-hidden");

            $("#newChatWindow").addClass("d-none");

            //chat 
            //$(".chatWarning").remove();
            //$(".repostToUser").remove();
        }
    });

    // CHAT - New chat window
    $(".showNewChatBtn").on("click", (e) => {
        e.preventDefault();
        $("#newChatWindowLoadPrev").removeClass("d-none");

        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");
        $("#newChatWindow").removeClass("d-none");

        let userGuid = e.target.dataset.guid;

        let alreadyLoaded = Boolean(sessionStorage.getItem("new_chats_loaded"));
        if (!alreadyLoaded) {
            $.get("/asyncload/chat/new", { userGuid }, resp => {
                sessionStorage.setItem("new_chats_loaded", true);
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

                    sessionStorage.setItem("inviteUser", userGuid);
                    $("#inviteWindowUsername").text(name);
                    $("#inviteWindowAvatar").attr("src", "../media/userAvatars/" + avatar);

                    $("#activeChatContainer").removeClass("d-none");
                    $("#activeChatUserAvatar").attr("src", "../media/userAvatars/" + avatar)
                    $("#activeChatUserName").text(name);
                    $("#activeChatWarning").removeClass("d-none");
                });
            })
        }
    });

    // CHAT - Send message
    $("#chatMessageForm").on("submit", (e) => {
        e.preventDefault();
        let message = e.target.getElementsByTagName("input")[0].value;
        let chatUser = sessionStorage.getItem("chatWith");
        let chatGuid = sessionStorage.getItem("activeChat");
        $("#chatMessageInput").val("");

        let normalizeGuid = chatGuid;
        if (chatGuid == "null") {
            normalizeGuid = null;
        }

        $.post("/asyncload/chat/sendmessage", { message, chatUser, chat: normalizeGuid }, resp => {
            if (sessionStorage.getItem("activeChat") == "null") {
                sessionStorage.setItem("activeChat", resp.chatGuid);
                joinChat(resp.chatGuid);
                let info = $("#activeChatInfo");
                let avatar = info.data("avatar");
                let userGuid = info.data("user");
                let userName = info.data("name");
                $("#activeChatWarning").addClass("d-none");
                $("#activeChatLoad").prepend(`<div class="chatMessage myMessage">${resp.message}<br><span class="mesTime">сегодня</span></div>`);
                $("chatUsersLoad").prepend(`<div class="chatUserSelect"><button data-name="${userName}" data-guid="${resp.chatGuid}" data-user="${userGuid}" class="asyncSelectChatBtn btn"><img class="me-1" src="../media/userAvatars/${avatar}" /><div>${userName}<br /><span>Последнее сегодня</span></div></button></div>`);
            }
        });
    })

    // CHAT - Settings
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

    const joinChat = (chatGuid) => {
        let url = "/chat/join/" + _connectionId + "/" + chatGuid;
        $.post(url, null, (resp) => {
        })
    };

    const LoadActiveChat = (chatGuid, avatar, user, name) => {
        $(".chatMessage").remove();
        $("#activeChatContainer").removeClass("d-none");
        $("#defaultChatContainer").addClass("d-none");
        $("#activeChatWindowLoadPrev").removeClass("d-none");
        $("#activeChatWarning").remove();

        sessionStorage.setItem("activeChat", chatGuid);
        sessionStorage.setItem("chatWith", user);

        $("#activeChatProfileLink").attr("href", "/user/" + user);
        $("#activeChatUserAvatar").attr("src", avatar)
        $("#activeChatUserName").text(name);
        $("#activeChatUserLink").attr("href", "/user/" + user);

        sessionStorage.setItem("inviteUser", user);
        $("#inviteWindowUsername").text(name);
        $("#inviteWindowAvatar").attr("src", avatar);
       
        joinChat(chatGuid);

        $.get("/asyncload/chat/getchat", { chatGuid }, resp => {
            console.log(resp);
            $("#activeChatWindowLoadPrev").addClass("d-none");
            if (resp.messages.length > 0) {
                resp.messages.forEach(x => {
                    if (x.isMy == true) {
                        if (x.isRepost) {
                            $("#activeChatLoad").append(`<div class="chatMessage msgRepost myMessage"><a href="/idea/${x.ideaGuid}">Поделился идеей<div style="background: linear-gradient(0deg,rgba(0, 0, 0, 0.44),rgba(0, 0, 0, 0.44)),url(../media/ideaAvatars/${x.ideaAvatar});"><span>${x.message}</span></div></a></div>`);
                        }
                        else if (x.isRepeat) {
                            $("#activeChatLoad").append(`<div class="chatMessage myMessage msgRepeat">${x.message}</div>`);
                        }
                        else {
                            $("#activeChatLoad").append(`<div class="chatMessage myMessage">${x.message}<br><span class="mesTime">${x.datePublish}</span></div>`);
                        }
                    }
                    else if (x.isMy == false) {
                        if (x.isRepost) {
                            $("#activeChatLoad").append(`<div class="chatMessage msgRepost"><img src="../media/userAvatars/${x.authorAvatar}" /><a href="/idea/${x.ideaGuid}">Поделился идеей<div style="background: linear-gradient(0deg,rgba(0, 0, 0, 0.44),rgba(0, 0, 0, 0.44)),url(../media/ideaAvatars/${x.ideaAvatar});"><span>${x.message}</span></div></a></div>`);
                        }
                        else if (x.isRepeat == true) {
                            $("#activeChatLoad").append(`<div class="chatMessage msgRepeat"><img src="../media/userAvatars/${x.authorAvatar}"/>${x.message}</div>`);
                        } else {
                            $("#activeChatLoad").append(`<div class="chatMessage"><img src="../media/userAvatars/${x.authorAvatar}"/>${x.message}<br><span class="mesTime">${x.datePublish}</span></div>`);
                        }
                    }
                })
            }
        });
    };

    const CheckActiveChat = () => {
        let info = $("#activeChatInfo");
        let avatar = info.data("avatar");
        let userGuid = info.data("user");
        let userName = info.data("name");
        let chat = info.data("chat");
        let normalizeAvatar = "../media/userAvatars/" + avatar;

        if (userName != undefined) {
            if (chat == "0") {
                chat = null;
                $(".chatMessage").remove();
                $("#activeChatContainer").removeClass("d-none");
                $("#defaultChatContainer").addClass("d-none");
                $("#activeChatWindowLoadPrev").addClass("d-none");
                $("#activeChatWarning").removeClass("d-none");

                sessionStorage.setItem("activeChat", chat);
                sessionStorage.setItem("chatWith", userGuid);

                $("#activeChatUserAvatar").attr("src", normalizeAvatar);               
                $("#activeChatUserName").text(userName);

                sessionStorage.setItem("inviteUser", userGuid);
                $("#inviteWindowUsername").text(userName);
                $("#inviteWindowAvatar").attr("src", normalizeAvatar);
                $("#activeChatProfileLink").attr("href", "/user/" + userGuid);

            } else LoadActiveChat(chat, normalizeAvatar, userGuid, userName);
        }        
    }
    CheckActiveChat();
    
    // CHAT - Select
    $(".asyncSelectChatBtn").on("click", (e) => {

        let elem = e.target.closest("button");
        $(".asyncSelectChatBtn").removeClass("active");
        elem.classList.add("active");

        let chatGuid = elem.dataset.guid;
        let avatar = elem.getElementsByTagName("img")[0].src;
        let userGuid = elem.dataset.user;
        let name = elem.dataset.name;        

        LoadActiveChat(chatGuid, avatar, userGuid, name);
    });

    // CHAT - Close chat window
    $(".closeNewChatWindowBtn").on("click", (e) => {
        e.preventDefault();
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
        $("#newChatWindow").addClass("d-none");
        //$(".repostToUser").remove();
    });
   
    // Chat - Recieve
    connection.on("RecieveMessage", (x) => {

        console.log(x);

        let isMy = sessionStorage.getItem("chatWith") !== x.authorGuid;
        if (isMy == true) {
            if (x.isRepost) {
                $("#activeChatLoad").prepend(`<div class="chatMessage msgRepost myMessage"><a href="/idea/${x.ideaGuid}">Поделился идеей<div style="background: linear-gradient(0deg,rgba(0, 0, 0, 0.44),rgba(0, 0, 0, 0.44)),url(../media/ideaAvatars/${x.ideaAvatar});"><span>${x.message}</span></div></a></div>`);
            }
            else if (x.isRepeat) {
                $("#activeChatLoad").prepend(`<div class="chatMessage myMessage msgRepeat">${x.message}</div>`);
            }
            else {
                $("#activeChatLoad").prepend(`<div class="chatMessage myMessage">${x.message}<br><span class="mesTime">сегодня</span></div>`);
            }
        }
        else if (isMy == false) {
            if (x.isRepost) {
                $("#activeChatLoad").prepend(`<div class="chatMessage msgRepost"><img src="../media/userAvatars/${x.authorAvatar}" /><a href="/idea/${x.ideaGuid}">Поделился идеей<div style="background: linear-gradient(0deg,rgba(0, 0, 0, 0.44),rgba(0, 0, 0, 0.44)),url(../media/ideaAvatars/${x.ideaAvatar});"><span>${x.message}</span></div></a></div>`);
            }
            else if (x.isRepeat == true) {
                $("#activeChatLoad").prepend(`<div class="chatMessage msgRepeat"><img src="../media/userAvatars/${x.authorAvatar}"/>${x.message}</div>`);
            } else {
                $("#activeChatLoad").prepend(`<div class="chatMessage"><img src="../media/userAvatars/${x.authorAvatar}"/>${x.message}<br><span class="mesTime">сегодня</span></div>`);
            }
        }
    });
       
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

    // INVITE - Show
    $(".showInviteWindowBtn").on("click", (e) => {
        $("#inviteWindow").removeClass("d-none");
        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");

        $("#inviteWindowLoadPrev").removeClass("d-none");        

        let alreadyLoaded = Boolean(sessionStorage.getItem("invite_ideas_loaded"));
        if (!alreadyLoaded) {
            $.get("/asyncload/user/getinvite", {}, resp => {
                sessionStorage.setItem("invite_ideas_loaded", true);
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
                } else {
                    $("#inviteWindowLoad").append("<div class='inviteWarning h-100 d-flex justify-content-center align-items-center text-center'><p class='t-md t-med text-muted'>Активных идей не найдено</p></div>")
                }
            })
        };
    });

    $(".closeInviteWindowBtn").on("click", (e) => {
        e.preventDefault();
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
        $("#inviteWindow").addClass("d-none");
    });

});
