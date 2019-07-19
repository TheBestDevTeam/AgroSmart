from flask import Flask
from flask_restful import Resource, Api
import requests
import json

app = Flask(__name__)
api = Api(app)

container_ip = "http://localhost:5000/CropSettings/{0}"

class RetrieveSettings(Resource):
    def get(self, crop_id):
        response = requests.get(container_ip.format(crop_id))
        if response.status_code >= 400:
            return json.dumps(dict())
        return response.json()

api.add_resource(RetrieveSettings, '/sensornode/<int:crop_id>')

if __name__ == "__main__":
    app.run(host="0.0.0.0", port="80")
