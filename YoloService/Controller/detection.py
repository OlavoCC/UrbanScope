"""
Controller: recebe e retorna dados -- sem logica de negocio.
"""

import base64

from fastapi import APIRouter, File, HTTPException, UploadFile

from Controller import schemas
from Services import predict

router = APIRouter()


@router.post("/Api/YOLO/detect", response_model=schemas.DetectResponse)
async def detect(image: UploadFile = File(..., description="Foto da via (jpg/png/webp)")):
    data = await image.read()
    if not data:
        raise HTTPException(status_code=400, detail="Imagem vazia.")

    try:
        result = predict.run(data)
    except RuntimeError as exc:  # modelo nao carregado
        raise HTTPException(status_code=503, detail=str(exc)) from exc
    except ValueError as exc:  # imagem invalida
        raise HTTPException(status_code=400, detail=str(exc)) from exc

    # Monta a resposta (so formatacao de dados)
    detections = []
    for det in result.detections:
        x1, y1, x2, y2 = det.bbox
        detections.append(
            schemas.Detection(
                confidence=schemas.Confidence(
                    raw=det.confidence,
                    normalized=round(det.confidence, 4),
                ),
                bbox=schemas.BBox(
                    raw=[x1, y1, x2, y2],
                    normalized=[
                        round(x1 / result.width, 4),
                        round(y1 / result.height, 4),
                        round(x2 / result.width, 4),
                        round(y2 / result.height, 4),
                    ],
                ),
            )
        )

    conf_max = max((d.confidence for d in result.detections), default=0.0)

    return schemas.DetectResponse(
        have_hole=result.have_hole,
        confidence_max=schemas.Confidence(
            raw=conf_max,
            normalized=round(conf_max, 4),
        ),
        detections=detections,
        processing_time_ms=result.processing_time_ms,
        model_version=predict.model_version(),
        image_annotated=_data_url(result.annotated_jpeg, "image/jpeg"),
    )



def _data_url(content: bytes, mime: str) -> str:
    return f"data:{mime};base64,{base64.b64encode(content).decode('ascii')}"


def _sniff_mime(data: bytes, declared: str | None) -> str:
    """Descobre o tipo real da imagem (o navegador/curl manda errado as vezes)."""
    if declared and declared.startswith("image/"):
        return declared
    if data[:3] == b"\xff\xd8\xff":
        return "image/jpeg"
    if data[:8] == b"\x89PNG\r\n\x1a\n":
        return "image/png"
    if data[:4] == b"RIFF" and data[8:12] == b"WEBP":
        return "image/webp"
    if data[:2] == b"BM":
        return "image/bmp"
    return "image/jpeg"
