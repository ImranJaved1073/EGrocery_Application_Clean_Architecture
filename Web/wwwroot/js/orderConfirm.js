// Import the SignalR library

<>// Import the SignalR library
    <script src="https://cdnjs.cloudflare.com/ajax/libs/toastify-js/1.12.0/toastify.min.js"></script><script src="~/microsoft/signalr/dist/browser/signalr.js"></script></>

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/orderNotificationHub")
    .build();

// Start the connection
connection.start().then(function () {
    console.log("SignalR connected");
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveOrderStatus", function (orderNumber, status) {
    const message = `Your order ${orderNumber} status is now: ${status}`;
    console.log(message);
    // Display the toast notification.
    Toastify({
        text: message,
        duration: 5000,
        close: true,
        gravity: "top", // top or bottom
        position: "right", // left, center or right
        backgroundColor: "#4fbe87",
    }).showToast();
});


// Example of manually triggering an order status update (for testing purposes)
// connection.invoke("NotifyOrderStatusChange", "ORDER1234", "Completed")
//     .catch(err => console.error(err.toString()));
