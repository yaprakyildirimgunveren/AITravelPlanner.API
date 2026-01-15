from datetime import date
from typing import List

from fastapi import FastAPI
from pydantic import BaseModel, Field

app = FastAPI(title="AITravelPlanner AI Service", version="1.0.0")


class RecommendationRequest(BaseModel):
    user_id: int = Field(alias="userId")
    from_location: str = Field(alias="from")
    to: str
    travel_date: date = Field(alias="travelDate")
    budget: float
    travelers: int = 1

    class Config:
        populate_by_name = True


class RecommendationItem(BaseModel):
    destination: str
    reason: str
    estimated_cost: float = Field(alias="estimatedCost")
    activities: List[str]

    class Config:
        populate_by_name = True


class RecommendationResponse(BaseModel):
    recommendations: List[RecommendationItem]


@app.get("/health")
def health_check() -> dict:
    return {"status": "ok"}


@app.post("/recommendations", response_model=RecommendationResponse)
def recommend(request: RecommendationRequest) -> RecommendationResponse:
    budget_per_person = request.budget / max(request.travelers, 1)
    base_reason = f"Best match for a {request.travel_date.strftime('%B')} trip"

    recommendations = [
        RecommendationItem(
            destination=request.to,
            reason=f"{base_reason} with direct route from {request.from_location}",
            estimated_cost=round(budget_per_person * 0.9, 2),
            activities=["city tour", "local food", "museum visit"],
        ),
        RecommendationItem(
            destination=f"{request.to} nearby escapes",
            reason="Short trips that fit within your budget",
            estimated_cost=round(budget_per_person * 0.7, 2),
            activities=["nature walk", "day cruise", "local market"],
        ),
    ]

    return RecommendationResponse(recommendations=recommendations)
