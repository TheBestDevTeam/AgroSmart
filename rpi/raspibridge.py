from flask import Flask
from flask_restful import Resource, Api
import numpy as np
import requests
import json

app = Flask(__name__)
api = Api(app)

container_ip = "http://sribs-ubuntu:5000/{0}/{1}"

class RetrieveSettings(Resource):
    def get(self, crop_id):
        response = requests.get(container_ip.format("CropSettings",crop_id))
        if response.status_code >= 400:
            return json.dumps(dict())
        return response.json()

class UserLocationDetails(Resource):
    def get(self, user_id):
        response = requests.get(container_ip.format("UserSettings",user_id))
        if response.status_code >= 400:
            return json.dumps(dict())
        return response.json()

class PostSensorData(Resource):
    def post(self):
        data = request.get_json()
        
        response = requests.post(container_ip.format("SensorData",""),json=data)

api.add_resource(RetrieveSettings, '/sensornode/<int:crop_id>')
api.add_resource(PostSensorData, '/SensorData/')

if __name__ == "__main__":
    app.run(host="0.0.0.0", port="80")
