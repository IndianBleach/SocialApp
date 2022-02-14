$(document).ready(() => {
    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    let _connectionId = '';

    connection.on("RecieveMessage", (message) => {
        console.log(message);
    })

    connection.start()
        .then(() => {
            console.log("Connect open");
            _connectionId = connection.invoke("getConnectionId");
        })
        .catch(() => {
            console.log("Connect failed");
        });

    // Chat - Recieve
    connection.on("RecieveMessage", (message) => {

        console.log(message);

    });

    const joinChat = (chatGuid) => {
        let url = "chat/join/" + _connectionId + "/" + chatGuid;
        $.post(url, null, (resp) => {
            console.log("Join chat stabile", resp);
        })
    };
})