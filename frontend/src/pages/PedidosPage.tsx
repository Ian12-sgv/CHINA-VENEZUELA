import { useEffect, useState, type FormEvent } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faChevronDown, faCloudArrowUp, faFileExcel, faTrash } from '@fortawesome/free-solid-svg-icons'
import { faFilePdf } from '@fortawesome/free-regular-svg-icons'
import { jsPDF } from 'jspdf'
import autoTable from 'jspdf-autotable'
import { catalogosApi, pedidosApi, receptoresApi } from '../api'
import { ConfirmDeleteDialog } from '../components/ConfirmDeleteDialog'
import type { CrearProductoPedidoRequest, PaginaProductosPedido, PedidoResumen, ProductoPedido, TipoImagenProductoPedido } from '../types'

const crearFormularioInicial = (): CrearProductoPedidoRequest => ({ pedidoId: null, nombreNuevoGrupo: null, codigoBarraAsignado: '', precioRmb: null, totalRmb: null, cantidadDoz: null, referenciaAsignada: '', tipoProducto: null, agente: null, fabrica: null, composicionTela: null, colorParaFabricar: null, marcaProducto: null, curvaTalla: null, packPorCaja: null, cantidadUnidades: null, marcaBulto: null, cantidadBulto: null })
type FiltroEnvio = 'todos' | 'enviados' | 'pendientes'
type ModoGrupo = '' | 'existente' | 'nuevo'
const formatoYuan = (valor: number | null) => valor === null ? '-' : `¥ ${valor.toLocaleString('es-VE', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`

