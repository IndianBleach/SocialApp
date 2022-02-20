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

    // TOPIC - Comment
    $("#topicWindowForm").on("submit", (e) => {
        e.preventDefault();
        let base = e.target.getElementsByTagName("input")[0].value;
        let text = Validate(base);
        let guid = e.target.dataset.guid;

        $.post("/asyncload/idea/topiccomment", { guid, text }, resp => {
            console.log(resp);
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

    // TOPIC - Create
    $("#newTopicForm").on("submit", (e) => {
        e.preventDefault();
        let ideaGuid = e.target.dataset.guid;
        let baseName = e.target.getElementsByTagName("input")[0].value;
        let normalizeName = Validate(baseName);
        let desc = e.target.getElementsByTagName("textarea")[0].value;
        let normalizeDesc = Validate(desc);

        console.log(baseName);
        console.log(normalizeName);
        console.log(normalizeDesc);

        $.post("/asyncload/idea/createtopic", { name: normalizeName, content: normalizeDesc, ideaGuid }, resp => {
            console.log(resp);
        })
    })



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

    // EMOJI
    $("#showMessageEmojiBtn").on("click", (e) => {
        e.preventDefault();
        $("#choiceEmojiWindow").removeClass("d-none");
    });
    $(".asyncSendEmojiBtn").on("click", (e) => {
        let messageNow = $("#topicCommentInput").val();
        let mess = `${messageNow + e.target.textContent.replace(/ /g, "")}`;
        $("#topicCommentInput").val(mess);
    });
    $("#closeEmojiWindowBtn").on("click", (e) => {
        $("#choiceEmojiWindow").addClass("d-none");
    });

})