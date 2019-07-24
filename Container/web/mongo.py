from pymongo import MongoClient

mongo_client = MongoClient('mongodb://agrosmartdb.westus.azurecontainer.io:27017')
db = mongo_client.projectDB