export function PedidosPage() {
  const client = useQueryClient()
  const [form, setForm] = useState<CrearProductoPedidoRequest>(crearFormularioInicial)
  const [modoGrupo, setModoGrupo] = useState<ModoGrupo>('')
  const [busqueda, setBusqueda] = useState('')
  const [filtroEnvio, setFiltroEnvio] = useState<FiltroEnvio>('todos')
  const [grupoFiltro, setGrupoFiltro] = useState('')
  const [pagina, setPagina] = useState(1)
  const [error, setError] = useState('')
  const [exportando, setExportando] = useState<'excel' | 'pdf' | null>(null)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [productoEditando, setProductoEditando] = useState<ProductoPedido | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<ProductoPedido | null>(null)
  const [productoEnviar, setProductoEnviar] = useState<ProductoPedido | null>(null)
  const [receptorCodigo, setReceptorCodigo] = useState('')
  const [tipoImagenEliminar, setTipoImagenEliminar] = useState<TipoImagenProductoPedido | null>(null)
  const [imagenFabricaArchivo, setImagenFabricaArchivo] = useState<File | null>(null)
  const [imagenTerminadaArchivo, setImagenTerminadaArchivo] = useState<File | null>(null)
  const enviado = filtroEnvio === 'todos' ? undefined : filtroEnvio === 'enviados'

  const productos = useQuery({ queryKey: ['pedidos-productos', pagina, busqueda, filtroEnvio, grupoFiltro], queryFn: () => pedidosApi.productos(pagina, busqueda, enviado, 10, grupoFiltro) })
  const grupos = useQuery({ queryKey: ['pedidos-grupos'], queryFn: pedidosApi.grupos })
  const agentes = useQuery({ queryKey: ['pedidos-agentes'], queryFn: pedidosApi.agentes })
  const receptores = useQuery({ queryKey: ['receptores'], queryFn: receptoresApi.listar })
  const marcasBulto = useQuery({ queryKey: ['catalogo', 'marcas-bulto'], queryFn: () => catalogosApi.listar('marcas-bulto') })
  const actualizarProductoEnCache = (producto: ProductoPedido) => {
    const actualizarPagina = (datos: PaginaProductosPedido | undefined) => datos ? { ...datos, items: datos.items.map(item => item.id === producto.id ? { ...item, ...producto } : item) } : datos
    client.setQueriesData<PaginaProductosPedido>({ queryKey: ['pedidos-productos'] }, actualizarPagina)
    client.setQueriesData<PaginaProductosPedido>({ queryKey: ['pedidos-grupo-detalle'] }, actualizarPagina)
  }
  const refrescarPedidos = async () => {
    await Promise.all([
      client.refetchQueries({ queryKey: ['pedidos-productos'], type: 'active' }),
      client.refetchQueries({ queryKey: ['pedidos-grupo-detalle'], type: 'active' }),
      client.refetchQueries({ queryKey: ['pedidos-grupos'], type: 'active' }),
    ])
  }

  const guardar = useMutation({
    mutationFn: async () => {
      const producto = editingId ? await pedidosApi.actualizarProducto(editingId, form) : await pedidosApi.crearProducto(form)
      if (imagenFabricaArchivo) await pedidosApi.subirImagen(producto.id, 'fabrica', imagenFabricaArchivo)
      if (imagenTerminadaArchivo) await pedidosApi.subirImagen(producto.id, 'producto-terminado', imagenTerminadaArchivo)
      return producto
    },
    onSuccess: async (producto) => { actualizarProductoEnCache(producto); setForm(crearFormularioInicial()); setModoGrupo(''); setImagenFabricaArchivo(null); setImagenTerminadaArchivo(null); setEditingId(null); setProductoEditando(null); setError(''); setPagina(1); await refrescarPedidos() },
    onError: (e) => setError(e instanceof Error ? e.message : 'No fue posible guardar.'),
  })
  const eliminar = useMutation({ mutationFn: pedidosApi.eliminarProducto, onSuccess: async () => { setDeleteTarget(null); await refrescarPedidos() }, onError: (e) => { setError(e instanceof Error ? e.message : 'No fue posible eliminar.'); setDeleteTarget(null) } })
  const actualizarImagen = useMutation({ mutationFn: ({ tipo, archivo }: { tipo: TipoImagenProductoPedido; archivo: File }) => editingId ? pedidosApi.subirImagen(editingId, tipo, archivo) : Promise.reject(new Error('Selecciona un producto.')), onSuccess: async () => { setError(''); await refrescarPedidos() }, onError: (e) => setError(e instanceof Error ? e.message : 'No fue posible actualizar la imagen.') })
  const eliminarImagen = useMutation({ mutationFn: () => editingId && tipoImagenEliminar ? pedidosApi.eliminarImagen(editingId, tipoImagenEliminar) : Promise.reject(new Error('Selecciona una imagen.')), onSuccess: () => { if (tipoImagenEliminar === 'fabrica') setProductoEditando((x) => x ? { ...x, tieneImagenFabrica: false } : x); if (tipoImagenEliminar === 'producto-terminado') setProductoEditando((x) => x ? { ...x, tieneImagenProductoTerminado: false } : x); setTipoImagenEliminar(null); setError(''); refrescarPedidos() }, onError: (e) => { setTipoImagenEliminar(null); setError(e instanceof Error ? e.message : 'No fue posible eliminar la imagen.') } })
  const enviar = useMutation({ mutationFn: () => productoEnviar && receptorCodigo ? pedidosApi.enviarProducto(productoEnviar.id, receptorCodigo) : Promise.reject(new Error('Selecciona un receptor.')), onSuccess: async () => { setProductoEnviar(null); setReceptorCodigo(''); setError(''); await refrescarPedidos() }, onError: (e) => setError(e instanceof Error ? e.message : 'No fue posible enviar.') })

  const field = (key: keyof CrearProductoPedidoRequest, value: string) => setForm((x) => ({ ...x, [key]: value || null }))
  const numericField = (key: 'packPorCaja' | 'cantidadUnidades' | 'cantidadDoz' | 'precioRmb' | 'totalRmb' | 'cantidadBulto', value: string) => setForm((x) => ({ ...x, [key]: value === '' ? null : Number(value) }))
  const seleccionarModoGrupo = (modo: ModoGrupo) => { setModoGrupo(modo); setForm((x) => ({ ...x, pedidoId: null, nombreNuevoGrupo: null })) }
  const cancelar = () => { setEditingId(null); setProductoEditando(null); setImagenFabricaArchivo(null); setImagenTerminadaArchivo(null); setModoGrupo(''); setForm(crearFormularioInicial()) }
  const editar = (producto: ProductoPedido) => {
    if (producto.enviado) return
    setEditingId(producto.id); setProductoEditando(producto); setImagenFabricaArchivo(null); setImagenTerminadaArchivo(null); setModoGrupo(producto.pedidoId ? 'existente' : '')
    setForm({ pedidoId: producto.pedidoId, nombreNuevoGrupo: null, codigoBarraAsignado: producto.codigoBarraAsignado, precioRmb: producto.precioRmb, totalRmb: producto.totalRmb, cantidadDoz: producto.cantidadDoz, referenciaAsignada: producto.referenciaAsignada, tipoProducto: producto.tipoProducto, agente: producto.agente, fabrica: producto.fabrica, composicionTela: producto.composicionTela, colorParaFabricar: producto.colorParaFabricar, marcaProducto: producto.marcaProducto, curvaTalla: producto.curvaTalla, packPorCaja: producto.packPorCaja, cantidadUnidades: producto.cantidadUnidades, marcaBulto: producto.marcaBulto, cantidadBulto: producto.cantidadBulto })
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
  const exportar = async (tipo: 'excel' | 'pdf', producto?: ProductoPedido, pedido?: PedidoResumen) => {
    try {
      setExportando(tipo); setError('')
      const pedidoId = pedido?.id ?? grupoFiltro
      const resultado = producto ? null : await pedidosApi.productos(1, busqueda, enviado, 5000, pedidoId)
      const items = producto ? [producto] : resultado?.items ?? []
      const imagenes = await Promise.all(items.map(cargarImagenesParaExportacion))
      const filas = items.map((item, indice) => filaExportable(item, imagenes[indice]))
      const nombreBase = producto
        ? `subpedido-${producto.referenciaAsignada}`
        : pedido ? `pedido-${pedido.nombre}` : grupoFiltro ? 'grupo-de-pedidos' : 'catalogo-productos'
      const titulo = producto
        ? `Subpedido: ${producto.referenciaAsignada}`
        : pedido ? `Pedido: ${pedido.nombre}` : grupoFiltro ? 'Pedidos agrupados' : 'Catálogo de productos'
      if (tipo === 'excel') {
        descargarExcelCompatible(filas, `${nombreBase}.xlsx`, imagenes)
      } else {
        const pdf = new jsPDF({ orientation: 'landscape', unit: 'pt', format: 'a3' })
        const encabezados = Object.keys(filas[0] ?? filaExportableVacia)
        const indiceImagenFabrica = 3
        const indiceImagenTerminada = 13
        pdf.setFontSize(16); pdf.text(titulo, 40, 34)
        autoTable(pdf, {
          startY: 48,
          head: [encabezados],
          body: filas.map((fila) => Object.values(fila).map(String)),
          styles: { fontSize: 6, minCellHeight: 58, valign: 'middle' },
          columnStyles: { [indiceImagenFabrica]: { cellWidth: 62 }, [indiceImagenTerminada]: { cellWidth: 62 } },
          margin: { left: 20, right: 20 },
          didDrawCell: (dato) => {
            if (dato.section !== 'body' || (dato.column.index !== indiceImagenFabrica && dato.column.index !== indiceImagenTerminada)) return
            const imagen = dato.column.index === indiceImagenFabrica ? imagenes[dato.row.index]?.fabrica : imagenes[dato.row.index]?.productoTerminado
            if (!imagen) return
            try {
              const formato = imagen.startsWith('data:image/png') ? 'PNG' : imagen.startsWith('data:image/webp') ? 'WEBP' : 'JPEG'
              pdf.addImage(imagen, formato, dato.cell.x + 4, dato.cell.y + 4, dato.cell.width - 8, dato.cell.height - 8, undefined, 'FAST')
            } catch { /* Una imagen incompatible no debe impedir descargar el documento. */ }
          },
        })
        pdf.save(`${nombreBase}.pdf`)
      }
    } catch (e) { setError(e instanceof Error ? e.message : 'No fue posible generar el archivo.') } finally { setExportando(null) }
  }

  return <section className="page-grid">
    <div className="page-heading"><div><span className="eyebrow">Operación</span><h2>Pedidos</h2><p>Catálogo de productos para pedidos agrupados.</p></div></div>
    <article className="card form-card">
      <div className="form-title"><div><h3>{editingId ? 'Editar producto' : 'Nuevo producto'}</h3><p>Registra los datos asignados al producto.</p></div>{editingId && <button className="link-button" onClick={cancelar}>Cancelar</button>}</div>
      {error && <p className="error">{error}</p>}
      <form className="form-grid" onSubmit={(e: FormEvent) => { e.preventDefault(); guardar.mutate() }}>
        <label><span>Grupo de pedido</span><select required value={modoGrupo} onChange={(e) => seleccionarModoGrupo(e.target.value as ModoGrupo)}><option value="">Selecciona una opción</option><option value="existente">Agregar a un grupo existente</option><option value="nuevo">Crear un grupo nuevo</option></select></label>
        {modoGrupo === 'existente' && <label><span>Grupo existente</span><select required value={form.pedidoId ?? ''} onChange={(e) => setForm((x) => ({ ...x, pedidoId: e.target.value || null }))}><option value="">Selecciona un grupo</option>{grupos.data?.map((grupo) => <option key={grupo.id} value={grupo.id}>{grupo.nombre} ({grupo.cantidadPedidos})</option>)}</select></label>}
        {modoGrupo === 'nuevo' && <label><span>Nombre del grupo nuevo</span><input required value={form.nombreNuevoGrupo ?? ''} onChange={(e) => field('nombreNuevoGrupo', e.target.value)} placeholder="Ej. Pedido octubre 2026" /></label>}
        <label><span>Agente</span><select value={form.agente ?? ''} onChange={(e) => field('agente', e.target.value)}><option value="">Sin asignar</option>{agentes.data?.map((agente) => <option key={agente.id} value={agente.nombre}>{agente.nombre}</option>)}</select></label>
        <label><span>Tipo de pedido</span><select required value={form.tipoProducto ?? ''} onChange={(e) => field('tipoProducto', e.target.value)}><option value="">Selecciona un tipo</option><option value="Nuevo">Nuevo</option><option value="Repetido">Repetido</option></select></label>
        <CampoImagenProducto etiqueta="Imagen de fábricas" tipo="fabrica" productoId={editingId} tieneImagen={productoEditando?.tieneImagenFabrica ?? false} archivo={imagenFabricaArchivo} ocupada={actualizarImagen.isPending || eliminarImagen.isPending} seleccionar={(archivo) => { setImagenFabricaArchivo(archivo); setError('') }} actualizar={(archivo) => actualizarImagen.mutate({ tipo: 'fabrica', archivo })} eliminar={() => setTipoImagenEliminar('fabrica')} error={setError} />
        <label><span>Fábrica</span><input value={form.fabrica ?? ''} onChange={(e) => field('fabrica', e.target.value)} /></label>
        <label><span>Composición tela</span><input value={form.composicionTela ?? ''} onChange={(e) => field('composicionTela', e.target.value)} /></label>
        <label><span>Color para fabricar</span><input value={form.colorParaFabricar ?? ''} onChange={(e) => field('colorParaFabricar', e.target.value)} /></label>
        <label><span>Marca del producto</span><input value={form.marcaProducto ?? ''} onChange={(e) => field('marcaProducto', e.target.value)} /></label>
        <label><span>Curva talla</span><input value={form.curvaTalla ?? ''} onChange={(e) => field('curvaTalla', e.target.value)} /></label>
        <label><span>Pack por cajas</span><input type="number" min="1" step="1" value={form.packPorCaja ?? ''} onChange={(e) => numericField('packPorCaja', e.target.value)} /></label>
        <label><span>Cantidad de unidades</span><input type="number" min="1" step="1" value={form.cantidadUnidades ?? ''} onChange={(e) => numericField('cantidadUnidades', e.target.value)} /></label>
        <label><span>Marca del bulto</span><select value={form.marcaBulto ?? ''} onChange={(e) => setForm((current) => ({ ...current, marcaBulto: e.target.value || null, cantidadBulto: e.target.value ? current.cantidadBulto : null }))}><option value="">No aplica</option>{marcasBulto.data?.map((marca) => <option key={marca.id} value={marca.nombre}>{marca.nombre}</option>)}</select></label><label><span>Cantidad de bulto</span><input type="number" min="1" step="1" disabled={!form.marcaBulto} value={form.cantidadBulto ?? ''} onChange={(e) => numericField('cantidadBulto', e.target.value)} /></label>
        <CampoImagenProducto etiqueta="Imagen del producto terminado" tipo="producto-terminado" productoId={editingId} tieneImagen={productoEditando?.tieneImagenProductoTerminado ?? false} archivo={imagenTerminadaArchivo} ocupada={actualizarImagen.isPending || eliminarImagen.isPending} seleccionar={(archivo) => { setImagenTerminadaArchivo(archivo); setError('') }} actualizar={(archivo) => actualizarImagen.mutate({ tipo: 'producto-terminado', archivo })} eliminar={() => setTipoImagenEliminar('producto-terminado')} error={setError} />
        <label><span>Referencia asignada</span><input required value={form.referenciaAsignada} onChange={(e) => field('referenciaAsignada', e.target.value)} /></label>
        <label><span>Código barra asignado</span><input required value={form.codigoBarraAsignado} onChange={(e) => field('codigoBarraAsignado', e.target.value)} /></label>
        <label className="currency-field"><span>Precio (RMB)</span><div className="currency-input"><span aria-hidden="true">¥</span><input type="number" min="0" step="0.01" inputMode="decimal" placeholder="0.00" value={form.precioRmb ?? ''} onChange={(e) => numericField('precioRmb', e.target.value)} /></div><small>Yuan chino · CNY</small></label>
        <label className="currency-field"><span>Total (RMB)</span><div className="currency-input"><span aria-hidden="true">¥</span><input type="number" min="0" step="0.01" inputMode="decimal" placeholder="0.00" value={form.totalRmb ?? ''} onChange={(e) => numericField('totalRmb', e.target.value)} /></div><small>Yuan chino · CNY</small></label>
        <label><span>Cantidad DOZ</span><input type="number" min="1" step="1" value={form.cantidadDoz ?? ''} onChange={(e) => numericField('cantidadDoz', e.target.value)} /></label>
        <div className="description-field"><button className="primary" disabled={guardar.isPending}>{guardar.isPending ? 'Guardando...' : editingId ? 'Guardar cambios' : 'Registrar producto'}</button></div>
      </form>
    </article>
    <Tabla pagina={productos.data} grupos={grupos.data ?? []} grupoFiltro={grupoFiltro} busqueda={busqueda} filtroEnvio={filtroEnvio} cargar={productos.isLoading} exportando={exportando} buscar={(value) => { setBusqueda(value); setPagina(1) }} filtrarGrupo={(value) => { setGrupoFiltro(value); setPagina(1) }} filtrarEnvio={(value) => { setFiltroEnvio(value); setPagina(1) }} irPagina={setPagina} editar={editar} eliminar={setDeleteTarget} enviar={setProductoEnviar} exportar={exportar} />
    {productoEnviar && <article className="card form-card"><div className="form-title"><div><h3>Enviar producto</h3><p>{productoEnviar.referenciaAsignada}</p></div><button className="link-button" onClick={() => setProductoEnviar(null)}>Cancelar</button></div><label><span>Receptor</span><select value={receptorCodigo} onChange={(e) => setReceptorCodigo(e.target.value)}><option value="">Selecciona un usuario</option>{receptores.data?.map((x) => <option key={x.codigoUsuario} value={x.codigoUsuario}>{x.nombre}</option>)}</select></label><button className="primary" disabled={enviar.isPending || !receptorCodigo} onClick={() => enviar.mutate()}>{enviar.isPending ? 'Enviando...' : 'Enviar por correo'}</button></article>}
    <ConfirmDeleteDialog open={tipoImagenEliminar !== null} itemName={tipoImagenEliminar === 'fabrica' ? 'la imagen de fábricas' : 'la imagen del producto terminado'} onCancel={() => setTipoImagenEliminar(null)} onConfirm={() => eliminarImagen.mutate()} />
    <ConfirmDeleteDialog open={deleteTarget !== null} itemName={deleteTarget?.referenciaAsignada ?? ''} onCancel={() => setDeleteTarget(null)} onConfirm={() => deleteTarget && eliminar.mutate(deleteTarget.id)} />
  </section>
}

function Tabla({ pagina, grupos, grupoFiltro, busqueda, filtroEnvio, cargar, exportando, buscar, filtrarGrupo, filtrarEnvio, irPagina, editar, eliminar, enviar, exportar }: { pagina: PaginaProductosPedido | undefined; grupos: PedidoResumen[]; grupoFiltro: string; busqueda: string; filtroEnvio: FiltroEnvio; cargar: boolean; exportando: 'excel' | 'pdf' | null; buscar: (x: string) => void; filtrarGrupo: (x: string) => void; filtrarEnvio: (x: FiltroEnvio) => void; irPagina: (x: number) => void; editar: (x: ProductoPedido) => void; eliminar: (x: ProductoPedido) => void; enviar: (x: ProductoPedido) => void; exportar: (tipo: 'excel' | 'pdf', producto?: ProductoPedido, pedido?: PedidoResumen) => void }) {
  const [grupoExpandido, setGrupoExpandido] = useState<string | null>(grupoFiltro || null)
  useEffect(() => { if (grupoFiltro) setGrupoExpandido(grupoFiltro) }, [grupoFiltro])
  const itemsSinGrupo = (pagina?.items ?? []).filter((producto) => !producto.pedidoId)
  const gruposVisibles = grupoFiltro ? grupos.filter((grupo) => grupo.id === grupoFiltro) : grupos
  const grupoActual = grupos.find((grupo) => grupo.id === grupoFiltro)
  const alternarGrupo = (id: string) => setGrupoExpandido((actual) => actual === id ? null : id)

  return <article className="card table-card">
    <div className="table-toolbar"><div><h3>Catálogo de productos</h3><p>{grupoActual ? `${grupoActual.nombre} · ${grupoActual.cantidadPedidos} subpedidos` : `${pagina?.total ?? 0} artículos · ${pagina?.totalPaginas ?? 0} páginas`}</p></div><div><select aria-label="Filtrar por grupo de pedido" value={grupoFiltro} onChange={(e) => filtrarGrupo(e.target.value)}><option value="">Todos los pedidos agrupados</option>{grupos.map((grupo) => <option key={grupo.id} value={grupo.id}>{grupo.nombre} ({grupo.cantidadPedidos})</option>)}</select><select aria-label="Filtrar por envío" value={filtroEnvio} onChange={(e) => filtrarEnvio(e.target.value as FiltroEnvio)}><option value="todos">Todos</option><option value="enviados">Enviados</option><option value="pendientes">Pendientes de envío</option></select><input placeholder="Buscar en todos los campos" value={busqueda} onChange={(e) => buscar(e.target.value)} /></div></div>
    <div className="table-wrap"><table className="productos-table"><thead><tr><th>Grupo</th><th>Agente</th><th>Tipo de pedido</th><th>Imagen de fábricas</th><th>Fábrica</th><th>Composición tela</th><th>Color para fabricar</th><th>Marca del producto</th><th>Curva talla</th><th>Pack por cajas</th><th>Cantidad de unidades</th><th>Marca del bulto</th><th>Cantidad de bulto</th><th>Imagen producto terminado</th><th>Referencia asignada</th><th>Código barra asignado</th><th>Precio (RMB)</th><th>Total (RMB)</th><th>Cantidad DOZ</th><th>Estado</th><th>Acciones</th></tr></thead><tbody>
      {cargar ? <tr><td colSpan={21} className="empty">Cargando...</td></tr> : <>
        {gruposVisibles.map((grupo) => <GrupoAcordeon key={grupo.id} grupo={grupo} expandido={grupoExpandido === grupo.id} busqueda={busqueda} filtroEnvio={filtroEnvio} exportando={exportando} alternar={() => alternarGrupo(grupo.id)} editar={editar} eliminar={eliminar} enviar={enviar} exportar={exportar} />)}
        {itemsSinGrupo.map((producto) => <FilaProducto key={producto.id} producto={producto} etiquetaGrupo="Sin grupo" exportando={exportando} editar={editar} eliminar={eliminar} enviar={enviar} exportar={exportar} />)}
        {gruposVisibles.length === 0 && itemsSinGrupo.length === 0 && <tr><td colSpan={21} className="empty">No hay productos para mostrar.</td></tr>}
      </>}
    </tbody></table></div>
    {grupoFiltro === '' && (pagina?.totalPaginas ?? 0) > 1 && <div className="table-toolbar"><button className="secondary" disabled={pagina!.pagina <= 1} onClick={() => irPagina(pagina!.pagina - 1)}>Anterior</button><span>Página {pagina!.pagina} de {pagina!.totalPaginas}</span><button className="secondary" disabled={pagina!.pagina >= pagina!.totalPaginas} onClick={() => irPagina(pagina!.pagina + 1)}>Siguiente</button></div>}
  </article>
}

function GrupoAcordeon({ grupo, expandido, busqueda, filtroEnvio, exportando, alternar, editar, eliminar, enviar, exportar }: { grupo: PedidoResumen; expandido: boolean; busqueda: string; filtroEnvio: FiltroEnvio; exportando: 'excel' | 'pdf' | null; alternar: () => void; editar: (x: ProductoPedido) => void; eliminar: (x: ProductoPedido) => void; enviar: (x: ProductoPedido) => void; exportar: (tipo: 'excel' | 'pdf', producto?: ProductoPedido, pedido?: PedidoResumen) => void }) {
  const enviado = filtroEnvio === 'todos' ? undefined : filtroEnvio === 'enviados'
  const detalle = useQuery({ queryKey: ['pedidos-grupo-detalle', grupo.id, busqueda, filtroEnvio], queryFn: () => pedidosApi.productos(1, busqueda, enviado, 5000, grupo.id), enabled: expandido })
  return <>
    <tr className="group-summary-row"><td colSpan={20} className="group-summary-cell"><button type="button" className={`group-accordion${expandido ? ' expanded' : ''}`} onClick={alternar} aria-expanded={expandido}><FontAwesomeIcon icon={faChevronDown} /><span>{grupo.nombre}</span><small>{grupo.cantidadPedidos} subpedidos</small></button></td><td className="actions group-export-actions"><button className="export-button export-excel export-inline" type="button" disabled={exportando !== null} onClick={() => exportar('excel', undefined, grupo)} title="Descargar el pedido completo en Excel" aria-label="Descargar el pedido completo en Excel"><FontAwesomeIcon icon={faFileExcel} /></button><button className="export-button export-pdf export-inline" type="button" disabled={exportando !== null} onClick={() => exportar('pdf', undefined, grupo)} title="Descargar el pedido completo en PDF" aria-label="Descargar el pedido completo en PDF"><FontAwesomeIcon icon={faFilePdf} /></button></td></tr>
    {expandido && (detalle.isLoading ? <tr><td colSpan={21} className="empty">Cargando subpedidos...</td></tr> : (detalle.data?.items ?? []).length === 0 ? <tr><td colSpan={21} className="empty">No hay subpedidos que coincidan con los filtros.</td></tr> : detalle.data!.items.map((producto) => <FilaProducto key={producto.id} producto={producto} etiquetaGrupo="↳ Subpedido" exportando={exportando} editar={editar} eliminar={eliminar} enviar={enviar} exportar={exportar} />))}
  </>
}

function FilaProducto({ producto, etiquetaGrupo, exportando, editar, eliminar, enviar, exportar }: { producto: ProductoPedido; etiquetaGrupo: string; exportando: 'excel' | 'pdf' | null; editar: (x: ProductoPedido) => void; eliminar: (x: ProductoPedido) => void; enviar: (x: ProductoPedido) => void; exportar: (tipo: 'excel' | 'pdf', producto?: ProductoPedido, pedido?: PedidoResumen) => void }) {
  return <tr className="subpedido-row"><td>{etiquetaGrupo}</td><td>{producto.agente ?? '-'}</td><td>{producto.tipoProducto ?? '-'}</td><td><ImagenProducto id={producto.id} tipo="fabrica" tieneImagen={producto.tieneImagenFabrica} /></td><td>{producto.fabrica ?? '-'}</td><td>{producto.composicionTela ?? '-'}</td><td>{producto.colorParaFabricar ?? '-'}</td><td>{producto.marcaProducto ?? '-'}</td><td>{producto.curvaTalla ?? '-'}</td><td>{producto.packPorCaja ?? '-'}</td><td>{producto.cantidadUnidades ?? '-'}</td><td>{producto.marcaBulto ?? '-'}</td><td>{producto.cantidadBulto ?? '-'}</td><td><ImagenProducto id={producto.id} tipo="producto-terminado" tieneImagen={producto.tieneImagenProductoTerminado} /></td><td><strong>{producto.referenciaAsignada}</strong></td><td>{producto.codigoBarraAsignado}</td><td>{formatoYuan(producto.precioRmb)}</td><td>{formatoYuan(producto.totalRmb)}</td><td>{producto.cantidadDoz ?? '-'}</td><td><span className="tag">{producto.enviado ? 'Enviado' : 'Pendiente'}</span></td><td className="actions"><button className="export-button export-excel export-inline" type="button" disabled={exportando !== null} onClick={() => exportar('excel', producto)} title="Descargar este subpedido en Excel" aria-label="Descargar este subpedido en Excel"><FontAwesomeIcon icon={faFileExcel} /></button><button className="export-button export-pdf export-inline" type="button" disabled={exportando !== null} onClick={() => exportar('pdf', producto)} title="Descargar este subpedido en PDF" aria-label="Descargar este subpedido en PDF"><FontAwesomeIcon icon={faFilePdf} /></button><button className="link-button" disabled={producto.enviado} title={producto.enviado ? 'Un pedido enviado no puede editarse' : undefined} onClick={() => editar(producto)}>Editar</button><button className="danger-button" disabled={producto.enviado} title={producto.enviado ? 'Un pedido enviado no puede eliminarse' : undefined} onClick={() => eliminar(producto)}>Eliminar</button><button className="link-button" disabled={producto.enviado} title={producto.enviado ? 'Este pedido ya fue enviado' : undefined} onClick={() => enviar(producto)}>{producto.enviado ? 'Enviado' : 'Enviar'}</button></td></tr>
}
function CampoImagenProducto({ etiqueta, tipo, productoId, tieneImagen, archivo, ocupada, seleccionar, actualizar, eliminar, error }: { etiqueta: string; tipo: TipoImagenProductoPedido; productoId: string | null; tieneImagen: boolean; archivo: File | null; ocupada: boolean; seleccionar: (archivo: File | null) => void; actualizar: (archivo: File) => void; eliminar: () => void; error: (mensaje: string) => void }) { const idInput = `imagen-${tipo}`; const validar = (archivo: File | null) => { if (archivo && archivo.size > 15 * 1024 * 1024) { error('La imagen no puede superar 15 MB.'); return null }; return archivo }; if (productoId && tieneImagen) return <div className="image-product-field"><span>{etiqueta}</span><div className="image-current-preview image-current-actions"><ImagenProducto id={productoId} tipo={tipo} tieneImagen /><span><strong>Imagen actual</strong><small>Administra la imagen.</small></span><div className="image-actions"><input id={idInput} className="image-upload-input" type="file" accept="image/jpeg,image/png,image/webp" onChange={(e) => { const archivo = validar(e.target.files?.[0] ?? null); if (archivo) actualizar(archivo); e.target.value = '' }} /><label htmlFor={idInput} className="image-action image-action-primary" title="Actualizar imagen" aria-label={`Actualizar ${etiqueta}`}><FontAwesomeIcon icon={faCloudArrowUp} /></label><button type="button" className="image-action image-action-danger" title="Eliminar imagen" aria-label={`Eliminar ${etiqueta}`} disabled={ocupada} onClick={eliminar}><FontAwesomeIcon icon={faTrash} /></button></div></div></div>; return <label className="image-upload-field"><span>{etiqueta}</span><input className="image-upload-input" type="file" accept="image/jpeg,image/png,image/webp" onChange={(e) => seleccionar(validar(e.target.files?.[0] ?? null))} /><span className="image-upload-control"><span className="image-upload-icon" aria-hidden="true"><FontAwesomeIcon icon={faCloudArrowUp} /></span><span className="image-upload-copy"><strong>Seleccionar imagen</strong><small>{archivo?.name ?? 'JPEG, PNG o WebP · Máximo 15 MB'}</small></span></span></label> }
function ImagenProducto({ id, tipo, tieneImagen }: { id: string; tipo: TipoImagenProductoPedido; tieneImagen: boolean }) { const [url, setUrl] = useState<string | null>(null); useEffect(() => { if (!tieneImagen) { setUrl(null); return }; let activa = true; let creada: string | null = null; void pedidosApi.obtenerImagen(id, tipo).then((valor) => { creada = valor; if (activa) setUrl(valor) }).catch(() => { if (activa) setUrl(null) }); return () => { activa = false; if (creada) URL.revokeObjectURL(creada) } }, [id, tipo, tieneImagen]); return <span className="product-thumbnail-frame">{url ? <img className="product-thumbnail" src={url} alt="Imagen del producto" /> : <span className="image-placeholder">Sin<br />imagen</span>}</span> }
type ImagenesParaExportacion = { fabrica: string | null; productoTerminado: string | null }

async function cargarImagenesParaExportacion(producto: ProductoPedido): Promise<ImagenesParaExportacion> {
  const [fabrica, productoTerminado] = await Promise.all([
    cargarImagenComoDataUrl(producto.id, 'fabrica', producto.tieneImagenFabrica),
    cargarImagenComoDataUrl(producto.id, 'producto-terminado', producto.tieneImagenProductoTerminado),
  ])
  return { fabrica, productoTerminado }
}

async function cargarImagenComoDataUrl(id: string, tipo: TipoImagenProductoPedido, existe: boolean) {
  if (!existe) return null
  const url = await pedidosApi.obtenerImagen(id, tipo)
  if (!url) return null
  try {
    const respuesta = await fetch(url)
    if (!respuesta.ok) return null
    return await convertirImagenAPng(await respuesta.blob())
  } finally { URL.revokeObjectURL(url) }
}

async function convertirImagenAPng(blob: Blob) {
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

const filaExportableVacia = { 'Grupo': '', 'Agente': '', 'Tipo de pedido': '', 'Imagen de fábricas': '', 'Fábrica': '', 'Composición tela': '', 'Color para fabricar': '', 'Marca del producto': '', 'Curva talla': '', 'Pack por cajas': '', 'Cantidad de unidades': '', 'Marca del bulto': '', 'Cantidad de bulto': '', 'Imagen producto terminado': '', 'Referencia asignada': '', 'Código barra asignado': '', 'Precio (¥ RMB)': '', 'Total (¥ RMB)': '', 'Cantidad DOZ': '', 'Estado': '' }
function filaExportable(x: ProductoPedido, imagenes?: ImagenesParaExportacion) { return { 'Grupo': x.grupoPedidoNombre ?? 'Sin grupo', 'Agente': x.agente ?? '', 'Tipo de pedido': x.tipoProducto ?? '', 'Imagen de fábricas': imagenes?.fabrica ? '' : 'Sin imagen', 'Fábrica': x.fabrica ?? '', 'Composición tela': x.composicionTela ?? '', 'Color para fabricar': x.colorParaFabricar ?? '', 'Marca del producto': x.marcaProducto ?? '', 'Curva talla': x.curvaTalla ?? '', 'Pack por cajas': x.packPorCaja ?? '', 'Cantidad de unidades': x.cantidadUnidades ?? '', 'Marca del bulto': x.marcaBulto ?? '', 'Cantidad de bulto': x.cantidadBulto ?? '', 'Imagen producto terminado': imagenes?.productoTerminado ? '' : 'Sin imagen', 'Referencia asignada': x.referenciaAsignada, 'Código barra asignado': x.codigoBarraAsignado, 'Precio (¥ RMB)': formatoYuan(x.precioRmb), 'Total (¥ RMB)': formatoYuan(x.totalRmb), 'Cantidad DOZ': x.cantidadDoz ?? '', 'Estado': x.enviado ? 'Enviado' : 'Pendiente' } }

function descargarExcelCompatible(filas: Record<string, string | number>[], nombreArchivo: string, imagenes: ImagenesParaExportacion[]) {
  const encabezados = Object.keys(filas[0] ?? filaExportableVacia)
  const columnas = [22, 16, 18, 16, 20, 22, 20, 20, 15, 14, 18, 18, 14, 18, 22, 22, 14, 14, 14, 14]
  const referenciasImagenes: { datos: Uint8Array; fila: number; columna: number }[] = []
  imagenes.forEach((imagen, indice) => {
    if (imagen.fabrica) referenciasImagenes.push({ datos: dataUrlABytes(imagen.fabrica), fila: indice + 1, columna: 3 })
    if (imagen.productoTerminado) referenciasImagenes.push({ datos: dataUrlABytes(imagen.productoTerminado), fila: indice + 1, columna: 13 })
  })
  const filasXml = [encabezados, ...filas.map((fila) => encabezados.map((encabezado) => fila[encabezado] ?? ''))].map((fila, indiceFila) => {
    const celdas = fila.map((valor, indiceColumna) => celdaXlsx(indiceColumna, indiceFila + 1, valor, indiceFila === 0 ? 1 : 2)).join('')
    const atributos = indiceFila === 0 ? ' ht="22" customHeight="1"' : ' ht="58" customHeight="1"'
    return `<row r="${indiceFila + 1}"${atributos}>${celdas}</row>`
  }).join('')
  const tieneImagenes = referenciasImagenes.length > 0
  const columnasXml = columnas.map((ancho, indice) => `<col min="${indice + 1}" max="${indice + 1}" width="${ancho}" customWidth="1"/>`).join('')
  const hoja = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><dimension ref="A1:T${filas.length + 1}"/><sheetViews><sheetView workbookViewId="0"/></sheetViews><cols>${columnasXml}</cols><sheetData>${filasXml}</sheetData>${tieneImagenes ? '<drawing r:id="rId1"/>' : ''}</worksheet>`
  const entradas: EntradaZip[] = [
    { nombre: '[Content_Types].xml', datos: textoABytes(`<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/>${tieneImagenes ? '<Default Extension="png" ContentType="image/png"/><Override PartName="/xl/drawings/drawing1.xml" ContentType="application/vnd.openxmlformats-officedocument.drawing+xml"/>' : ''}<Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/><Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/><Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/></Types>`) },
    { nombre: '_rels/.rels', datos: textoABytes('<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/></Relationships>') },
    { nombre: 'xl/workbook.xml', datos: textoABytes('<?xml version="1.0" encoding="UTF-8" standalone="yes"?><workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><sheets><sheet name="Pedidos" sheetId="1" r:id="rId1"/></sheets></workbook>') },
    { nombre: 'xl/_rels/workbook.xml.rels', datos: textoABytes('<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/><Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/></Relationships>') },
    { nombre: 'xl/styles.xml', datos: textoABytes('<?xml version="1.0" encoding="UTF-8" standalone="yes"?><styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><fonts count="2"><font><sz val="10"/><name val="Arial"/></font><font><b/><color rgb="FF173D75"/><sz val="10"/><name val="Arial"/></font></fonts><fills count="3"><fill><patternFill patternType="none"/></fill><fill><patternFill patternType="gray125"/></fill><fill><patternFill patternType="solid"><fgColor rgb="FFEEF4FB"/><bgColor indexed="64"/></patternFill></fill></fills><borders count="2"><border><left/><right/><top/><bottom/><diagonal/></border><border><left style="thin"><color rgb="FFC8D5E6"/></left><right style="thin"><color rgb="FFC8D5E6"/></right><top style="thin"><color rgb="FFC8D5E6"/></top><bottom style="thin"><color rgb="FFC8D5E6"/></bottom><diagonal/></border></borders><cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs><cellXfs count="3"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/><xf numFmtId="0" fontId="1" fillId="2" borderId="1" applyFont="1" applyFill="1" applyBorder="1" applyAlignment="1"><alignment horizontal="center" vertical="center" wrapText="1"/></xf><xf numFmtId="0" fontId="0" fillId="0" borderId="1" applyBorder="1" applyAlignment="1"><alignment vertical="center" wrapText="1"/></xf></cellXfs></styleSheet>') },
    { nombre: 'xl/worksheets/sheet1.xml', datos: textoABytes(hoja) },
  ]
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

type EntradaZip = { nombre: string; datos: Uint8Array }
function textoABytes(valor: string) { return new TextEncoder().encode(valor) }
function dataUrlABytes(url: string) { const base64 = url.slice(url.indexOf(',') + 1); const binario = atob(base64); return Uint8Array.from(binario, (caracter) => caracter.charCodeAt(0)) }
function escaparXml(valor: string | number) { return String(valor).replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;').replaceAll('"', '&quot;').replaceAll("'", '&apos;') }
function nombreColumnaXlsx(indice: number) { let resultado = ''; let valor = indice + 1; while (valor > 0) { const resto = (valor - 1) % 26; resultado = String.fromCharCode(65 + resto) + resultado; valor = Math.floor((valor - 1) / 26) } return resultado }
function celdaXlsx(columna: number, fila: number, valor: string | number, estilo: number) { return `<c r="${nombreColumnaXlsx(columna)}${fila}" t="inlineStr" s="${estilo}"><is><t>${escaparXml(valor)}</t></is></c>` }
function xmlDibujoXlsx(imagenes: { datos: Uint8Array; fila: number; columna: number }[]) { return `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><xdr:wsDr xmlns:xdr="http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing" xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">${imagenes.map((imagen, indice) => `<xdr:twoCellAnchor editAs="oneCell"><xdr:from><xdr:col>${imagen.columna}</xdr:col><xdr:colOff>95250</xdr:colOff><xdr:row>${imagen.fila}</xdr:row><xdr:rowOff>47625</xdr:rowOff></xdr:from><xdr:to><xdr:col>${imagen.columna + 1}</xdr:col><xdr:colOff>0</xdr:colOff><xdr:row>${imagen.fila + 1}</xdr:row><xdr:rowOff>0</xdr:rowOff></xdr:to><xdr:pic><xdr:nvPicPr><xdr:cNvPr id="${indice + 1}" name="Imagen ${indice + 1}"/><xdr:cNvPicPr/></xdr:nvPicPr><xdr:blipFill><a:blip r:embed="rId${indice + 1}"/><a:stretch><a:fillRect/></a:stretch></xdr:blipFill><xdr:spPr><a:xfrm><a:off x="0" y="0"/><a:ext cx="0" cy="0"/></a:xfrm><a:prstGeom prst="rect"><a:avLst/></a:prstGeom></xdr:spPr></xdr:pic><xdr:clientData/></xdr:twoCellAnchor>`).join('')}</xdr:wsDr>` }
function crc32(datos: Uint8Array) { let crc = 0xffffffff; for (const dato of datos) { crc ^= dato; for (let indice = 0; indice < 8; indice++) crc = (crc >>> 1) ^ (0xedb88320 & -(crc & 1)) } return (crc ^ 0xffffffff) >>> 0 }
function entero16(valor: number) { const bytes = new Uint8Array(2); new DataView(bytes.buffer).setUint16(0, valor, true); return bytes }
function entero32(valor: number) { const bytes = new Uint8Array(4); new DataView(bytes.buffer).setUint32(0, valor, true); return bytes }
function empaquetarZip(entradas: EntradaZip[]) { const locales: Uint8Array[] = []; const centrales: Uint8Array[] = []; let desplazamiento = 0; for (const entrada of entradas) { const nombre = textoABytes(entrada.nombre); const crc = crc32(entrada.datos); const local = new Uint8Array(30 + nombre.length); local.set(entero32(0x04034b50), 0); local.set(entero16(20), 4); local.set(entero16(0x0800), 6); local.set(entero16(0), 8); local.set(entero16(0), 10); local.set(entero16(0), 12); local.set(entero32(crc), 14); local.set(entero32(entrada.datos.length), 18); local.set(entero32(entrada.datos.length), 22); local.set(entero16(nombre.length), 26); local.set(entero16(0), 28); local.set(nombre, 30); locales.push(local, entrada.datos); const central = new Uint8Array(46 + nombre.length); central.set(entero32(0x02014b50), 0); central.set(entero16(20), 4); central.set(entero16(20), 6); central.set(entero16(0x0800), 8); central.set(entero16(0), 10); central.set(entero16(0), 12); central.set(entero16(0), 14); central.set(entero32(crc), 16); central.set(entero32(entrada.datos.length), 20); central.set(entero32(entrada.datos.length), 24); central.set(entero16(nombre.length), 28); central.set(entero16(0), 30); central.set(entero16(0), 32); central.set(entero16(0), 34); central.set(entero16(0), 36); central.set(entero32(0), 38); central.set(entero32(desplazamiento), 42); central.set(nombre, 46); centrales.push(central); desplazamiento += local.length + entrada.datos.length } const tamanoCentral = centrales.reduce((total, parte) => total + parte.length, 0); const fin = new Uint8Array(22); fin.set(entero32(0x06054b50), 0); fin.set(entero16(0), 4); fin.set(entero16(0), 6); fin.set(entero16(entradas.length), 8); fin.set(entero16(entradas.length), 10); fin.set(entero32(tamanoCentral), 12); fin.set(entero32(desplazamiento), 16); fin.set(entero16(0), 20); const partes = [...locales, ...centrales, fin].map((parte) => parte.buffer.slice(parte.byteOffset, parte.byteOffset + parte.byteLength) as ArrayBuffer); return new Blob(partes, { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' }) }
function descargarBlob(archivo: Blob, nombre: string) { const url = URL.createObjectURL(archivo); const enlace = document.createElement('a'); enlace.href = url; enlace.download = nombre; enlace.click(); URL.revokeObjectURL(url) }