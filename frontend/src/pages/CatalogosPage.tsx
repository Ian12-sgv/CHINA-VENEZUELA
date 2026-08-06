import { useState } from 'react'
import type { FormEvent } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { catalogosApi } from '../api'
import { ConfirmDeleteDialog } from '../components/ConfirmDeleteDialog'
import type { CatalogoTipo } from '../types'

const sections: { tipo: CatalogoTipo; titulo: string; descripcion: string }[] = [
  { tipo: 'contenedores-compartidos', titulo: 'Contenedor compartido', descripcion: 'Administra las opciones de contenedor compartido.' },
  { tipo: 'empresas', titulo: 'Empresa', descripcion: 'Administra las empresas asociadas a las compras recibidas.' },
  { tipo: 'marcas-bulto', titulo: 'Marca de bulto', descripcion: 'Administra las marcas identificadoras de la carga.' },
]

export function CatalogosPage() {
  const [activeType, setActiveType] = useState<CatalogoTipo>('contenedores-compartidos')
  const active = sections.find(section => section.tipo === activeType)!
  return <section className="page-grid"><div className="page-heading"><div><span className="eyebrow">Configuración</span><h2>Catálogos operativos</h2><p>Estos registros alimentan los selectores de las recepciones.</p></div></div>
    <div className="tabs" role="tablist" aria-label="Catálogos">{sections.map(section => <button key={section.tipo} role="tab" aria-selected={activeType === section.tipo} className={activeType === section.tipo ? 'active' : ''} onClick={() => setActiveType(section.tipo)}>{section.titulo}</button>)}</div>
    <CatalogSection key={active.tipo} {...active} />
  </section>
}

function CatalogSection({ tipo, titulo, descripcion }: typeof sections[number]) {
  const client = useQueryClient(); const [nombre, setNombre] = useState(''); const [editingId, setEditingId] = useState<string | null>(null); const [deleteTarget, setDeleteTarget] = useState<{ id: string; nombre: string } | null>(null); const [error, setError] = useState('')
  const query = useQuery({ queryKey: [tipo], queryFn: () => catalogosApi.listar(tipo) })
  const refresh = () => client.invalidateQueries({ queryKey: [tipo] })
  const save = useMutation({ mutationFn: () => editingId ? catalogosApi.actualizar(tipo, editingId, nombre) : catalogosApi.crear(tipo, nombre), onSuccess: () => { setNombre(''); setEditingId(null); refresh() }, onError: (e) => setError(e instanceof Error ? e.message : 'No fue posible guardar el registro.') })
  const remove = useMutation({ mutationFn: (id: string) => catalogosApi.eliminar(tipo, id), onSuccess: () => { refresh(); setDeleteTarget(null) }, onError: (e) => { setError(e instanceof Error ? e.message : 'No fue posible eliminar el registro.'); setDeleteTarget(null) } })
  const submit = (event: FormEvent) => { event.preventDefault(); setError(''); save.mutate() }
  const edit = (id: string, value: string) => { setEditingId(id); setNombre(value); setError('') }
  return <article className="card catalog-card tab-content"><div className="form-title"><div><h3>{titulo}</h3><p>{descripcion}</p></div>{editingId && <button className="link-button" onClick={() => { setEditingId(null); setNombre('') }}>Cancelar</button>}</div><form className="inline-form" onSubmit={submit}><input aria-label={`Nombre de ${titulo}`} placeholder={`Nombre de ${titulo}`} value={nombre} onChange={e => setNombre(e.target.value)} /><button className="primary" disabled={save.isPending}>{editingId ? 'Guardar cambios' : 'Agregar'}</button></form>{error && <p className="error">{error}</p>}<ul className="catalog-list">{query.data?.map(item => <li key={item.id}><span>{item.nombre}</span><span><button className="link-button" onClick={() => edit(item.id, item.nombre)}>Editar</button><button className="danger-button" onClick={() => window.confirm(`¿Eliminar “${item.nombre}”?`) && remove.mutate(item.id)}>Eliminar</button></span></li>)}{query.data?.length === 0 && <li className="empty">Sin registros.</li>}</ul><ConfirmDeleteDialog open={deleteTarget !== null} itemName={deleteTarget?.nombre ?? ''} onCancel={() => setDeleteTarget(null)} onConfirm={() => deleteTarget && remove.mutate(deleteTarget.id)} /></article>
}