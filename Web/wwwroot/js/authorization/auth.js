$(document).ready(() => {

    //userSignupTagsForm, userSignupInfoForm

    // Sign In
    $("#userSignInForm").on("submit", e => {
        e.preventDefault();

        let inputs = $("#userSignInForm input");

        let model = {
            username: inputs[0].value,
            password: inputs[1].value,
        };

        $.post("/authorize/signin", { model }, res => {
            if (res.isRedirect == true) {
                window.location.href = res.redirectUrl
            }
            else {
                console.log(res.message);
            }
        });
    });


    // Sign Up
    $("#userSignupTagsForm").on("submit", e => {
        e.preventDefault();

        let inputs = $("#userSignupInfoForm input");

        let selectedTags = [];

        $("#selectedTags button").each((x, elem) => {
            selectedTags.push(elem.dataset.tag);
        });

        let model = {
            username: inputs[0].value,
            password: inputs[1].value,
            confirmPassword: inputs[2].value,
            tags: selectedTags
        };

        $.post("/authorize/signup", { model }, res => {
            console.log(res.message);
            if (res.isRedirect == true) {
                window.location.href = res.redirectUrl
            }
            else {
                console.log(res.message);
            }
        });
    });

});
