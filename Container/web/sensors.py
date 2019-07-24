from flask import jsonify, request
from flask_restful import Resource
from mongo import db

sensor = db["Sensors"]

class SensorData(Resource):
    def post(self):
        data = request.get_json()
        if sensor.find({'_id':data["_id"]}).count()==0:
            sensor.insert_one(data)
        else:
            sensor.find_one_and_update({'_id':data["_id"]},{'$set':{'temperature':data["temperature"], 'humidity':data["humidity"], 'moisture':data["moisture"]}})
        return jsonify({
            'status':200,
            'message': 'users adding successful'
        })
    
    def get(self, mac_addr=None):
        try:
            if mac_addr is None:
                return [data for data in sensor.find()]
            return sensor.find_one({'_id':mac_addr})
        except:
            return dict()
        