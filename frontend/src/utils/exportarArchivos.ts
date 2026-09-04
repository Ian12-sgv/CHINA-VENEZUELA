export type EntradaZip = { nombre: string; datos: Uint8Array }

export function textoABytes(valor: string) { return new TextEncoder().encode(valor) }
export function dataUrlABytes(url: string) { const base64 = url.slice(url.indexOf(',') + 1); const binario = atob(base64); return Uint8Array.from(binario, (caracter) => caracter.charCodeAt(0)) }
export function escaparXml(valor: string | number) { return String(valor).replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;').replaceAll('"', '&quot;').replaceAll("'", '&apos;') }
export function nombreColumnaXlsx(indice: number) { let resultado = ''; let valor = indice + 1; while (valor > 0) { const resto = (valor - 1) % 26; resultado = String.fromCharCode(65 + resto) + resultado; valor = Math.floor((valor - 1) / 26) } return resultado }
export function celdaXlsx(columna: number, fila: number, valor: string | number, estilo: number) { return `<c r="${nombreColumnaXlsx(columna)}${fila}" t="inlineStr" s="${estilo}"><is><t>${escaparXml(valor)}</t></is></c>` }

function crc32(datos: Uint8Array) { let crc = 0xffffffff; for (const dato of datos) { crc ^= dato; for (let indice = 0; indice < 8; indice++) crc = (crc >>> 1) ^ (0xedb88320 & -(crc & 1)) } return (crc ^ 0xffffffff) >>> 0 }
function entero16(valor: number) { const bytes = new Uint8Array(2); new DataView(bytes.buffer).setUint16(0, valor, true); return bytes }
function entero32(valor: number) { const bytes = new Uint8Array(4); new DataView(bytes.buffer).setUint32(0, valor, true); return bytes }

export function empaquetarZip(entradas: EntradaZip[]) { const locales: Uint8Array[] = []; const centrales: Uint8Array[] = []; let desplazamiento = 0; for (const entrada of entradas) { const nombre = textoABytes(entrada.nombre); const crc = crc32(entrada.datos); const local = new Uint8Array(30 + nombre.length); local.set(entero32(0x04034b50), 0); local.set(entero16(20), 4); local.set(entero16(0x0800), 6); local.set(entero16(0), 8); local.set(entero16(0), 10); local.set(entero16(0), 12); local.set(entero32(crc), 14); local.set(entero32(entrada.datos.length), 18); local.set(entero32(entrada.datos.length), 22); local.set(entero16(nombre.length), 26); local.set(entero16(0), 28); local.set(nombre, 30); locales.push(local, entrada.datos); const central = new Uint8Array(46 + nombre.length); central.set(entero32(0x02014b50), 0); central.set(entero16(20), 4); central.set(entero16(20), 6); central.set(entero16(0x0800), 8); central.set(entero16(0), 10); central.set(entero16(0), 12); central.set(entero16(0), 14); central.set(entero32(crc), 16); central.set(entero32(entrada.datos.length), 20); central.set(entero32(entrada.datos.length), 24); central.set(entero16(nombre.length), 28); central.set(entero16(0), 30); central.set(entero16(0), 32); central.set(entero16(0), 34); central.set(entero16(0), 36); central.set(entero32(0), 38); central.set(entero32(desplazamiento), 42); central.set(nombre, 46); centrales.push(central); desplazamiento += local.length + entrada.datos.length } const tamanoCentral = centrales.reduce((total, parte) => total + parte.length, 0); const fin = new Uint8Array(22); fin.set(entero32(0x06054b50), 0); fin.set(entero16(0), 4); fin.set(entero16(0), 6); fin.set(entero16(entradas.length), 8); fin.set(entero16(entradas.length), 10); fin.set(entero32(tamanoCentral), 12); fin.set(entero32(desplazamiento), 16); fin.set(entero16(0), 20); const partes = [...locales, ...centrales, fin].map((parte) => parte.buffer.slice(parte.byteOffset, parte.byteOffset + parte.byteLength) as ArrayBuffer); return new Blob(partes, { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' }) }

export function descargarBlob(archivo: Blob, nombre: string) { const url = URL.createObjectURL(archivo); const enlace = document.createElement('a'); enlace.href = url; enlace.download = nombre; enlace.click(); URL.revokeObjectURL(url) }

const entradasBaseXlsx = (nombreHoja: string, tieneImagenes: boolean): EntradaZip[] => [
  { nombre: '[Content_Types].xml', datos: textoABytes(`<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/>${tieneImagenes ? '<Default Extension="png" ContentType="image/png"/><Override PartName="/xl/drawings/drawing1.xml" ContentType="application/vnd.openxmlformats-officedocument.drawing+xml"/>' : ''}<Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/><Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/><Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/></Types>`) },
  { nombre: '_rels/.rels', datos: textoABytes('<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/></Relationships>') },
  { nombre: 'xl/workbook.xml', datos: textoABytes(`<?xml version="1.0" encoding="UTF-8" standalone="yes"?><workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><sheets><sheet name="${escaparXml(nombreHoja)}" sheetId="1" r:id="rId1"/></sheets></workbook>`) },
  { nombre: 'xl/_rels/workbook.xml.rels', datos: textoABytes('<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/><Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/></Relationships>') },
  { nombre: 'xl/styles.xml', datos: textoABytes('<?xml version="1.0" encoding="UTF-8" standalone="yes"?><styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><fonts count="2"><font><sz val="10"/><name val="Arial"/></font><font><b/><color rgb="FF173D75"/><sz val="10"/><name val="Arial"/></font></fonts><fills count="3"><fill><patternFill patternType="none"/></fill><fill><patternFill patternType="gray125"/></fill><fill><patternFill patternType="solid"><fgColor rgb="FFEEF4FB"/><bgColor indexed="64"/></patternFill></fill></fills><borders count="2"><border><left/><right/><top/><bottom/><diagonal/></border><border><left style="thin"><color rgb="FFC8D5E6"/></left><right style="thin"><color rgb="FFC8D5E6"/></right><top style="thin"><color rgb="FFC8D5E6"/></top><bottom style="thin"><color rgb="FFC8D5E6"/></bottom><diagonal/></border></borders><cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs><cellXfs count="3"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/><xf numFmtId="0" fontId="1" fillId="2" borderId="1" applyFont="1" applyFill="1" applyBorder="1" applyAlignment="1"><alignment horizontal="center" vertical="center" wrapText="1"/></xf><xf numFmtId="0" fontId="0" fillId="0" borderId="1" applyBorder="1" applyAlignment="1"><alignment vertical="center" wrapText="1"/></xf></cellXfs></styleSheet>') },
]

function xmlDibujoXlsx(imagenes: { datos: Uint8Array; fila: number; columna: number }[]) { return `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><xdr:wsDr xmlns:xdr="http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing" xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">${imagenes.map((imagen, indice) => `<xdr:twoCellAnchor editAs="oneCell"><xdr:from><xdr:col>${imagen.columna}</xdr:col><xdr:colOff>95250</xdr:colOff><xdr:row>${imagen.fila}</xdr:row><xdr:rowOff>47625</xdr:rowOff></xdr:from><xdr:to><xdr:col>${imagen.columna + 1}</xdr:col><xdr:colOff>0</xdr:colOff><xdr:row>${imagen.fila + 1}</xdr:row><xdr:rowOff>0</xdr:rowOff></xdr:to><xdr:pic><xdr:nvPicPr><xdr:cNvPr id="${indice + 1}" name="Imagen ${indice + 1}"/><xdr:cNvPicPr/></xdr:nvPicPr><xdr:blipFill><a:blip r:embed="rId${indice + 1}"/><a:stretch><a:fillRect/></a:stretch></xdr:blipFill><xdr:spPr><a:xfrm><a:off x="0" y="0"/><a:ext cx="0" cy="0"/></a:xfrm><a:prstGeom prst="rect"><a:avLst/></a:prstGeom></xdr:spPr></xdr:pic><xdr:clientData/></xdr:twoCellAnchor>`).join('')}</xdr:wsDr>` }

export async function convertirImagenAPng(blob: Blob) {
  const temporal = URL.createObjectURL(blob)
  try {
    const imagen = await new Promise<HTMLImageElement>((resolver, rechazar) => {
      const elemento = new Image()
      elemento.onload = () => resolver(elemento)
      elemento.onerror = () => rechazar(new Error('No fue posible procesar la imagen.'))
      elemento.src = temporal
    })
    const maximo = 1000
    const escala = Math.min(1, maximo / Math.max(imagen.naturalWidth, imagen.naturalHeight))
    const lienzo = document.createElement('canvas')
    lienzo.width = Math.max(1, Math.round(imagen.naturalWidth * escala))
    lienzo.height = Math.max(1, Math.round(imagen.naturalHeight * escala))
    lienzo.getContext('2d')?.drawImage(imagen, 0, 0, lienzo.width, lienzo.height)
    return lienzo.toDataURL('image/png')
  } finally { URL.revokeObjectURL(temporal) }
}

export function descargarExcelConImagen(filas: Record<string, string | number>[], nombreArchivo: string, columnaImagen: string, imagenesPorFila: (string | null)[], nombreHoja = 'Datos') {
  const encabezados = Object.keys(filas[0] ?? {})
  const indiceImagen = encabezados.indexOf(columnaImagen)
  const referenciasImagenes: { datos: Uint8Array; fila: number; columna: number }[] = []
  imagenesPorFila.forEach((imagen, indice) => { if (imagen && indiceImagen >= 0) referenciasImagenes.push({ datos: dataUrlABytes(imagen), fila: indice + 1, columna: indiceImagen }) })
  const tieneImagenes = referenciasImagenes.length > 0
  const columnasXml = encabezados.map((encabezado, indice) => `<col min="${indice + 1}" max="${indice + 1}" width="${encabezado === columnaImagen ? 24 : 18}" customWidth="1"/>`).join('')
  const filasXml = [encabezados, ...filas.map((fila) => encabezados.map((encabezado) => fila[encabezado] ?? ''))].map((fila, indiceFila) => {
    const celdas = fila.map((valor, indiceColumna) => celdaXlsx(indiceColumna, indiceFila + 1, valor, indiceFila === 0 ? 1 : 2)).join('')
    const atributos = indiceFila === 0 ? '' : ' ht="70" customHeight="1"'
    return `<row r="${indiceFila + 1}"${atributos}>${celdas}</row>`
  }).join('')
  const hoja = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><dimension ref="A1:${nombreColumnaXlsx(encabezados.length - 1)}${filas.length + 1}"/><sheetViews><sheetView workbookViewId="0"/></sheetViews><cols>${columnasXml}</cols><sheetData>${filasXml}</sheetData>${tieneImagenes ? '<drawing r:id="rId1"/>' : ''}</worksheet>`
  const entradas: EntradaZip[] = [...entradasBaseXlsx(nombreHoja, tieneImagenes), { nombre: 'xl/worksheets/sheet1.xml', datos: textoABytes(hoja) }]
  if (tieneImagenes) {
    entradas.push(
      { nombre: 'xl/worksheets/_rels/sheet1.xml.rels', datos: textoABytes('<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing" Target="../drawings/drawing1.xml"/></Relationships>') },
      { nombre: 'xl/drawings/drawing1.xml', datos: textoABytes(xmlDibujoXlsx(referenciasImagenes)) },
      { nombre: 'xl/drawings/_rels/drawing1.xml.rels', datos: textoABytes(`<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">${referenciasImagenes.map((_, indice) => `<Relationship Id="rId${indice + 1}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/image" Target="../media/image${indice + 1}.png"/>`).join('')}</Relationships>`) },
    )
    referenciasImagenes.forEach((imagen, indice) => entradas.push({ nombre: `xl/media/image${indice + 1}.png`, datos: imagen.datos }))
  }
  descargarBlob(empaquetarZip(entradas), nombreArchivo)
}

export function descargarExcelSimple(filas: Record<string, string | number>[], nombreArchivo: string, nombreHoja = 'Datos') {
  const encabezados = Object.keys(filas[0] ?? {})
  const filasXml = [encabezados, ...filas.map((fila) => encabezados.map((encabezado) => fila[encabezado] ?? ''))].map((fila, indiceFila) => {
    const celdas = fila.map((valor, indiceColumna) => celdaXlsx(indiceColumna, indiceFila + 1, valor, indiceFila === 0 ? 1 : 2)).join('')
    return `<row r="${indiceFila + 1}">${celdas}</row>`
  }).join('')
  const columnasXml = encabezados.map((_, indice) => `<col min="${indice + 1}" max="${indice + 1}" width="20" customWidth="1"/>`).join('')
  const hoja = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><dimension ref="A1:${nombreColumnaXlsx(encabezados.length - 1)}${filas.length + 1}"/><sheetViews><sheetView workbookViewId="0"/></sheetViews><cols>${columnasXml}</cols><sheetData>${filasXml}</sheetData></worksheet>`
  const entradas: EntradaZip[] = [...entradasBaseXlsx(nombreHoja, false), { nombre: 'xl/worksheets/sheet1.xml', datos: textoABytes(hoja) }]
  descargarBlob(empaquetarZip(entradas), nombreArchivo)
}
