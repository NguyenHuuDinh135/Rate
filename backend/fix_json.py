import json

mapping = {
    "user2Id": "2b6dfb56-8a4d-4df7-8b9f-b2d0a18bb58e",
    "user3Id": "72c94147-cb3a-4f80-b015-9acb882bc5c0",
    "user4Id": "ce129c6e-1990-4db8-97e2-e4daec6c8d10"
}

for filename in ["src/Infrastructure/Data/SeedData/Bookings.json", "src/Infrastructure/Data/SeedData/Payments.json"]:
    with open(filename, 'r') as f:
        data = json.load(f)
    
    for item in data:
        if "UserKey" in item:
            user_key = item["UserKey"]
            if user_key in mapping:
                item["UserId"] = mapping[user_key]
            del item["UserKey"]
            
    with open(filename, 'w') as f:
        json.dump(data, f, indent=2)

print("Fixed JSON files")
