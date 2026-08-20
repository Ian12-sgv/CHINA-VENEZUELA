import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faCloudArrowUp, faTrash } from '@fortawesome/free-solid-svg-icons'
import { pedidosApi, receptoresApi } from '../api'
import { ConfirmDeleteDialog } from '../components/ConfirmDeleteDialog'
import type {
  CrearProductoPedidoRequest,
  PaginaProductosPedido,
  ProductoPedido,
  RegistroPrecioPedido,
} from '../types'

const crearFormularioInicial = (): CrearProductoPedidoRequest => ({
  codigoBarra: '',
  referencia: '',
  nombre: '',
  marca: null,
  categoria: '',
  talla: null,
  color: null,
  fabricante: null,
  precioDetal: 0,
  costo: 0,
  fechaPedido: new Date().toISOString().slice(0, 10),
})

type FiltroEnvio = 'todos' | 'enviados' | 'pendientes'

export function PedidosPage() {
  const client = useQueryClient()
  const [tab, setTab] = useState<'productos' | 'precios'>('productos')
  const [form, setForm] = useState<CrearProductoPedidoRequest>(crearFormularioInicial)
  const [busqueda, setBusqueda] = useState('')
  const [fechaFiltro, setFechaFiltro] = useState('')
  const [filtroEnvio, setFiltroEnvio] = useState<FiltroEnvio>('todos')
  const [pagina, setPagina] = useState(1)
  const [error, setError] = useState('')
  const [editingId, setEditingId] = useState<string | null>(null)
  const [productoEditando, setProductoEditando] = useState<ProductoPedido | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<ProductoPedido | null>(null)
  const [productoEnviar, setProductoEnviar] = useState<ProductoPedido | null>(null)
  const [receptorCodigo, setReceptorCodigo] = useState('')
  const [confirmarEliminarFoto, setConfirmarEliminarFoto] = useState(false)
  const [imagenArchivo, setImagenArchivo] = useState<File | null>(null)

  const enviado = filtroEnvio === 'todos' ? undefined : filtroEnvio === 'enviados'
  const productos = useQuery({
    queryKey: ['pedidos-productos', pagina, busqueda, fechaFiltro, filtroEnvio],
    queryFn: () => pedidosApi.productos(pagina, busqueda, fechaFiltro, enviado),
  })
  const precios = useQuery({ queryKey: ['pedidos-precios'], queryFn: pedidosApi.registrosPrecios })
  const receptores = useQuery({ queryKey: ['receptores'], queryFn: receptoresApi.listar })

  const guardar = useMutation({
    mutationFn: async () => {
      const producto = editingId
        ? await pedidosApi.actualizarProducto(editingId, form)
        : await pedidosApi.crearProducto(form)
      if (imagenArchivo) await pedidosApi.subirImagen(producto.id, imagenArchivo)
      return producto
    },
    onSuccess: () => {
      setForm(crearFormularioInicial())
      setImagenArchivo(null)
      setEditingId(null)
      setProductoEditando(null)
      setError('')
      setPagina(1)
      void client.invalidateQueries({ queryKey: ['pedidos-productos'] })
    },
    onError: (e) => setError(e instanceof Error ? e.message : 'No fue posible guardar.'),
  })

  const eliminar = useMutation({
    mutationFn: pedidosApi.eliminarProducto,
    onSuccess: () => {
      setDeleteTarget(null)
      void client.invalidateQueries({ queryKey: ['pedidos-productos'] })
    },
    onError: (e) => {
      setError(e instanceof Error ? e.message : 'No fue posible eliminar.')
      setDeleteTarget(null)
    },
  })

  const actualizarImagen = useMutation({
    mutationFn: (archivo: File) => editingId ? pedidosApi.subirImagen(editingId, archivo) : Promise.reject(new Error('Selecciona un producto.')),
    onSuccess: () => {
      setError('')
      void client.invalidateQueries({ queryKey: ['pedidos-productos'] })
    },
    onError: (e) => setError(e instanceof Error ? e.message : 'No fue posible actualizar la foto.'),
  })

  const eliminarImagen = useMutation({
    mutationFn: () => editingId ? pedidosApi.eliminarImagen(editingId) : Promise.reject(new Error('Selecciona un producto.')),
    onSuccess: () => {
      setConfirmarEliminarFoto(false)
      setProductoEditando((producto) => producto ? { ...producto, tieneImagen: false } : producto)
      setError('')
      void client.invalidateQueries({ queryKey: ['pedidos-productos'] })
    },
    onError: (e) => { setConfirmarEliminarFoto(false); setError(e instanceof Error ? e.message : 'No fue posible eliminar la foto.') },
  })
  const enviar = useMutation({
    mutationFn: () => productoEnviar && receptorCodigo
      ? pedidosApi.enviarProducto(productoEnviar.id, receptorCodigo)
      : Promise.reject(new Error('Selecciona un receptor.')),
    onSuccess: () => {
      setProductoEnviar(null)
      setReceptorCodigo('')
      setError('')
      void client.invalidateQueries({ queryKey: ['pedidos-productos'] })
    },
    onError: (e) => setError(e instanceof Error ? e.message : 'No fue posible enviar.'),
  })

  const field = (key: keyof CrearProductoPedidoRequest, value: string) =>
    setForm((current) => ({ ...current, [key]: value || null }))

  const editar = (producto: ProductoPedido) => {
    if (producto.enviado) return
    setEditingId(producto.id)
    setProductoEditando(producto)
    setImagenArchivo(null)
    setForm({
      codigoBarra: producto.codigoBarra,
      referencia: producto.referencia,
      nombre: producto.nombre,
      marca: producto.marca,
      categoria: producto.categoria,
      talla: producto.talla,
      color: producto.color,
      fabricante: producto.fabricante,
      precioDetal: producto.precioDetal,
      costo: producto.costo,
      fechaPedido: producto.fechaPedido,
    })
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  return <section className="page-grid">
    <div className="page-heading">
      <div>
        <span className="eyebrow">Operación</span>
        <h2>Pedidos</h2>
        <p>Catálogo de productos y consulta de precios.</p>
      </div>
    </div>

    <div className="tabs">
      <button className={tab === 'productos' ? 'active' : ''} onClick={() => setTab('productos')}>Productos</button>
      <button className={tab === 'precios' ? 'active' : ''} onClick={() => setTab('precios')}>Registros de precios</button>
    </div>

    {tab === 'productos' && <>
      <article className="card form-card">
        <div className="form-title">
          <div>
            <h3>{editingId ? 'Editar producto' : 'Nuevo producto'}</h3>
            <p>Registra los productos disponibles para pedidos.</p>
          </div>
          {editingId && <button className="link-button" onClick={() => { setEditingId(null); setProductoEditando(null); setImagenArchivo(null); setForm(crearFormularioInicial()) }}>Cancelar</button>}
        </div>
        {error && <p className="error">{error}</p>}
        <form className="form-grid" onSubmit={(e: FormEvent) => { e.preventDefault(); guardar.mutate() }}>
          <label><span>Código de barra</span><input required value={form.codigoBarra} onChange={(e) => field('codigoBarra', e.target.value)} /></label>
          <label><span>Referencia</span><input required value={form.referencia} onChange={(e) => field('referencia', e.target.value)} /></label>
          <label><span>Nombre</span><input required value={form.nombre} onChange={(e) => field('nombre', e.target.value)} /></label>
          <label><span>Categoría</span><input required value={form.categoria} onChange={(e) => field('categoria', e.target.value)} /></label>
          <label><span>Marca</span><input value={form.marca ?? ''} onChange={(e) => field('marca', e.target.value)} /></label>
          <label><span>Fabricante</span><input value={form.fabricante ?? ''} onChange={(e) => field('fabricante', e.target.value)} /></label>
          <label><span>Color</span><input value={form.color ?? ''} onChange={(e) => field('color', e.target.value)} /></label>
          <label><span>Talla</span><input value={form.talla ?? ''} onChange={(e) => field('talla', e.target.value)} /></label>
          <label><span>Precio detal</span><input required type="number" min="0" step="0.01" value={form.precioDetal} onChange={(e) => setForm((x) => ({ ...x, precioDetal: Number(e.target.value) }))} /></label>
          <label><span>Costo</span><input required type="number" min="0" step="0.01" value={form.costo} onChange={(e) => setForm((x) => ({ ...x, costo: Number(e.target.value) }))} /></label>
          <label><span>Fecha del pedido</span><input required type="date" value={form.fechaPedido} onChange={(e) => setForm((x) => ({ ...x, fechaPedido: e.target.value }))} /></label>
          {editingId && productoEditando?.tieneImagen ? <div className="image-product-field">
            <span>Imagen del producto</span>
            <div className="image-current-preview image-current-actions">
              <ImagenProducto id={productoEditando.id} tieneImagen />
              <span><strong>Imagen actual</strong><small>Administra la imagen.</small></span>
              <div className="image-actions">
                <input id="actualizar-imagen" className="image-upload-input" type="file" accept="image/jpeg,image/png,image/webp" onChange={(e) => { const archivo = e.target.files?.[0] ?? null; if (archivo && archivo.size > 15 * 1024 * 1024) { setError('La imagen no puede superar 15 MB.'); e.target.value = ''; return } if (archivo) actualizarImagen.mutate(archivo); e.target.value = '' }} />
                <label htmlFor="actualizar-imagen" className="image-action image-action-primary" title="Actualizar foto" aria-label="Actualizar foto"><FontAwesomeIcon icon={faCloudArrowUp} /></label>
                <button type="button" className="image-action image-action-danger" title="Eliminar foto" aria-label="Eliminar foto" disabled={eliminarImagen.isPending} onClick={() => setConfirmarEliminarFoto(true)}><FontAwesomeIcon icon={faTrash} /></button>
              </div>
            </div>
          </div> : <label className="image-upload-field">
            <span>Imagen del producto</span>
            <input className="image-upload-input" type="file" accept="image/jpeg,image/png,image/webp" onChange={(e) => { const archivo = e.target.files?.[0] ?? null; if (archivo && archivo.size > 15 * 1024 * 1024) { setError('La imagen no puede superar 15 MB.'); e.target.value = ''; return } setImagenArchivo(archivo); setError('') }} />
            <span className="image-upload-control"><span className="image-upload-icon" aria-hidden="true">↑</span><span className="image-upload-copy"><strong>Seleccionar imagen</strong><small>{imagenArchivo?.name ?? 'JPEG, PNG o WebP · Máximo 15 MB'}</small></span></span>
          </label>}          <div className="description-field"><button className="primary" disabled={guardar.isPending}>{guardar.isPending ? 'Guardando...' : editingId ? 'Guardar cambios' : 'Registrar producto'}</button></div>
        </form>
      </article>

      <Tabla
        pagina={productos.data}
        busqueda={busqueda}
        fecha={fechaFiltro}
        filtroEnvio={filtroEnvio}
        cargar={productos.isLoading}
        buscar={(value) => { setBusqueda(value); setPagina(1) }}
        filtrarFecha={(value) => { setFechaFiltro(value); setPagina(1) }}
        filtrarEnvio={(value) => { setFiltroEnvio(value); setPagina(1) }}
        irPagina={setPagina}
        editar={editar}
        eliminar={setDeleteTarget}
        enviar={setProductoEnviar}
      />

      {productoEnviar && <article className="card form-card">
        <div className="form-title">
          <div><h3>Enviar producto</h3><p>{productoEnviar.nombre}</p></div>
          <button className="link-button" onClick={() => setProductoEnviar(null)}>Cancelar</button>
        </div>
        <label>
          <span>Receptor</span>
          <select value={receptorCodigo} onChange={(e) => setReceptorCodigo(e.target.value)}>
            <option value="">Selecciona un usuario</option>
            {receptores.data?.map((x) => <option key={x.codigoUsuario} value={x.codigoUsuario}>{x.nombre}</option>)}
          </select>
        </label>
        <button className="primary" disabled={enviar.isPending || !receptorCodigo} onClick={() => enviar.mutate()}>{enviar.isPending ? 'Enviando...' : 'Enviar por correo'}</button>
      </article>}

      <ConfirmDeleteDialog open={confirmarEliminarFoto} itemName="la foto actual" onCancel={() => setConfirmarEliminarFoto(false)} onConfirm={() => eliminarImagen.mutate()} />
      <ConfirmDeleteDialog open={deleteTarget !== null} itemName={deleteTarget?.nombre ?? ''} onCancel={() => setDeleteTarget(null)} onConfirm={() => deleteTarget && eliminar.mutate(deleteTarget.id)} />
    </>}

    {tab === 'precios' && <Precios items={precios.data ?? []} />}
  </section>
}

function Tabla({ pagina, busqueda, fecha, filtroEnvio, cargar, buscar, filtrarFecha, filtrarEnvio, irPagina, editar, eliminar, enviar }: {
  pagina: PaginaProductosPedido | undefined
  busqueda: string
  fecha: string
  filtroEnvio: FiltroEnvio
  cargar: boolean
  buscar: (x: string) => void
  filtrarFecha: (x: string) => void
  filtrarEnvio: (x: FiltroEnvio) => void
  irPagina: (x: number) => void
  editar: (x: ProductoPedido) => void
  eliminar: (x: ProductoPedido) => void
  enviar: (x: ProductoPedido) => void
}) {
  const items = pagina?.items ?? []
  return <article className="card table-card">
    <div className="table-toolbar">
      <div><h3>Catálogo de productos</h3><p>{pagina?.total ?? 0} artículos · {pagina?.totalPaginas ?? 0} páginas</p></div>
      <div>
        <select aria-label="Filtrar por envío" value={filtroEnvio} onChange={(e) => filtrarEnvio(e.target.value as FiltroEnvio)}>
          <option value="todos">Todos</option>
          <option value="enviados">Enviados</option>
          <option value="pendientes">Pendientes de envío</option>
        </select>
        <input type="date" aria-label="Filtrar por fecha" value={fecha} onChange={(e) => filtrarFecha(e.target.value)} />
        <input placeholder="Buscar" value={busqueda} onChange={(e) => buscar(e.target.value)} />
      </div>
    </div>
    <div className="table-wrap"><table>
      <thead><tr><th>Fecha</th><th>Código</th><th>Referencia</th><th>Producto</th><th>Imagen</th><th>Marca</th><th>Categoría</th><th>Precio</th><th>Costo</th><th>Estado</th><th>Acciones</th></tr></thead>
      <tbody>
        {cargar ? <tr><td colSpan={11} className="empty">Cargando...</td></tr>
          : items.length === 0 ? <tr><td colSpan={11} className="empty">No hay productos.</td></tr>
            : items.map((x) => <tr key={x.id}>
              <td>{new Date(`${x.fechaPedido}T00:00:00`).toLocaleDateString('es-VE')}</td>
              <td>{x.codigoBarra}</td><td>{x.referencia}</td><td><strong>{x.nombre}</strong></td><td><ImagenProducto id={x.id} tieneImagen={x.tieneImagen} /></td><td>{x.marca ?? '-'}</td><td>{x.categoria}</td>
              <td>${x.precioDetal.toFixed(2)}</td><td>${x.costo.toFixed(2)}</td>
              <td><span className="tag">{x.enviado ? 'Enviado' : 'Pendiente'}</span></td>
              <td className="actions">
                <button className="link-button" disabled={x.enviado} title={x.enviado ? 'Un pedido enviado no puede editarse' : undefined} onClick={() => editar(x)}>Editar</button>
                <button className="danger-button" disabled={x.enviado} title={x.enviado ? "Un pedido enviado no puede eliminarse" : undefined} onClick={() => eliminar(x)}>Eliminar</button>
                <button className="link-button" disabled={x.enviado} title={x.enviado ? 'Este pedido ya fue enviado' : undefined} onClick={() => enviar(x)}>{x.enviado ? 'Enviado' : 'Enviar'}</button>
              </td>
            </tr>)}
      </tbody>
    </table></div>
    {(pagina?.totalPaginas ?? 0) > 1 && <div className="table-toolbar">
      <button className="secondary" disabled={pagina!.pagina <= 1} onClick={() => irPagina(pagina!.pagina - 1)}>Anterior</button>
      <span>Página {pagina!.pagina} de {pagina!.totalPaginas}</span>
      <button className="secondary" disabled={pagina!.pagina >= pagina!.totalPaginas} onClick={() => irPagina(pagina!.pagina + 1)}>Siguiente</button>
    </div>}
  </article>
}

function ImagenProducto({ id, tieneImagen }: { id: string; tieneImagen: boolean }) {
  const [url, setUrl] = useState<string | null>(null)
  useEffect(() => {
    if (!tieneImagen) { setUrl(null); return }
    let activa = true
    let creada: string | null = null
    void pedidosApi.obtenerImagen(id).then((valor) => { creada = valor; if (activa) setUrl(valor) }).catch(() => { if (activa) setUrl(null) })
    return () => { activa = false; if (creada) URL.revokeObjectURL(creada) }
  }, [id, tieneImagen])
  return <span className="product-thumbnail-frame">{url ? <img className="product-thumbnail" src={url} alt="Imagen del producto" /> : <span className="image-placeholder">Sin<br />imagen</span>}</span>
}
function Precios({ items }: { items: RegistroPrecioPedido[] }) {
  return <article className="card table-card">
    <div className="table-toolbar"><h3>Registros de precios</h3><span>{items.length} artículos</span></div>
    <div className="table-wrap"><table>
      <thead><tr><th>Código</th><th>Producto</th><th>Sucursal</th><th>Precio sistema</th><th>Precio verificado</th></tr></thead>
      <tbody>{items.length === 0 ? <tr><td colSpan={5} className="empty">No hay registros de precios.</td></tr> : items.map((x) => <tr key={x.id}><td>{x.codigoBarra}</td><td>{x.producto}</td><td>{x.sucursal}</td><td>${x.precioSistema.toFixed(2)}</td><td>${x.precioVerificado.toFixed(2)}</td></tr>)}</tbody>
    </table></div>
  </article>
}
