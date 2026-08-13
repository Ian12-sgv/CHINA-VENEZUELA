import { useState } from 'react'
import type { FormEvent } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { catalogosApi, empresasApi } from '../api'
import { ConfirmDeleteDialog } from '../components/ConfirmDeleteDialog'
import type { CatalogoTipo, ClasificacionEmpresa, Empresa } from '../types'

const sections: { tipo: CatalogoTipo; titulo: string; descripcion: string }[] = [
  { tipo: 'contenedores-compartidos', titulo: 'Contenedor compartido', descripcion: 'Administra las opciones de contenedor compartido.' },
  { tipo: 'empresas', titulo: 'Empresa', descripcion: 'Administra las empresas asociadas a las compras recibidas.' },
  { tipo: 'marcas-bulto', titulo: 'Marca de bulto', descripcion: 'Administra las marcas identificadoras de la carga.' },
  { tipo: 'aduanas', titulo: 'Aduana', descripcion: 'Administra las aduanas disponibles para los recibos.' },
  { tipo: 'puertos-llegada', titulo: 'Puerto de llegada', descripcion: 'Administra los puertos disponibles para los recibos.' },
]

export function CatalogosPage() {
  const [activeType, setActiveType] = useState<CatalogoTipo>('contenedores-compartidos')
  const active = sections.find(section => section.tipo === activeType)!

  return <section className="page-grid">
    <div className="page-heading"><div><span className="eyebrow">Configuracion</span><h2>Catalogos operativos</h2><p>Estos registros alimentan los selectores de las recepciones.</p></div></div>
    <div className="tabs" role="tablist">{sections.map(section => <button key={section.tipo} className={activeType === section.tipo ? 'active' : ''} onClick={() => setActiveType(section.tipo)}>{section.titulo}</button>)}</div>
    {activeType === 'empresas' ? <EmpresasSection /> : <CatalogSection key={active.tipo} {...active} />}
  </section>
}

function EmpresasSection() {
  const client = useQueryClient()
  const [nombre, setNombre] = useState('')
  const [rif, setRif] = useState('')
  const [clasificacion, setClasificacion] = useState<ClasificacionEmpresa>('Oriente')
  const [editando, setEditando] = useState<Empresa | null>(null)
  const [eliminando, setEliminando] = useState<Empresa | null>(null)
  const [error, setError] = useState('')
  const query = useQuery({ queryKey: ['empresas'], queryFn: empresasApi.listar })
  const refresh = () => void client.invalidateQueries({ queryKey: ['empresas'] })
  const save = useMutation({
    mutationFn: () => editando ? empresasApi.actualizar(editando.id, { nombre, rif, clasificacion }) : empresasApi.crear({ nombre, rif, clasificacion }),
    onSuccess: () => { setNombre(''); setRif(''); setClasificacion('Oriente'); setEditando(null); refresh() },
    onError: reason => setError(reason instanceof Error ? reason.message : 'No fue posible guardar la empresa.'),
  })
  const remove = useMutation({
    mutationFn: (id: string) => empresasApi.eliminar(id),
    onSuccess: () => { setEliminando(null); refresh() },
    onError: reason => { setEliminando(null); setError(reason instanceof Error ? reason.message : 'No fue posible eliminar la empresa.') },
  })
  const editar = (empresa: Empresa) => { setEditando(empresa); setNombre(empresa.nombre); setRif(empresa.rif ?? ''); setClasificacion(empresa.clasificacion ?? 'Oriente'); setError('') }
  const cancelar = () => { setEditando(null); setNombre(''); setRif(''); setClasificacion('Oriente') }

  return <article className="card catalog-card">
    <div className="form-title"><div><h3>{editando ? 'Editar empresa' : 'Nueva empresa'}</h3><p>El RIF se normaliza y no puede repetirse.</p></div>{editando && <button className="link-button" onClick={cancelar}>Cancelar</button>}</div>
    <form className="form-grid" onSubmit={(event: FormEvent) => { event.preventDefault(); setError(''); save.mutate() }}>
      <label><span>Nombre</span><input value={nombre} onChange={event => setNombre(event.target.value)} required maxLength={200} /></label>
      <label><span>RIF</span><input value={rif} onChange={event => setRif(event.target.value)} required placeholder="J-12345678-9" maxLength={20} /></label>
      <label><span>Clasificacion</span><select value={clasificacion} onChange={event => setClasificacion(event.target.value as ClasificacionEmpresa)}><option>Oriente</option><option>Occidente</option><option>Aliada</option></select></label>
      <div className="description-field">{error && <p className="error">{error}</p>}<button className="primary" disabled={save.isPending}>{editando ? 'Guardar cambios' : 'Agregar empresa'}</button></div>
    </form>
    <div className="table-wrap empresa-table-wrap"><table className="empresa-table"><thead><tr><th>Nombre</th><th>RIF</th><th>Clasificacion</th><th>Acciones</th></tr></thead><tbody>
      {query.data?.map(empresa => <tr key={empresa.id}><td><strong>{empresa.nombre}</strong></td><td>{empresa.rif ?? 'Pendiente'}</td><td>{empresa.clasificacion ?? 'Pendiente'}</td><td className="actions"><button className="link-button" onClick={() => editar(empresa)}>Editar</button><button className="danger-button" onClick={() => setEliminando(empresa)}>Eliminar</button></td></tr>)}
      {query.data?.length === 0 && <tr><td colSpan={4} className="empty">Sin empresas.</td></tr>}
    </tbody></table></div>
    <ConfirmDeleteDialog open={eliminando !== null} itemName={eliminando?.nombre ?? ''} onCancel={() => setEliminando(null)} onConfirm={() => eliminando && remove.mutate(eliminando.id)} />
  </article>
}

