
var messengerNames = [];
var messengerUsageCounts = [];
$.ajax({
    type: "GET",
    url: "get_top_messengers/5",
    contentType: "application/json; charset=utf-8",
    success: function(data) {
        $.each(JSON.parse(data), function(i, obj){
            messengerNames.push(obj.MessengerName);
            messengerUsageCounts.push(obj.MessengerUsageCount);
        });

        new Chart(document.getElementById("bar-chart"), {
            type: 'bar',
            data: {
              labels: messengerNames,
              datasets: [
                {
                  label: "Number of usage",
                  backgroundColor: ["#3e95cd", "#8e5ea2","#3cba9f","#e8c3b9","#c45850"],
                  data: messengerUsageCounts
                }
              ]
            },
            options: {
              legend: { display: false },
              title: {
                display: true,
                text: 'Top 5 most used messengers'
              }
            }
        });
    }

});

