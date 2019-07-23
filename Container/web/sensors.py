from flask import jsonify, request
from flask_restful import Resource

current_data = {"temperature":27, "humidity": 76, "moisture": 25}

class SensorData(Resource):
    def post(self):
        current_data = request.get_json()
    
    def get(self):
        return jsonify(current_data)