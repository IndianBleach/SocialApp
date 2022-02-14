$(document).ready(() => {
    //idea
    $("#asyncNewIdeaForm").on("submit", e => {
        e.preventDefault();

        let selectedTags = [];
        $("#selectedNewIdeaTags button").each((e, elem) => {
            selectedTags.push(elem.dataset.tag);
        });

        let model = {
            authorGuid: "",
            name: e.target.getElementsByTagName("input")[0].value,
            description: e.target.getElementsByTagName("textarea")[0].value,
            tags: selectedTags,
            isPrivate: e.target.getElementsByTagName("input")[1].checked,
        };

        console.log(model);

        $.post("/create/idea", { model }, resp => {
            console.log(resp);
        });
    });
})