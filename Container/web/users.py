from flask import jsonify, request
from flask_restful import Resource
from mongo import db

users = db['Users']

def user_exists(user_id):
    return not(users.find({'_id':user_id}).count()==0)

def get_user_details(user_id):
    if not user_exists(user_id):
        return dict()
    try:
        return users.find_one({'_id':user_id})
    except:
        return dict()

class UserDetails(Resource):
    def get(self, user_id):
        return get_user_details(user_id)

    def post(self, user_id):
        data = request.get_json()
        data["_id"] = user_id
        users.insert_one(data)
        return jsonify({
            'status':200,
            'message': 'users adding successful'
        })

class Location
