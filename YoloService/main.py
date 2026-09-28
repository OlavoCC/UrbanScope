"""
UrbanScope - YoloService

Ponto de entrada: sobe a FastAPI e carrega o modelo YOLO (pasta Models/)
na inicializacao.

Rodar:
    uvicorn main:app --host 0.0.0.0 --port 8000
"""

from contextlib import asynccontextmanager

from fastapi import FastAPI

from Controller.detection import router as detection_router
from Services import predict


@asynccontextmanager
async def lifespan(_: FastAPI):
    # Startup: carrega o .pt da pasta Models/
    if not predict.load_model():
        print("[AVISO] Servico iniciado SEM modelo -- /detect vai retornar 503.")
    yield
    # Shutdown: nada a limpar por enquanto


app = FastAPI(
    title="UrbanScope - YoloService",
    description="Servico de deteccao de buracos (YOLOv8)",
    version="1.0.0",
    lifespan=lifespan,
)

app.include_router(detection_router)
