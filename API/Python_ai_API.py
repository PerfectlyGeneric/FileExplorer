import io
import torch
from typing import List
from fastapi import FastAPI, File, UploadFile, Form
from PIL import Image
from transformers import CLIPModel, CLIPProcessor


# path to local model
model_path = r"E:\Proj_enter\FileExplorer\Models"
base_tags = ["cat", "dog", "person", "car", "tree", "building", "indoor", "outdoor", "food", "animal", "landscape"]

app = FastAPI()

print("Loading model...")
device = "cuda" if torch.cuda.is_available() else "cpu"

try:
    model = CLIPModel.from_pretrained(model_path).to(device)
    processor = CLIPProcessor.from_pretrained(model_path)
    print("Model loaded.")
except Exception as e:
    print(f"Failed to load model: {e}")

@app.post("/tag-image")
def tag_image(
    file: UploadFile = File(...), 
    candidate_tags: List[str] = Form(...) # expecting a comma separated string e.g. "cat, dog, plane"
):
    # read the image file
    try:
        contents = file.file.read()
        image = Image.open(io.BytesIO(contents)).convert("RGB")
    except:
        return {"error": "Could not read image"}

    # prepare inputs for the model
    try:
        inputs = processor(text=candidate_tags, images=image, return_tensors="pt", padding=True)
        inputs = inputs.to(device)

        with torch.no_grad():
            outputs = model(**inputs)
            logits = outputs.logits_per_image 
            probs = logits.softmax(dim=1)

        probs_list = probs.cpu().numpy()[0]
        results = []

        # loop through and apply penalty if needed
        for i in range(len(candidate_tags)):
            tag_name = candidate_tags[i]
            score = probs_list[i]

            if tag_name in base_tags:
                score = score - 0.15
        
            results.append({"tag": tag_name, "score": float(score)})

         # sort the list by score
        results.sort(key=lambda x: x["score"], reverse=True)

        return {"tags_ranked": results}

    except Exception as e:
        print(f"Inference error: {e}")