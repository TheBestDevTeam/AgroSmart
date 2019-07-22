from azure.cognitiveservices.vision.customvision.prediction import CustomVisionPredictionClient

ENDPOINT = "https://southcentralus.api.cognitive.microsoft.com"

# Replace with a valid key
training_key = "bf131f1c88cb4e79a0b7bb441132bd15"
prediction_key = "d8268eeac89e4b96be46861ebe1289b2"
prediction_resource_id = "/subscriptions/27b750cd-ed43-42fd-9044-8d75e124ae55/resourceGroups/simrat/providers/Microsoft.CognitiveServices/accounts/simrat_prediction"


predictor = CustomVisionPredictionClient(prediction_key, endpoint=ENDPOINT)

with open("test_image.jpg", "rb") as image_contents:
    results = predictor.classify_image("fc87789f-6871-4fba-b176-d538c9a26d1a", "sisatia-image-classifier", image_contents.read())

    # Display the results.
    for prediction in results.predictions:
        print("\t" + prediction.tag_name +
              ": {0:.2f}%".format(prediction.probability * 100))

