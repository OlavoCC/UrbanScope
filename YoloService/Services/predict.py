"""
Logica de inferencia -- portada do scripts/inference/predict.py do projeto C:\\YOLO.

Regras mantidas:
    - CONF_THRESHOLD = 0.55 (deteccoes abaixo disso nem aparecem)
    - have_hole = qualquer deteccao sobrou
"""

import time
from dataclasses import dataclass
from pathlib import Path
from typing import List, Optional

import cv2
import numpy as np
from ultralytics import YOLO

BASE_DIR = Path(__file__).resolve().parent.parent  # C:\UrbanScope\YoloService
MODELS_DIR = BASE_DIR / "Models"

CONF_THRESHOLD = 0.55

_model: Optional[YOLO] = None
_model_version: str = ""


@dataclass
class Detection:
    confidence: float   # bruto do modelo (0-1)
    bbox: List[int]     # [x1, y1, x2, y2] em pixels


@dataclass
class PredictResult:
    have_hole: bool
    detections: List[Detection]
    width: int
    height: int
    processing_time_ms: float
    annotated_jpeg: bytes   # imagem com retangulos desenhados (jpeg)


def load_model() -> bool:
    """Carrega o .pt mais recente da pasta Models/."""
    global _model, _model_version

    if not MODELS_DIR.exists():
        print(f"[ERRO] Pasta do modelo nao existe: {MODELS_DIR}")
        return False

    files = list(MODELS_DIR.glob("*.pt"))
    if not files:
        print(f"[ERRO] Nenhum .pt encontrado em {MODELS_DIR}")
        return False

    path = max(files, key=lambda p: p.stat().st_mtime)  # modelo mais recente
    _model = YOLO(str(path))
    _model_version = path.stem  # nome inteiro do arquivo, sem .pt
    print(f"[OK] Modelo carregado: {path.name}")
    return True


def model_version() -> str:
    return _model_version


def run(image_bytes: bytes) -> PredictResult:
    """Roda a inferencia e devolve deteccoes + imagem anotada."""
    if _model is None:
        raise RuntimeError("Modelo nao carregado.")

    # Decodifica os bytes em imagem (BGR) -- ultralytics nao aceita bytes direto
    img = cv2.imdecode(np.frombuffer(image_bytes, np.uint8), cv2.IMREAD_COLOR)
    if img is None:
        raise ValueError("Nao foi possivel decodificar a imagem.")

    start = time.perf_counter()
    results = _model.predict(source=img, conf=CONF_THRESHOLD, verbose=False)
    elapsed = (time.perf_counter() - start) * 1000

    detections: List[Detection] = []
    width = height = 0
    annotated_jpeg = b""

    for r in results:
        height, width = r.orig_shape[:2]

        for box in r.boxes:
            x1, y1, x2, y2 = box.xyxy[0].tolist()
            detections.append(
                Detection(
                    confidence=float(box.conf[0]),
                    bbox=[round(x1), round(y1), round(x2), round(y2)],
                )
            )

        # Imagem com os retangulos ja desenhados (plot do ultralytics)
        annotated = r.plot()
        ok, buffer = cv2.imencode(".jpg", annotated)
        if ok:
            annotated_jpeg = buffer.tobytes()

    # Regra do predict.py: deteccoes ja vem filtradas pelo CONF_THRESHOLD
    have_hole = len(detections) > 0

    return PredictResult(
        have_hole=have_hole,
        detections=detections,
        width=width,
        height=height,
        processing_time_ms=round(elapsed, 2),
        annotated_jpeg=annotated_jpeg,
    )
