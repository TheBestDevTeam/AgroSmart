var rain_percent = {"moderate rain":50, "heavy rain":75, "less rain":25};
$.getJSON("https://api.openweathermap.org/data/2.5/forecast?zip=500050,IN&units=metric&appid=232439050c541defc0696cb23a1e1ae2", function(data){
    console.log(data["list"].length)
    for(var i =0; i< data["list"].length; i++)
    {
        var curr_date = new Date()
        if (data["list"][i]["dt_txt"] > curr_date)
        {
            console.log("Entered")
            document.getElementById("weather").innerHTML = data["list"][i]["weather"][0]["description"]
            document.getElementById("temp").innerHTML = data["list"][i]["main"]["temp"]
            document.getElementById("humidity").innerHTML = data["list"][i]["main"]["humidity"]
            document.getElementById("rain").innerHTML = rain_percent[data["list"][i]["weather"][0]["description"]]
            break;
        }
        console.log(curr_date)
    }
});
