$(document).ready(() => {
    // topicDetailAuthorLink, topicDetailName, topicDetailAuthorName
    // topicDetailDescription


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

                resp.comments.forEach(x => {
                    $("#topicWindowCommentsLoad").append(`<div class="topicMessage"><img src="../media/userAvatars/${x.authorAvatar}" /><div><a href="/user/${x.authorGuid}">${x.authorName}<span class="text-muted"> - ${x.datePublished}</span></a><br />${x.comment}</div></div>`);
                })                
            }
        })
    });

    // TOPIC - Comment
    $("#topicWindowForm").on("submit", (e) => {
        e.preventDefault();
        let text = e.target.getElementsByTagName("input")[0].value;
        let guid = e.target.dataset.guid;

        $.post("/asyncload/idea/topiccomment", { guid, text }, resp => {
            console.log(resp);
            let avatar = $("#topicCommentAvatar").attr("src");
            $("#topicWindowCommentsLoad").append(`<div class="topicMessage"><img src="${avatar}" /><div><a disabled>Вы<span class="text-muted"> - сегодня</span></a><br />${text}</div></div>`);            
        })
    })

    // TOPIC - Close
    $(".closeTopicWindowBtn").on("click", (e) => {
        $("#hideBackgroundWrapper").addClass("d-none");
        $("body").removeClass("overflow-hidden");
        $("#topicWindowWrapper").addClass("d-none");
        $(".topicMessage").remove();
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