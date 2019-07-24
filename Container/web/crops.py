from flask import jsonify, request
from flask_restful import Resource
from mongo import db

crops = db['Crops']

''' Helper Functions '''

def crop_exists(crop_id):
    return not(crops.find({'_id':crop_id}).count()==0)

def get_crop_settings(crop_id):
    if not crop_exists(crop_id):
        return dict()
    try:
        return crops.find({'_id':crop_id})[0]['settings']
    except:
        return dict()

''' API Class '''

class CropSettings(Resource):
    def put(self, crop_id):
        data = request.get_json(force=True)
        crops.find_one_and_update({'_id':crop_id},{'$set':{'settings':data['settings']}})
        return jsonify({
            'status':200,
            'message': 'Crops Settings Updated'
        })

    def get(self, crop_id):
        return get_crop_settings(crop_id)

    def delete(self, crop_id):
        crops.delete_one({'_id':crop_id})
        return jsonify({
            'status':200,
            'message': 'Crops deletion successful'
        })        
    def post(self, crop_id):
        data = request.get_json()
        crops.insert_one(data)
        return jsonify({
            'status':200,
            'message': 'Crops adding successful'
        })

class GetAllCropSettings(Resource):
    def get(self):
        return [crop for crop in crops.find()]