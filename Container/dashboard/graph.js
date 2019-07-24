function rand() {
    return Math.random();
  }
  
  var time = new Date();
  
  var data1 = [{
    x: [time],
    y: 25,
    mode: 'lines',
    line: {color: '#00A8A9'}
  }]

  var data2 = [{
    x: [time],
    y: 61,
    mode: 'lines',
    line: {color: '#00A8A9'}
  }]

  var data3 = [{
    x: [time],
    y: 1,
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
  
  var cnt = 0;
  
  var interval = setInterval(function() {
  
    var time = new Date();
  
    $.getJSON("http://localhost:5000/SensorData")
  
    Plotly.extendTraces('grapht', update, [0])
    Plotly.extendTraces('graph2', update, [0])
    Plotly.extendTraces('graph3', update, [0])
  
    if(cnt === 100) clearInterval(interval);
  }, 3000);