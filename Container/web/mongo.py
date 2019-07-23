from pymongo import MongoClient

mongo_client = MongoClient('mongodb://agrosmart:27017')
db = mongo_client.projectDB