function CatalogSection({ tipo, titulo, descripcion }: typeof sections[number]) {
  const client = useQueryClient()
  const [nombre, setNombre] = useState('')
  const [editingId, setEditingId] = useState<string | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<{ id: string; nombre: string } | null>(null)
  const [error, setError] = useState('')
  const query = useQuery({ queryKey: [tipo], queryFn: () => catalogosApi.listar(tipo) })
  const refresh = () => void client.invalidateQueries({ queryKey: [tipo] })
  const save = useMutation({ mutationFn: () => editingId ? catalogosApi.actualizar(tipo, editingId, nombre) : catalogosApi.crear(tipo, nombre), onSuccess: () => { setNombre(''); setEditingId(null); refresh() }, onError: reason => setError(reason instanceof Error ? reason.message : 'No fue posible guardar.') })
  const remove = useMutation({ mutationFn: (id: string) => catalogosApi.eliminar(tipo, id), onSuccess: () => { refresh(); setDeleteTarget(null) }, onError: reason => { setError(reason instanceof Error ? reason.message : 'No fue posible eliminar.'); setDeleteTarget(null) } })

  return <article className="card catalog-card tab-content"><div className="form-title"><div><h3>{titulo}</h3><p>{descripcion}</p></div></div><form className="inline-form" onSubmit={event => { event.preventDefault(); save.mutate() }}><input value={nombre} onChange={event => setNombre(event.target.value)} required placeholder={`Nombre de ${titulo}`} /><button className="primary">{editingId ? 'Guardar' : 'Agregar'}</button></form>{error && <p className="error">{error}</p>}<ul className="catalog-list">{query.data?.map(item => <li key={item.id}><span>{item.nombre}</span><span><button className="link-button" onClick={() => { setEditingId(item.id); setNombre(item.nombre) }}>Editar</button><button className="danger-button" onClick={() => setDeleteTarget(item)}>Eliminar</button></span></li>)}</ul><ConfirmDeleteDialog open={deleteTarget !== null} itemName={deleteTarget?.nombre ?? ''} onCancel={() => setDeleteTarget(null)} onConfirm={() => deleteTarget && remove.mutate(deleteTarget.id)} /></article>
}