from flask import Flask, jsonify
from flask_restful import Api, Resource
from flask_cors import CORS
from crops import CropSettings, GetAllCropSettings
from sensors import SensorData
import json

app = Flask(__name__)
cors = CORS(app, resources={r"/*": {"origins": "*"}})
api = Api(app=app)

class Entry(Resource):
    def get(self):
        return jsonify({'status':200, 'message':'Welcome to Crop Settings API. Please refer README.md for reference'})

api.add_resource(Entry, '/')
api.add_resource(CropSettings, '/CropSettings/<int:crop_id>')
api.add_resource(GetAllCropSettings, '/CropSettings/')
api.add_resource(SensorData, '/SensorData/')

if __name__ == '__main__':
    app.run(host='0.0.0.0',port=80,debug=True)