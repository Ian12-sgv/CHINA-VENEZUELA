import { useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { ConfirmDeleteDialog } from '../components/ConfirmDeleteDialog'
import { faCircleCheck, faFileExcel, faMagnifyingGlass, faTriangleExclamation } from '@fortawesome/free-solid-svg-icons'
import { faFilePdf } from '@fortawesome/free-regular-svg-icons'
import { jsPDF } from 'jspdf'
import autoTable from 'jspdf-autotable'
import { catalogosApi, comprasApi, receptoresApi } from '../api'
import { convertirImagenAPng, descargarExcelConImagen } from '../utils/exportarArchivos'
import type { Catalogo, CompraRecibida, CompraRecibidaRequest, CompraStatus } from '../types'

const emptyForm: CompraRecibidaRequest = { contenedorCompartidoId: null, nombreContenedor: '', numeroContenedor: '', empresaId: '', descripcion: null, fechaSalida: '', fechaLlegada: null, aduana: null, puertoLlegada: '', marcaBultoId: null, receptorCodigoUsuario: null }
const date = (value: string | null) => value ? new Date(`${value}T00:00:00`).toLocaleDateString('es-VE') : '-'
const estadosCompra: CompraStatus[] = ['En proceso', 'Aprobado', 'Sin terminar']
const columnaArchivoComprobante = 'Comprobante cargado'
const filaExportableCompra = (item: CompraRecibida, nombreEmpresa: string, nombreMarca: string, tieneImagen: boolean): Record<string, string> => ({
  'Contenedor': item.nombreContenedor,
  'Numero de contenedor': item.numeroContenedor,
  'Empresa': nombreEmpresa,
  'Fecha de salida': date(item.fechaSalida),
  'Fecha de llegada': date(item.fechaLlegada),
  'Puerto de llegada': item.puertoLlegada,
  'Aduana': item.aduana ?? 'No aplica',
  'Marca de bulto': nombreMarca,
  'Receptor': item.receptorNombre ?? 'Sin asignar',
  'Status': item.status,
  'Descripcion': item.descripcion ?? '',
  'Comprobante enviado': item.fechaComprobanteEnviadoUtc ? 'Si' : 'No',
  [columnaArchivoComprobante]: tieneImagen ? '' : item.nombreArchivoComprobante ?? 'Sin archivo',
})

async function cargarImagenComprobante(item: CompraRecibida): Promise<string | null> {
  if (!item.nombreArchivoComprobante || !item.tipoContenidoArchivoComprobante?.startsWith('image/')) return null
  const url = await comprasApi.obtenerArchivoComprobante(item.id)
  if (!url) return null
  try {
    const respuesta = await fetch(url)
    if (!respuesta.ok) return null
    return await convertirImagenAPng(await respuesta.blob())
  } finally { URL.revokeObjectURL(url) }
}

export function ComprasRecibidasPage() {
  const client = useQueryClient(); const [form, setForm] = useState<CompraRecibidaRequest>(emptyForm); const [editingId, setEditingId] = useState<string | null>(null); const [deleteTarget, setDeleteTarget] = useState<CompraRecibida | null>(null); const [error, setError] = useState(''); const [success, setSuccess] = useState(''); const [search, setSearch] = useState(''); const [fechaLlegadaFiltro, setFechaLlegadaFiltro] = useState(''); const [comprobantesEnviando, setComprobantesEnviando] = useState<Set<string>>(() => new Set()); const [archivoComprobantePendiente, setArchivoComprobantePendiente] = useState<File | null>(null); const [exportando, setExportando] = useState<'excel' | 'pdf' | null>(null); const [mostrarRango, setMostrarRango] = useState(false); const [rangoDesde, setRangoDesde] = useState(''); const [rangoHasta, setRangoHasta] = useState('')
  useEffect(() => { if (!success) return; const timeout = window.setTimeout(() => setSuccess(''), 2000); return () => window.clearTimeout(timeout) }, [success])
  useEffect(() => { if (!error) return; const timeout = window.setTimeout(() => setError(''), 5000); return () => window.clearTimeout(timeout) }, [error])
  const compras = useQuery({ queryKey: ['compras-recibidas'], queryFn: comprasApi.listar, refetchOnMount: 'always' })
  const empresas = useQuery({ queryKey: ['empresas'], queryFn: () => catalogosApi.listar('empresas') })
  const marcas = useQuery({ queryKey: ['marcas-bulto'], queryFn: () => catalogosApi.listar('marcas-bulto') })
  const contenedores = useQuery({ queryKey: ['contenedores-compartidos'], queryFn: () => catalogosApi.listar('contenedores-compartidos') })
  const aduanas = useQuery({ queryKey: ['aduanas'], queryFn: () => catalogosApi.listar('aduanas') })
  const puertos = useQuery({ queryKey: ['puertos-llegada'], queryFn: () => catalogosApi.listar('puertos-llegada') })
  const receptores = useQuery({ queryKey: ['receptores'], queryFn: receptoresApi.listar })
  const refresh = () => client.invalidateQueries({ queryKey: ['compras-recibidas'] })
  const reset = () => { setForm(emptyForm); setEditingId(null); setError(''); setArchivoComprobantePendiente(null) }
  const save = useMutation({
    mutationFn: () => editingId ? comprasApi.actualizar(editingId, form) : comprasApi.crear(form),
    onSuccess: async (compra) => {
      let mensaje = ''
      if (!editingId && archivoComprobantePendiente) {
        try { await comprasApi.subirArchivoComprobante(compra.id, archivoComprobantePendiente) }
        catch { mensaje = 'El recibo se registro, pero no fue posible cargar el comprobante.' }
      }
      setForm(emptyForm); setEditingId(null); setArchivoComprobantePendiente(null); setError(mensaje)
      refresh()
    },
    onError: (e) => setError(e instanceof Error ? e.message : 'No fue posible guardar.'),
  })
  const subirArchivoComprobante = useMutation({
    mutationFn: ({ id, archivo }: { id: string; archivo: File }) => comprasApi.subirArchivoComprobante(id, archivo),
    onSuccess: (compra) => { client.setQueryData<CompraRecibida[]>(['compras-recibidas'], actuales => actuales?.map(item => item.id === compra.id ? compra : item)); refresh() },
    onError: (e) => setError(e instanceof Error ? e.message : 'No fue posible cargar el comprobante.'),
  })
  const eliminarArchivoComprobante = useMutation({
    mutationFn: comprasApi.eliminarArchivoComprobante,
    onSuccess: (_data, id) => { client.setQueryData<CompraRecibida[]>(['compras-recibidas'], actuales => actuales?.map(item => item.id === id ? { ...item, nombreArchivoComprobante: null, tipoContenidoArchivoComprobante: null, fechaCargaArchivoComprobanteUtc: null } : item)); refresh() },
    onError: (e) => setError(e instanceof Error ? e.message : 'No fue posible eliminar el comprobante.'),
  })
  const verArchivoComprobante = async (id: string) => {
    try { const url = await comprasApi.obtenerArchivoComprobante(id); if (url) window.open(url, '_blank', 'noopener') }
    catch { setError('No fue posible abrir el comprobante.') }
  }
  const seleccionarArchivoComprobante = (id: string | null, archivo: File | null) => {
    if (!archivo) return
    if (archivo.size > 15 * 1024 * 1024) { setError('El comprobante no puede superar 15 MB.'); return }
    setError('')
    if (id) subirArchivoComprobante.mutate({ id, archivo })
    else setArchivoComprobantePendiente(archivo)
  }
  const enviarComprobante = useMutation({
    mutationFn: comprasApi.enviarComprobante,
    onMutate: id => setComprobantesEnviando(actuales => new Set(actuales).add(id)),
    onSettled: (_data, _error, id) => setComprobantesEnviando(actuales => { const siguientes = new Set(actuales); siguientes.delete(id); return siguientes }),    onSuccess: (resultado, id) => { client.setQueryData<CompraRecibida[]>(['compras-recibidas'], actuales => actuales?.map(compra => compra.id === id ? { ...compra, fechaComprobanteEnviadoUtc: resultado.enviadoEnUtc } : compra)); refresh(); setSuccess(`Comprobante enviado a ${resultado.receptor} con copia a ${resultado.copia}.`) },
    onError: motivo => setError(motivo instanceof Error ? motivo.message : 'No fue posible enviar el comprobante.'),
  })
  const remove = useMutation({ mutationFn: comprasApi.eliminar, onSuccess: () => { refresh(); setDeleteTarget(null) }, onError: (e) => { setError(e instanceof Error ? e.message : 'No fue posible eliminar.'); setDeleteTarget(null) } })
  const actualizarStatus = useMutation({
    mutationFn: ({ id, status }: { id: string; status: CompraStatus }) => comprasApi.actualizarStatus(id, status),
    onSuccess: (compra) => { client.setQueryData<CompraRecibida[]>(['compras-recibidas'], actuales => actuales?.map(item => item.id === compra.id ? compra : item)); refresh() },
    onError: (e) => setError(e instanceof Error ? e.message : 'No fue posible actualizar el status.'),
  })
  const field = (key: keyof CompraRecibidaRequest, value: string) => setForm(current => ({ ...current, [key]: value === '' ? null : value }))
  const submit = (event: FormEvent) => { event.preventDefault(); setError(''); save.mutate() }
  const edit = (item: CompraRecibida) => { setEditingId(item.id); setForm({ contenedorCompartidoId: item.contenedorCompartidoId, nombreContenedor: item.nombreContenedor, numeroContenedor: item.numeroContenedor, empresaId: item.empresaId, descripcion: item.descripcion, fechaSalida: item.fechaSalida, fechaLlegada: item.fechaLlegada, aduana: item.aduana, puertoLlegada: item.puertoLlegada, marcaBultoId: item.marcaBultoId, receptorCodigoUsuario: item.receptorCodigoUsuario }); setArchivoComprobantePendiente(null); window.scrollTo({ top: 0, behavior: 'smooth' }) }
  const editingItem = editingId ? compras.data?.find(item => item.id === editingId) ?? null : null
  const name = (list: Catalogo[] | undefined, id: string | null) => list?.find(item => item.id === id)?.nombre ?? '-'
  const filtered = useMemo(() => {
    const term = search.trim().toLocaleLowerCase('es-VE')

    return (compras.data ?? []).filter(item => {
      const coincideFechaLlegada = !fechaLlegadaFiltro || item.fechaLlegada === fechaLlegadaFiltro
      if (!coincideFechaLlegada) return false
      if (!term) return true

      const searchableFields = [
        item.nombreContenedor,
        item.numeroContenedor,
        name(empresas.data, item.empresaId),
        item.fechaSalida,
        date(item.fechaSalida),
        item.fechaLlegada ?? '',
        date(item.fechaLlegada),
        item.puertoLlegada,
        name(marcas.data, item.marcaBultoId),
        item.receptorNombre ?? '',
        item.status,
      ]

      return searchableFields.join(' ').toLocaleLowerCase('es-VE').includes(term)
    })
  }, [compras.data, empresas.data, marcas.data, fechaLlegadaFiltro, search])
  const comprasEnRango = useMemo(() => !rangoDesde || !rangoHasta ? [] : (compras.data ?? []).filter(item => item.fechaSalida >= rangoDesde && item.fechaSalida <= rangoHasta), [compras.data, rangoDesde, rangoHasta])
  const exportar = async (tipo: 'excel' | 'pdf', items: CompraRecibida[], nombreBase: string, titulo: string) => {
    if (items.length === 0) { setError('No hay recibos de compra para este reporte.'); return }
    try {
      setExportando(tipo); setError('')
      const imagenes = await Promise.all(items.map(cargarImagenComprobante))
      const filas = items.map((item, indice) => filaExportableCompra(item, name(empresas.data, item.empresaId), name(marcas.data, item.marcaBultoId), imagenes[indice] !== null))
      if (tipo === 'excel') {
        descargarExcelConImagen(filas, `${nombreBase}.xlsx`, columnaArchivoComprobante, imagenes, 'Recibos de compra')
      } else {
        const pdf = new jsPDF({ orientation: 'landscape', unit: 'pt', format: 'a3' })
        const encabezados = Object.keys(filas[0])
        const indiceImagen = encabezados.indexOf(columnaArchivoComprobante)
        pdf.setFontSize(16); pdf.text(titulo, 40, 34)
        autoTable(pdf, {
          startY: 48,
          head: [encabezados],
          body: filas.map(fila => Object.values(fila)),
          styles: { fontSize: 8, minCellHeight: 46, valign: 'middle' },
          columnStyles: { [indiceImagen]: { cellWidth: 50 } },
          margin: { left: 20, right: 20 },
          didDrawCell: (dato) => {
            if (dato.section !== 'body' || dato.column.index !== indiceImagen) return
            const imagen = imagenes[dato.row.index]
            if (!imagen) return
            try { pdf.addImage(imagen, 'PNG', dato.cell.x + 4, dato.cell.y + 4, dato.cell.width - 8, dato.cell.height - 8, undefined, 'FAST') }
            catch { /* Una imagen incompatible no debe impedir descargar el documento. */ }
          },
        })
        pdf.save(`${nombreBase}.pdf`)
      }
    } catch (e) { setError(e instanceof Error ? e.message : 'No fue posible generar el archivo.') } finally { setExportando(null) }
  }
  if ([compras, empresas, marcas, contenedores, aduanas, puertos, receptores].some(query => query.isLoading)) return <p className="loading">Cargando operacion...</p>
  if ([compras, empresas, marcas, contenedores, aduanas, puertos, receptores].some(query => query.isError)) return <p className="error">No fue posible conectar con el API. Verifica que el backend este iniciado.</p>
  return <section className="page-grid">{success && <div className="toast toast-success" role="status" aria-live="polite"><FontAwesomeIcon icon={faCircleCheck} /><span>{success}</span></div>}{error && <div className="toast toast-error" role="alert" aria-live="assertive"><FontAwesomeIcon icon={faTriangleExclamation} /><span>{error}</span></div>}<div className="page-heading"><div><h2>Recibos de compra</h2><p>Registra la mercancia llegada desde China hacia Venezuela.</p></div><span className="badge">{compras.data?.length ?? 0} {compras.data?.length === 1 ? 'registro' : 'registros'}</span></div>
    <form className="card form-card" onSubmit={submit}><div className="form-title"><div className="form-title-icon">+</div><h3>{editingId ? 'Editar recibo de compra' : 'Nuevo recibo de compra'}</h3>{editingId && <button type="button" className="link-button" onClick={reset}>Cancelar edicion</button>}</div>{error && <p className="error">{error}</p>}<div className="form-grid">
      <label><span>Nombre del contenedor</span><input required placeholder="Ej. Contenedor 32" value={form.nombreContenedor} onChange={e => field('nombreContenedor', e.target.value)} /></label><label><span>Numero del contenedor</span><input required placeholder="MSKU-0000000" value={form.numeroContenedor} onChange={e => field('numeroContenedor', e.target.value)} /></label>
      <label><span>Empresa</span><select required value={form.empresaId ?? ''} onChange={e => field('empresaId', e.target.value)}><option value="">Selecciona una empresa</option>{empresas.data?.map(item => <option key={item.id} value={item.id}>{item.nombre}</option>)}</select></label><label><span>Contenedor compartido</span><select value={form.contenedorCompartidoId ?? ''} onChange={e => field('contenedorCompartidoId', e.target.value)}><option value="">No aplica</option>{contenedores.data?.map(item => <option key={item.id} value={item.id}>{item.nombre}</option>)}</select></label>
      <label><span>Fecha de salida</span><input required type="date" value={form.fechaSalida} onChange={e => field('fechaSalida', e.target.value)} /></label><label><span>Fecha de llegada</span><input type="date" value={form.fechaLlegada ?? ''} onChange={e => field('fechaLlegada', e.target.value)} /></label>
      <label><span>Puerto de llegada</span><select required value={form.puertoLlegada ?? ''} onChange={e => field('puertoLlegada', e.target.value)}><option value="">Selecciona un puerto</option>{form.puertoLlegada && !puertos.data?.some(item => item.nombre === form.puertoLlegada) && <option value={form.puertoLlegada}>{form.puertoLlegada} (historico)</option>}{puertos.data?.map(item => <option key={item.id} value={item.nombre}>{item.nombre}</option>)}</select></label><label><span>Marca de bulto</span><select value={form.marcaBultoId ?? ''} onChange={e => field('marcaBultoId', e.target.value)}><option value="">No aplica</option>{marcas.data?.map(item => <option key={item.id} value={item.id}>{item.nombre}</option>)}</select></label>
      <label><span>Aduana</span><select value={form.aduana ?? ''} onChange={e => field('aduana', e.target.value)}><option value="">No aplica</option>{form.aduana && !aduanas.data?.some(item => item.nombre === form.aduana) && <option value={form.aduana}>{form.aduana} (historico)</option>}{aduanas.data?.map(item => <option key={item.id} value={item.nombre}>{item.nombre}</option>)}</select></label><label><span>Receptor</span><select required value={form.receptorCodigoUsuario ?? ''} onChange={e => field('receptorCodigoUsuario', e.target.value)}><option value="">Selecciona un receptor</option>{receptores.data?.map(usuario => <option key={usuario.codigoUsuario} value={usuario.codigoUsuario}>{usuario.nombre}</option>)}</select></label><label><span>Cargar comprobante</span>{editingItem?.nombreArchivoComprobante ? <div className="comprobante-actual"><span title={editingItem.nombreArchivoComprobante}>{editingItem.nombreArchivoComprobante}</span><button type="button" className="link-button" onClick={() => verArchivoComprobante(editingItem.id)}>Ver</button><button type="button" className="danger-button" disabled={eliminarArchivoComprobante.isPending} onClick={() => eliminarArchivoComprobante.mutate(editingItem.id)}>Quitar</button></div> : <div className="comprobante-upload"><input type="file" accept="application/pdf,image/jpeg,image/png,image/webp" onChange={e => { seleccionarArchivoComprobante(editingId, e.target.files?.[0] ?? null); e.target.value = '' }} /><span>{archivoComprobantePendiente ? archivoComprobantePendiente.name : 'PDF, JPEG, PNG o WebP · Maximo 15 MB'}</span></div>}</label><label className="description-field"><span>Descripcion</span><textarea placeholder="Detalle de la mercancia" value={form.descripcion ?? ''} onChange={e => field('descripcion', e.target.value)} /></label></div><button className="primary" disabled={save.isPending}>{save.isPending ? 'Guardando...' : editingId ? 'Guardar cambios' : 'Registrar recibo de compra'}</button></form>
    <div className="card table-card"><div className="table-toolbar"><h3>Historial de recibos de compra</h3><div><label className="search"><FontAwesomeIcon icon={faMagnifyingGlass} /><input aria-label="Buscar en todos los campos" placeholder="Buscar en todos los campos" value={search} onChange={e => setSearch(e.target.value)} /></label><label className="arrival-date-filter"><span>Fecha llegada</span><input aria-label="Filtrar por fecha de llegada" type="date" value={fechaLlegadaFiltro} onChange={e => setFechaLlegadaFiltro(e.target.value)} /></label>{fechaLlegadaFiltro && <button type="button" className="clear-filter" onClick={() => setFechaLlegadaFiltro("")}>Limpiar</button>}<button className="refresh" onClick={() => refresh()}>Actualizar</button><button type="button" className="export-button export-excel" disabled={exportando !== null} title="Descargar reporte general en Excel" aria-label="Descargar reporte general en Excel" onClick={() => exportar('excel', compras.data ?? [], 'recibos-de-compra', 'Recibos de compra')}><FontAwesomeIcon icon={faFileExcel} /></button><button type="button" className="export-button export-pdf" disabled={exportando !== null} title="Descargar reporte general en PDF" aria-label="Descargar reporte general en PDF" onClick={() => exportar('pdf', compras.data ?? [], 'recibos-de-compra', 'Recibos de compra')}><FontAwesomeIcon icon={faFilePdf} /></button><button type="button" className="link-button" onClick={() => setMostrarRango(x => !x)}>{mostrarRango ? 'Ocultar rango' : 'Reporte por rango'}</button></div></div>{mostrarRango && <div className="date-range-export"><label><span>Salida desde</span><input type="date" value={rangoDesde} onChange={e => setRangoDesde(e.target.value)} /></label><label><span>Salida hasta</span><input type="date" min={rangoDesde || undefined} value={rangoHasta} onChange={e => setRangoHasta(e.target.value)} /></label><button type="button" className="export-button export-excel" disabled={exportando !== null || comprasEnRango.length === 0} title="Descargar rango en Excel" aria-label="Descargar rango en Excel" onClick={() => exportar('excel', comprasEnRango, `recibos-${rangoDesde}-a-${rangoHasta}`, `Recibos de compra: ${date(rangoDesde)} a ${date(rangoHasta)}`)}><FontAwesomeIcon icon={faFileExcel} /></button><button type="button" className="export-button export-pdf" disabled={exportando !== null || comprasEnRango.length === 0} title="Descargar rango en PDF" aria-label="Descargar rango en PDF" onClick={() => exportar('pdf', comprasEnRango, `recibos-${rangoDesde}-a-${rangoHasta}`, `Recibos de compra: ${date(rangoDesde)} a ${date(rangoHasta)}`)}><FontAwesomeIcon icon={faFilePdf} /></button><span className="date-range-count">{rangoDesde && rangoHasta ? `${comprasEnRango.length} ${comprasEnRango.length === 1 ? 'recibo' : 'recibos'} en el rango` : 'Selecciona ambas fechas'}</span></div>}<div className="table-wrap"><table><thead><tr><th>Contenedor</th><th>Empresa</th><th>Salida</th><th>Llegada</th><th>Puerto</th><th>Marca</th><th>Receptor</th><th>Archivo</th><th>Status</th><th>Comprobante</th><th>Acciones</th></tr></thead><tbody>{filtered.length === 0 ? <tr><td colSpan={11} className="empty">Aun no hay recibos de compra.</td></tr> : filtered.map(item => <tr key={item.id}><td><strong>{item.nombreContenedor}</strong><small>{item.numeroContenedor}</small></td><td>{name(empresas.data, item.empresaId)}</td><td>{date(item.fechaSalida)}</td><td>{date(item.fechaLlegada)}</td><td>{item.puertoLlegada}</td><td><span className="tag">{name(marcas.data, item.marcaBultoId)}</span></td><td>{item.receptorNombre ?? "Sin asignar"}</td><td><ArchivoComprobanteCelda item={item} ver={verArchivoComprobante} /></td><td><select className="status-select" value={item.status} disabled={actualizarStatus.isPending} onChange={event => { setError(""); actualizarStatus.mutate({ id: item.id, status: event.target.value as CompraStatus }) }}>{estadosCompra.map(status => <option key={status} value={status}>{status}</option>)}</select></td><td><button type="button" className="link-button" disabled={comprobantesEnviando.has(item.id) || item.fechaComprobanteEnviadoUtc !== null} title={item.fechaComprobanteEnviadoUtc ? "El comprobante ya fue enviado" : undefined} onClick={() => { setError(""); setSuccess(""); enviarComprobante.mutate(item.id) }}>{item.fechaComprobanteEnviadoUtc ? "Enviado" : comprobantesEnviando.has(item.id) ? "Enviando..." : "Enviar"}</button></td><td className="actions"><button className="export-button export-excel export-inline" type="button" disabled={exportando !== null} title="Descargar este recibo en Excel" aria-label="Descargar este recibo en Excel" onClick={() => exportar('excel', [item], `recibo-${item.numeroContenedor}`, `Recibo de compra: ${item.numeroContenedor}`)}><FontAwesomeIcon icon={faFileExcel} /></button><button className="export-button export-pdf export-inline" type="button" disabled={exportando !== null} title="Descargar este recibo en PDF" aria-label="Descargar este recibo en PDF" onClick={() => exportar('pdf', [item], `recibo-${item.numeroContenedor}`, `Recibo de compra: ${item.numeroContenedor}`)}><FontAwesomeIcon icon={faFilePdf} /></button><button className="link-button" disabled={item.fechaComprobanteEnviadoUtc !== null} title={item.fechaComprobanteEnviadoUtc ? "Una compra con comprobante enviado no puede editarse" : undefined} onClick={() => edit(item)}>Editar</button><button className="danger-button" disabled={item.fechaComprobanteEnviadoUtc !== null} title={item.fechaComprobanteEnviadoUtc ? "Una compra con comprobante enviado no puede eliminarse" : undefined} onClick={() => setDeleteTarget(item)}>Eliminar</button></td></tr>)}</tbody></table></div></div>
    <ConfirmDeleteDialog open={deleteTarget !== null} itemName={deleteTarget?.numeroContenedor ?? ''} onCancel={() => setDeleteTarget(null)} onConfirm={() => deleteTarget && remove.mutate(deleteTarget.id)} />
  </section>
}

function ArchivoComprobanteCelda({ item, ver }: { item: CompraRecibida; ver: (id: string) => void }) {
  if (!item.nombreArchivoComprobante) return <span className="image-placeholder">Sin<br />archivo</span>
  if (item.tipoContenidoArchivoComprobante?.startsWith('image/')) return <ArchivoComprobanteMiniatura id={item.id} nombre={item.nombreArchivoComprobante} ver={ver} />
  return <button type="button" className="link-button" onClick={() => ver(item.id)}>Ver PDF</button>
}

function ArchivoComprobanteMiniatura({ id, nombre, ver }: { id: string; nombre: string; ver: (id: string) => void }) {
  const [url, setUrl] = useState<string | null>(null)
  useEffect(() => {
    let activa = true; let creada: string | null = null
    void comprasApi.obtenerArchivoComprobante(id).then(valor => { creada = valor; if (activa) setUrl(valor) }).catch(() => { if (activa) setUrl(null) })
    return () => { activa = false; if (creada) URL.revokeObjectURL(creada) }
  }, [id])
  return <button type="button" className="comprobante-thumbnail-button" title={nombre} onClick={() => ver(id)}><span className="product-thumbnail-frame">{url ? <img className="product-thumbnail" src={url} alt="Comprobante" /> : <span className="image-placeholder">Cargando</span>}</span></button>
}











