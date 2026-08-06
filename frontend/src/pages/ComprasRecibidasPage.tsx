import { useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { ConfirmDeleteDialog } from '../components/ConfirmDeleteDialog'
import { faMagnifyingGlass } from '@fortawesome/free-solid-svg-icons'
import { catalogosApi, comprasApi } from '../api'
import type { Catalogo, CompraRecibida, CompraRecibidaRequest } from '../types'

const emptyForm: CompraRecibidaRequest = { contenedorCompartidoId: null, nombreContenedor: '', numeroContenedor: '', empresaId: '', descripcion: null, fechaSalida: '', fechaLlegada: null, aduana: null, puertoLlegada: '', marcaBultoId: null }
const date = (value: string | null) => value ? new Date(`${value}T00:00:00`).toLocaleDateString('es-VE') : '-'

export function ComprasRecibidasPage() {
  const client = useQueryClient(); const [form, setForm] = useState<CompraRecibidaRequest>(emptyForm); const [editingId, setEditingId] = useState<string | null>(null); const [deleteTarget, setDeleteTarget] = useState<CompraRecibida | null>(null); const [error, setError] = useState(''); const [search, setSearch] = useState('')
  const compras = useQuery({ queryKey: ['compras-recibidas'], queryFn: comprasApi.listar })
  const empresas = useQuery({ queryKey: ['empresas'], queryFn: () => catalogosApi.listar('empresas') })
  const marcas = useQuery({ queryKey: ['marcas-bulto'], queryFn: () => catalogosApi.listar('marcas-bulto') })
  const contenedores = useQuery({ queryKey: ['contenedores-compartidos'], queryFn: () => catalogosApi.listar('contenedores-compartidos') })
  const refresh = () => client.invalidateQueries({ queryKey: ['compras-recibidas'] })
  const reset = () => { setForm(emptyForm); setEditingId(null); setError('') }
  const save = useMutation({ mutationFn: () => editingId ? comprasApi.actualizar(editingId, form) : comprasApi.crear(form), onSuccess: () => { refresh(); reset() }, onError: (e) => setError(e instanceof Error ? e.message : 'No fue posible guardar.') })
  const remove = useMutation({ mutationFn: comprasApi.eliminar, onSuccess: () => { refresh(); setDeleteTarget(null) }, onError: (e) => { setError(e instanceof Error ? e.message : 'No fue posible eliminar.'); setDeleteTarget(null) } })
  const field = (key: keyof CompraRecibidaRequest, value: string) => setForm(current => ({ ...current, [key]: value === '' ? null : value }))
  const submit = (event: FormEvent) => { event.preventDefault(); setError(''); save.mutate() }
  const edit = (item: CompraRecibida) => { setEditingId(item.id); setForm({ contenedorCompartidoId: item.contenedorCompartidoId, nombreContenedor: item.nombreContenedor, numeroContenedor: item.numeroContenedor, empresaId: item.empresaId, descripcion: item.descripcion, fechaSalida: item.fechaSalida, fechaLlegada: item.fechaLlegada, aduana: item.aduana, puertoLlegada: item.puertoLlegada, marcaBultoId: item.marcaBultoId }); window.scrollTo({ top: 0, behavior: 'smooth' }) }
  const name = (list: Catalogo[] | undefined, id: string | null) => list?.find(item => item.id === id)?.nombre ?? '-'
  const filtered = useMemo(() => {
    const term = search.trim().toLocaleLowerCase('es-VE')

    if (!term) {
      return compras.data ?? []
    }

    return (compras.data ?? []).filter(item => {
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
      ]

      return searchableFields
        .join(' ')
        .toLocaleLowerCase('es-VE')
        .includes(term)
    })
  }, [compras.data, empresas.data, marcas.data, search])
  if ([compras, empresas, marcas, contenedores].some(query => query.isLoading)) return <p className="loading">Cargando operacion...</p>
  if ([compras, empresas, marcas, contenedores].some(query => query.isError)) return <p className="error">No fue posible conectar con el API. Verifica que el backend este iniciado.</p>
  return <section className="page-grid"><div className="page-heading"><div><h2>Recibos de compra</h2><p>Registra la mercancia llegada desde China hacia Venezuela.</p></div><span className="badge">{compras.data?.length ?? 0} {compras.data?.length === 1 ? 'registro' : 'registros'}</span></div>
    <form className="card form-card" onSubmit={submit}><div className="form-title"><div className="form-title-icon">+</div><h3>{editingId ? 'Editar recibo de compra' : 'Nuevo recibo de compra'}</h3>{editingId && <button type="button" className="link-button" onClick={reset}>Cancelar edicion</button>}</div>{error && <p className="error">{error}</p>}<div className="form-grid">
      <label><span>Nombre del contenedor</span><input required placeholder="Ej. Contenedor 32" value={form.nombreContenedor} onChange={e => field('nombreContenedor', e.target.value)} /></label><label><span>Numero del contenedor</span><input required placeholder="MSKU-0000000" value={form.numeroContenedor} onChange={e => field('numeroContenedor', e.target.value)} /></label>
      <label><span>Empresa</span><select required value={form.empresaId ?? ''} onChange={e => field('empresaId', e.target.value)}><option value="">Selecciona una empresa</option>{empresas.data?.map(item => <option key={item.id} value={item.id}>{item.nombre}</option>)}</select></label><label><span>Contenedor compartido</span><select value={form.contenedorCompartidoId ?? ''} onChange={e => field('contenedorCompartidoId', e.target.value)}><option value="">No aplica</option>{contenedores.data?.map(item => <option key={item.id} value={item.id}>{item.nombre}</option>)}</select></label>
      <label><span>Fecha de salida</span><input required type="date" value={form.fechaSalida} onChange={e => field('fechaSalida', e.target.value)} /></label><label><span>Fecha de llegada</span><input type="date" value={form.fechaLlegada ?? ''} onChange={e => field('fechaLlegada', e.target.value)} /></label>
      <label><span>Puerto de llegada</span><input required placeholder="Puerto Cabello" value={form.puertoLlegada} onChange={e => field('puertoLlegada', e.target.value)} /></label><label><span>Marca de bulto</span><select value={form.marcaBultoId ?? ''} onChange={e => field('marcaBultoId', e.target.value)}><option value="">No aplica</option>{marcas.data?.map(item => <option key={item.id} value={item.id}>{item.nombre}</option>)}</select></label>
      <label><span>Aduana</span><input placeholder="Aduana principal" value={form.aduana ?? ''} onChange={e => field('aduana', e.target.value)} /></label><label className="description-field"><span>Descripcion</span><textarea placeholder="Detalle de la mercancia" value={form.descripcion ?? ''} onChange={e => field('descripcion', e.target.value)} /></label></div><button className="primary" disabled={save.isPending}>{save.isPending ? 'Guardando...' : editingId ? 'Guardar cambios' : 'Registrar recibo de compra'}</button></form>
    <div className="card table-card"><div className="table-toolbar"><h3>Historial de recibos de compra</h3><div><label className="search"><FontAwesomeIcon icon={faMagnifyingGlass} /><input aria-label="Buscar en todos los campos" placeholder="Buscar en todos los campos" value={search} onChange={e => setSearch(e.target.value)} /></label><button className="refresh" onClick={() => refresh()}>Actualizar</button></div></div><div className="table-wrap"><table><thead><tr><th>Contenedor</th><th>Empresa</th><th>Salida</th><th>Llegada</th><th>Puerto</th><th>Marca</th><th>Acciones</th></tr></thead><tbody>{filtered.length === 0 ? <tr><td colSpan={7} className="empty">Aun no hay recibos de compra.</td></tr> : filtered.map(item => <tr key={item.id}><td><strong>{item.nombreContenedor}</strong><small>{item.numeroContenedor}</small></td><td>{name(empresas.data, item.empresaId)}</td><td>{date(item.fechaSalida)}</td><td>{date(item.fechaLlegada)}</td><td>{item.puertoLlegada}</td><td><span className="tag">{name(marcas.data, item.marcaBultoId)}</span></td><td className="actions"><button className="link-button" onClick={() => edit(item)}>Editar</button><button className="danger-button" onClick={() => setDeleteTarget(item)}>Eliminar</button></td></tr>)}</tbody></table></div></div>
    <ConfirmDeleteDialog open={deleteTarget !== null} itemName={deleteTarget?.numeroContenedor ?? ''} onCancel={() => setDeleteTarget(null)} onConfirm={() => deleteTarget && remove.mutate(deleteTarget.id)} />
  </section>
}
