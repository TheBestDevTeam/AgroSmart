var sensor_api="http://agrosmartservice.southindia.cloudapp.azure.com:9000/api/device/";
var user_api = "http://agrosmartservice.southindia.cloudapp.azure.com:9000/api/user/";
var status_api = "http://agrosmartservice.southindia.cloudapp.azure.com:9000/api/devicestatus/";
var rain_percent = {"moderate rain":"50%", "heavy rain":"75%", "light rain":"25%"};
var mac_address = "";

function rand() {
    return Math.random();
}

function populateUserData() {
    var username = document.getElementById("username").value;
    console.log(username)
    $.getJSON(user_api+username,function(data){
        console.log(username)
        var select = document.getElementById("selectmac"); 
        var options = data["deviceList"]; 
    
        for(var i = 0; i < options.length; i++) {
            var opt = options[i];
            var el = document.createElement("option");
            el.textContent = opt;
            el.value = opt;
            select.appendChild(el);
        }

        data = data["forecastData"]
        document.getElementById("weather").innerHTML = data["list"][2]["weather"][0]["description"]
        document.getElementById("temp").innerHTML = data["list"][2]["main"]["temp"]
        document.getElementById("humidity").innerHTML = data["list"][2]["main"]["humidity"].toString()+"%"
        document.getElementById("rain").innerHTML = rain_percent[data["list"][2]["weather"][0]["description"]]
    });
}
var time = new Date();

function initplot()
{
    $.getJSON(sensor_api, function(data){
        var data1 = [{
            x: [time],
            y: [data["temperature"]],
            mode: 'lines',
            line: {color: '#00A8A9'}
        }]

        var data2 = [{
            x: [time],
            y: [data["humidity"]],
            mode: 'lines',
            line: {color: '#00A8A9'}
        }]

        var data3 = [{
            x: [time],
            y: [data["moisture"]],
            mode: 'lines',
            line: {color: '#00A8A9'}
        }]
        
        var layout = {
            title: 'Temperature Sensor',
            xaxis: {
                title: 'Timestamp'
            },
            yaxis: {
                title: 'Temperature'
            }
        };

        var layout2 = {
            title: 'Humidity Sensor',
            xaxis: {
                title: 'Timestamp'
            },
            yaxis: {
                title: 'Humidity'
            }
        };

        var layout3 = {
            title: 'Moisture Sensor',
            xaxis: {
                title: 'Timestamp'
            },
            yaxis: {
                title: 'Moisture'
            }
        };
        
        Plotly.newPlot('grapht', data1, layout);
        Plotly.newPlot('graph2', data2, layout2);
        Plotly.newPlot('graph3', data3, layout3);
    });
}


function updatePlot() {
    var cnt=0;
    var interval = setInterval(function() {
    
    var time = new Date();
    
    $.getJSON(sensor_api, function(data){
        var update_t = {
            x:  [[time]],
            y: [[data["temperature"]]]
            };
        var update_h = {
            x:  [[time]],
            y: [[data["humidity"]]]
            };     
        var update_m = {
            x:  [[time]],
            y: [[data["moisture"]]]
            };
        document.getElementById("temperature").innerHTML = data["temperature"].toString() + " &deg;C"
        Plotly.extendTraces('grapht', update_t, [0])
        Plotly.extendTraces('graph2', update_h, [0])
        Plotly.extendTraces('graph3', update_m, [0])
    });
    if(cnt === 100) clearInterval(interval);
    }, 20000);
}

var stat_interval = setInterval(function() {
    $.getJSON(status_api, function(data) {
        document.getElementById('status').innerHTML = data["status"]
    });
}, 10000);


function apiDisplay() {
    mac_address = document.getElementById("selectmac").value;
    sensor_api = sensor_api + mac_address
    status_api = status_api + mac_address
    document.getElementById('display').innerHTML = mac_address
    initplot();
    updatePlot();
}