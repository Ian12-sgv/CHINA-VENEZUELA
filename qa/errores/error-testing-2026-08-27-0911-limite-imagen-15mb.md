# [MEDIO] Una imagen de exactamente 15 MB puede ser rechazada antes de la validación funcional

- ID: TEST-2026-08-27-002
- Tipo: Testing
- Estado: En corrección
- Fecha: 2026-08-27 09:11 (UTC-4)
- Módulo: Pedidos / Carga de imágenes
- Entorno: Revisión estática local
- Reportado por: qa-tester

## Descripción

La guía permite imágenes de hasta 15 MB. Sin embargo, los endpoints de carga aplican `RequestSizeLimit(15 * 1024 * 1024)` al cuerpo HTTP completo. En una solicitud multipart, el cuerpo incluye límites y cabeceras además de los bytes del archivo, por lo que una imagen válida de exactamente 15 MB puede superar el límite HTTP y recibir 413 antes de ejecutar la validación de archivo.

## Pasos para reproducir

1. Preparar una imagen JPEG, PNG o WebP válida de exactamente `15 * 1024 * 1024` bytes.
2. Enviar la imagen como `multipart/form-data` a una ruta de carga autenticada.
3. El cuerpo multipart supera el tamaño del archivo por los límites y metadatos.
4. El servidor puede rechazar la petición con 413 antes de llegar a `ValidarImagenAsync`.

## Resultado esperado

Una imagen válida de hasta 15 MB debe llegar a la validación de aplicación. Los archivos mayores a 15 MB deben recibir un error de validación controlado.

## Resultado actual

El límite de transporte y el límite funcional son iguales, por lo que el máximo documentado no está garantizado para cargas multipart.

## Evidencia

- `PedidosController.cs`: `TamanoMaximoImagen = 15 * 1024 * 1024`.
- Las rutas de imagen usan `[RequestSizeLimit(TamanoMaximoImagen)]`.
- La misma constante se compara con `imagen.Length` dentro de `ValidarImagenAsync`.

## Severidad y riesgo

**Medio.** El usuario puede recibir un fallo inesperado al cargar un archivo dentro del límite anunciado; afecta una funcionalidad principal de pedidos.

## Corrección propuesta

Configurar el límite del cuerpo multipart con una tolerancia técnica superior —por ejemplo 16 MB— y conservar la validación estricta de `imagen.Length <= 15 MB` en aplicación. Añadir una prueba de integración para archivo de exactamente 15 MB y otra de 15 MB + 1 byte.

## Corrección aplicada

- Se separó el límite funcional del archivo del límite de transporte multipart mediante `ImagenesUploadLimits`.
- `MaxFileBytes` conserva el máximo de 15 MB para el archivo.
- `MaxRequestBodyBytes` se configuró en 16 MB para que el cuerpo multipart tenga margen para límites y cabeceras.
- Ambos endpoints de carga usan el límite de cuerpo multipart: imagen por tipo y ruta de compatibilidad de producto terminado.
- Se añadieron dos pruebas automatizadas para conservar el máximo funcional y confirmar el margen técnico.

## Verificación de corrección

- Fecha: 2026-08-27.
- Prueba repetida: `dotnet build ChinaVenezuela.slnx --no-restore`, `dotnet test ChinaVenezuela.slnx --no-build`, `npm run build` y `npm run lint`.
- Resultado: compilación backend correcta; 5/5 pruebas .NET aprobadas; build y lint frontend aprobados con dos advertencias preexistentes de hooks.
- Pendiente de cierre: cargar una imagen válida de exactamente 15 MB y otra de 15 MB + 1 byte en un entorno local o de pruebas con API, JWT y base de datos configurados. La primera debe llegar a la validación de aplicación y la segunda debe recibir el error controlado de máximo 15 MB.
- Responsable: arreglo.
