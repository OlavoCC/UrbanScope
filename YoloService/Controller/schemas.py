"""
Schemas de saida (contrato da API) -- so estrutura de dados, sem logica.
"""

from typing import List

from pydantic import BaseModel, Field


class Confidence(BaseModel):
    raw: float = Field(..., description="Confiancia em precisao total do modelo (0-1)")
    normalized: float = Field(..., description="Confiancia 0-1 arredondada (4 casas)")


class BBox(BaseModel):
    raw: List[int] = Field(..., description="[x1, y1, x2, y2] em pixels")
    normalized: List[float] = Field(..., description="[x1, y1, x2, y2] normalizado 0-1")


class Detection(BaseModel):
    confidence: Confidence
    bbox: BBox


class DetectResponse(BaseModel):
    have_hole: bool = Field(..., description="Se tem buraco (regra do predict.py)")
    confidence_max: Confidence
    detections: List[Detection]
    processing_time_ms: float
    model_version: str = Field(..., description="Nome do arquivo do modelo sem .pt")
    image_annotated: str = Field(
        ..., description="Imagem com retangulos das deteccoes em data URL (base64)"
    )
