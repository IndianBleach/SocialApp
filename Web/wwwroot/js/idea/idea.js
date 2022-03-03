$(document).ready(() => {
    // topicDetailAuthorLink, topicDetailName, topicDetailAuthorName
    // topicDetailDescription

    const Validate = (str) => {
        return str.replace(/^(\s|\.|\,|\;|\:|\?|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\_|\~|\`|\'|\\|\-|\/|\+)*?$/, '');
    }

    // TOPIC - Show
    $(".asyncShowTopicWindow").on("click", (e) => {
        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");
        $("#topicWindowWrapper").removeClass("d-none");

        let guid = e.target.closest("a").dataset.topic;

        $.get("/asyncload/idea/gettopic", { guid }, resp => {
            if (resp != null) {
                console.log(resp);
                $("#topicWindowForm").attr("data-guid", resp.guid);
                $("#topicDetailAuthorLink").attr("src", "/user/" + resp.authorGuid);
                $("#topicDetailAuthorLink img").attr("src", "../media/userAvatars/" + resp.authorAvatar);
                $("#topicDetailAuthorLink span").text(resp.datePublished);
                $("#topicDetailName").text(resp.name);
                $("#topicDetailDescription").text(resp.description);
                
                if (resp.canEdit == true) {
                    $("#topicDetailEdit").append(`<button data-guid="${resp.guid}" class="topicRemoveBtn t-xl t-med btn p-0 ps-2 bg-transparent text-danger">X</button>`);
                }
                else {
                    $(".topicRemoveBtn").remove();
                }

                resp.comments.forEach(x => {
                    if (resp.canEdit == true) {
                        $("#topicWindowCommentsLoad").append(`<div class="topicMessage"><img src="../media/userAvatars/${x.authorAvatar}" /><div><a href="/user/${x.authorGuid}">${x.authorName}<span class="text-muted"> - ${x.datePublished}</span></a><button data-topic="${resp.guid}" data-comment="${x.guid}" class="ms-1 topicCommentRemoveBtn btn p-0 bg-transparent text-danger">X</button><br />${x.comment}</div></div>`);
                    }
                    else {
                        $("#topicWindowCommentsLoad").append(`<div class="topicMessage"><img src="../media/userAvatars/${x.authorAvatar}" /><div><a href="/user/${x.authorGuid}">${x.authorName}<span class="text-muted"> - ${x.datePublished}</span></a><br />${x.comment}</div></div>`);
                    }
                })

                // TOPIC - Remove Comment
                $(".topicCommentRemoveBtn").on("click", (e) => {
                    let topic = e.target.dataset.topic;
                    let comment = e.target.dataset.comment;
                    e.target.parentElement.parentElement.remove();
                    $.post("/asyncload/idea/removetopiccomment", { topicGuid: topic, commentGuid: comment }, resp => {
                        console.log(resp);
                    })
                })

                // TOPIC - Remove
                $(".topicRemoveBtn").on("click", (e) => {
                    let topicGuid = e.target.dataset.guid;
                    $.post("/asyncload/idea/removetopic", { topicGuid }, resp => {
                        console.log(resp);
                    })
                });
            }
        })
    });

    // TOPIC - Comment +
    $("#topicWindowForm").on("submit", (e) => {
        e.preventDefault();
        let base = e.target.getElementsByTagName("input")[0].value;
        let text = Validate(base);
        let guid = e.target.dataset.guid;

        $.post("/asyncload/idea/topiccomment", { guid, text }, resp => {
            let avatar = $("#topicCommentAvatar").attr("src");
            $("#topicWindowCommentsLoad").append(`<div class="topicMessage"><img src="${avatar}" /><div><a disabled>Вы<span class="text-muted"> - сегодня</span></a><br />${text}</div></div>`);
            $("#topicCommentInput").val("");
        });
    })

    // TOPIC - Close
    $(".closeTopicWindowBtn").on("click", (e) => {
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
        $("#topicWindowWrapper").addClass("d-none");
        $(".topicMessage").remove();
        $(".topicRemoveBtn").remove();
    });

    // TOPIC - Create window
    $(".showNewTopicWindowBtn").on("click", (e) => {
        $("#newTopicWindow").removeClass("d-none");
        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");
    });

    // TOPIC - Create close
    $(".closeNewTopicWindowBtn").on("click", (e) => {
        $("#newTopicWindow").addClass("d-none");
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
    });

    // TOPIC - Create +
    $("#newTopicForm").on("submit", (e) => {
        e.preventDefault();
        let ideaGuid = e.target.dataset.guid;
        let baseName = e.target.getElementsByTagName("input")[0].value;
        let normalizeName = Validate(baseName);
        let desc = e.target.getElementsByTagName("textarea")[0].value;
        let normalizeDesc = Validate(desc);

        $.post("/asyncload/idea/createtopic", { name: normalizeName, content: normalizeDesc, ideaGuid }, resp => {
            if (resp != null) {
                if (resp.isSuccess == true) {
                    window.location.reload();
                }
                else if (resp.isSuccess == false) {
                    $("#newTopicWindowError").text("При создании что-то пошло не так");
                }
            }
            else {
                $("#newTopicWindowError").text("При создании что-то пошло не так");
            }
        })
    })

    // GOAL - New window +
    $(".showNewGoallWindowBtn").on("click", (e) => {
        $("#newGoalWindow").removeClass("d-none");
        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");
    });
    $(".closeNewGoalWindowBtn").on("click", (e) => {
        $("#newGoalWindow").addClass("d-none");
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
    });
    $("#newGoalForm").on("submit", (e) => {
        e.preventDefault();
        let idea = e.target.dataset.guid;
        let name = Validate(e.target.getElementsByTagName("input")[0].value);
        let desc = Validate(e.target.getElementsByTagName("textarea")[0].value);
        let withTasks = e.target.getElementsByTagName("input")[1].checked;

        $.post("/asyncload/idea/creategoal", { idea, name, desc, withTasks }, resp => {
            if (resp != null) {
                if (resp.isSuccess == true) {
                    window.location.reload();
                }
                else {
                    $("#newGoalWindowError").text("При создании цели что-то пошло не так");
                }
            } else {
                $("#newGoalWindowError").text("При создании цели что-то пошло не так");
            }
        });
    })

    // GOAL - Show
    $(".asyncShowGoalWindow").on("click", (e) => {
        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");
        $("#goalWindowWrapper").removeClass("d-none");

        let goal = e.target.closest("a").dataset.goal;
        $.get("/asyncload/idea/getgoal", { goal }, resp => {
            $("#goalDetailTitle").text(resp.name);
            $("#goalDetailDesc").text(resp.description);
            $("#goalDetailLink").attr("href", "/user/" + resp.authorGuid);
            $("#goalDetailLink img").attr("src", "../media/userAvatars/" + resp.authorAvatar)
            $("#goalDetailAuthorName").text(resp.datePublished);
            $("#goalTaskForm").attr("data-goal", resp.guid);
            console.log(resp.tasks);
            if (resp.tasks.length > 0) {
                resp.tasks.forEach(x => {
                    let complete = true;
                    let text = "🏆";
                    if (x.status == 0) {
                        complete = false;
                        text = "🎯";
                    }
                    $("#goalWindowLoad").append(`<div class="goalMessage"><a href="/user/${x.authorGuid}" class="text-truncate idea_hide_text col-2"><img class="me-1" src="../media/userAvatars/${x.authorAvatar}" />${x.authorName}</a><p class="col-7">${x.description}</p><span class="col-1"><button data-task="${x.guid}" data-goal="${goal}" data-gcomplete="${complete}" class="asyncChangeStatusBtn g-status btn">${text}</button></span><span class="idea_hide_elems col-2">${x.datePublished}</span></div>`);
                })

                // TASK - Change
                $(".asyncChangeStatusBtn").on("click", (e) => {
                    e.target.setAttribute("disabled", true);
                    let task = e.target.dataset.task;
                    let goal = e.target.dataset.goal;
                    if (e.target.dataset.gcomplete !== "true") {
                        e.target.textContent = "🏆";
                        e.target.dataset.gcomplete = "true";
                        
                        $.post("/asyncload/idea/changetask", { newStatus: 1, task, goal }, resp => {
                            console.log(resp);
                        })
                    } else {
                        e.target.textContent = "🎯";
                        e.target.dataset.gcomplete = "false";
                        $.post("/asyncload/idea/changetask", { newStatus: 0, task, goal }, resp => {
                            console.log(resp);
                        })
                    }
                });
            }
        })
    });

    let curUserAvatar = $("#goalDetailCurAvatar").attr("src");

    // TASK - Create
    $("#goalTaskForm").on("submit", (e) => {
        e.preventDefault();
        let goal = e.target.dataset.goal;
        let idea = e.target.dataset.idea;
        let content = Validate(e.target.getElementsByTagName("input")[0].value);
        $("#goalTaskInput").val("");
        $("#choiceEmojiWindow").addClass("d-none");
        $("#goalWindowLoad").append(`<div class="goalMessage"><a href="/user/im" class="text-truncate idea_hide_text col-2"><img class="me-1" src="${curUserAvatar}" />Вы</a><p class="col-7">${content}</p><span class="col-1"><button disabled data-gcomplete="false" class="asyncChangeStatusBtn g-status btn">🎯</button></span><span class="idea_hide_elems col-2">сейчас</span></div>`);
        $.post("/asyncload/idea/createtask", { content, idea, goal }, resp => {
            console.log(resp);
        });
    });    

    // GOAL - Close
    $(".closeGoalWindowBtn").on("click", (e) => {
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
        $("#goalWindowWrapper").addClass("d-none");
        $(".goalMessage").remove();
    });
    
    //asyncRejectMemberBtn
    $(".asyncRejectMemberBtn").on("click", (e) => {
        let user = e.target.dataset.user;
        let idea = e.target.dataset.idea;
        e.target.closest("div").getElementsByTagName("button")[0].setAttribute("disabled", true);
        e.target.classList.add("clr-mute");
        e.target.textContent = "Принят";
        e.target.setAttribute("disabled", true);
        $.post("/asyncload/idea/rejectmember", { idea, user }, resp => {
            console.log(resp);
        })
    })

    //asyncAcceptMemberBtn
    $(".asyncAcceptMemberBtn").on("click", (e) => {
        let user = e.target.dataset.user;
        let idea = e.target.dataset.idea;
        e.target.closest("div").getElementsByTagName("button")[1].setAttribute("disabled", true);
        e.target.classList.add("clr-mute");
        e.target.textContent = "Принят";
        e.target.setAttribute("disabled", true);
        $.post("/asyncload/idea/acceptmember", { idea, user }, resp => {
            console.log(resp);
        })
    })

    //asyncRemoveMemberBtn
    $(".asyncRemoveMemberBtn").on("click", (e) => {
        let user = e.target.dataset.user;
        let idea = e.target.dataset.idea;
        e.target.classList.add("clr-mute");
        e.target.textContent = "Удалён";
        e.target.setAttribute("disabled", true);
        $.post("/asyncload/idea/removemember", { idea, user }, resp => {
            console.log(resp);
        })
    })

    // MEMBERS - Show
    $(".showMemberWindowBtn").on("click", (e) => {
        $("#membersWindow").removeClass("d-none");
        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");
    });

    // MEMBERS - to Requests
    $("#toMemberRequestsWindowBtn").on("click", (e) => {
        $("#toMembersWindowBtn").removeClass("btn-link-active");
        e.target.classList.add("btn-link-active");
        $("#memberWindowLoad").addClass("d-none");
        $("#memberRequestsWindowLoad").removeClass("d-none");
    });

    // MEMBERS - to Members
    $("#toMembersWindowBtn").on("click", (e) => {
        $("#toMemberRequestsWindowBtn").removeClass("btn-link-active");
        e.target.classList.add("btn-link-active");
        $("#memberRequestsWindowLoad").addClass("d-none");
        $("#memberWindowLoad").removeClass("d-none");
    });

    // MEMBERS - Close
    $(".closeMembersWindowBtn").on("click", (e) => {
        $("#membersWindow").addClass("d-none");
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
    });

    // REACTS
    $(".showReactWindowBtn").on("click", (e) => {
        $("#reactWindow").removeClass("d-none");
        $("#hideBackgroundWrapper").removeClass("d-none");
        $("body").addClass("overflow-hidden");
    });
    $(".closeReactWindowBtn").on("click", () => {
        $("#reactWindow").addClass("d-none");
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
    });

    // EMOJI -check
    $("#showMessageEmojiBtn").on("click", (e) => {
        e.preventDefault();
        $("#choiceEmojiWindow").removeClass("d-none");
    });
    $(".asyncSendEmojiBtn").on("click", (e) => {
        let messageTopic = Validate($("#topicCommentInput").val());
        let messageTask = Validate($("#goalTaskInput").val());

        if (messageTopic != undefined) {
            let mess = `${messageTopic + e.target.textContent.replace(/ /g, "")}`;
            $("#topicCommentInput").val(mess);
        }
        else if (messageTask != undefined) {
            let mess = `${messageTask + e.target.textContent.replace(/ /g, "")}`;
            $("#goalTaskInput").val(mess);
        };
    });
    $("#closeEmojiWindowBtn").on("click", (e) => {
        $("#choiceEmojiWindow").addClass("d-none");
    });

    // SETTINGS - Remove
    $("#removeIdeaForm").on("submit", (e) => {
        e.preventDefault();
        let idea = e.target.dataset.guid;
        let password = Validate(e.target.getElementsByTagName("input")[0].value);
        $.post("/asyncload/idea/remove", { idea, password }, resp => {
            console.log(resp);
        });
    });

    // SETTINGS - Update
    $("#ideaUpdateForm").on("submit", (e) => {
        e.preventDefault();

        let tags = [];
        $("#updatedTagsContainer button").each((x, e) => {
            tags.push(e.dataset.tag);
        })

        let model = new FormData();
        model.append('avatar', e.target.getElementsByTagName("input")[1].files[0]);
        model.append('description', e.target.getElementsByTagName("textarea")[0].value);
        model.append('status', e.target.getElementsByTagName("select")[0].value);
        model.append('private', e.target.getElementsByTagName("input")[2].checked);
        model.append('idea', e.target.getElementsByTagName("input")[0].value);
        model.append('tags', tags);

        console.log(model);

        let model2 = {
            avatar: e.target.getElementsByTagName("input")[1].files[0],
            description: e.target.getElementsByTagName("textarea")[0].value,
            status: e.target.getElementsByTagName("select")[0].value,
            private: e.target.getElementsByTagName("input")[2].checked,
            idea: e.target.getElementsByTagName("input")[0].value,
            tags: tags
        };

        /*
        $.post("/asyncload/idea/update", { model }, resp => {
            console.log(resp);
        });
        */

        $.ajax({
            url: '/asyncload/idea/update',
            type: 'POST',
            data: model,
            processData: false,
            contentType: false,
            success: (res) => {
                console.log(res);
            }
        });
    })

    // TAGS - Switcher
    sessionStorage.setItem("etc", $("#updatedTagsContainer button").length);
    $(".asyncSelectBtnTag").on("click", (e) => {
        e.preventDefault();
        if (e.target.classList.contains("updatedTag")) {
            e.target.classList.remove("updatedTag");
            $("#updatedTagsContainer").remove(e.target);
            $("#selectTagsContainer").append(e.target);
            let newEtc = Number(sessionStorage.getItem("etc")) - 1;
            sessionStorage.setItem("etc", newEtc);
        } else {
            let currentVal = Number(sessionStorage.getItem("etc"));
            if (currentVal < 5) {
                e.target.classList.add("updatedTag");
                $("#updatedTagsContainer").append(e.target);
                $("#selectTagsContainer").remove(e.target);
                let newEtc = currentVal + 1;
                sessionStorage.setItem("etc", newEtc);
            }
           
        }
    });

    // SETTINGS - Member
    $(".asyncSetDefaultRoleBtn").on("click", (e) => {
        let idea = e.target.closest("div").dataset.idea;
        let user = e.target.closest("div").dataset.user;
        e.target.classList.add("clr-mute");
        e.target.setAttribute("disabled", true);
        e.target.textContent = "Понижен";

        $.post("/asyncload/idea/updaterole", { idea, user, role: 0 }, resp => {
            console.log(resp);
        });
    });

    // ROLES - Modder
    $(".asyncSetModderRoleBtn").on("click", (e) => {
        let idea = e.target.closest("div").dataset.idea;
        let user = e.target.closest("div").dataset.user;

        let btn1 = e.target.closest("div").getElementsByTagName("button")[0];
        btn1.classList.add("clr-mute");
        btn1.setAttribute("disabled", true);
        let btn2 = e.target.closest("div").getElementsByTagName("button")[1];
        btn2.classList.add("clr-mute");
        btn2.setAttribute("disabled", true);
        e.target.textContent = "Повышен";

        $.post("/asyncload/idea/updaterole", { idea, user, role: 1 }, resp => {
            console.log(resp);
        });
    });

    // ROLES - Remove
    $(".asyncRemoveRoleBtn").on("click", (e) => {
        let idea = e.target.closest("div").dataset.idea;
        let user = e.target.closest("div").dataset.user;

        let btn1 = e.target.closest("div").getElementsByTagName("button")[0];
        btn1.classList.add("clr-mute");
        btn1.setAttribute("disabled", true);
        let btn2 = e.target.closest("div").getElementsByTagName("button")[1];
        btn2.classList.add("clr-mute");
        btn2.setAttribute("disabled", true);
        e.target.textContent = "Удалён";

        $.post("/asyncload/idea/removemember", { idea, user }, resp => {
            console.log(resp);
        })
    });    
})