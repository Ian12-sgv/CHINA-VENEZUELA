import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { gruposApi, usuariosApi } from '../api'
import { ConfirmDeleteDialog } from '../components/ConfirmDeleteDialog'

export function GruposPage() {
  const client = useQueryClient()
  const [nombre, setNombre] = useState('')
  const [nombreEditando, setNombreEditando] = useState<string | null>(null)
  const [eliminando, setEliminando] = useState<string | null>(null)
  const [usuarioSeleccionado, setUsuarioSeleccionado] = useState('')
  const [gruposAsignados, setGruposAsignados] = useState<string[]>([])
  const [error, setError] = useState('')
  const [mensaje, setMensaje] = useState('')
  const gruposQuery = useQuery({ queryKey: ['grupos'], queryFn: gruposApi.listar })
  const usuariosQuery = useQuery({ queryKey: ['usuarios'], queryFn: usuariosApi.listar })
  const actualizar = () => {
    void client.invalidateQueries({ queryKey: ['grupos'] })
    void client.invalidateQueries({ queryKey: ['usuarios'] })
    void client.invalidateQueries({ queryKey: ['auth-grupos'] })
  }
  const guardar = useMutation({ mutationFn: () => nombreEditando ? gruposApi.actualizar(nombreEditando, nombre) : gruposApi.crear(nombre), onSuccess: () => { setNombre(''); setNombreEditando(null); actualizar() }, onError: motivo => setError(motivo instanceof Error ? motivo.message : 'No fue posible guardar el grupo.') })
  const eliminar = useMutation({ mutationFn: (valor: string) => gruposApi.eliminar(valor), onSuccess: () => { setEliminando(null); actualizar() }, onError: motivo => { setEliminando(null); setError(motivo instanceof Error ? motivo.message : 'No fue posible eliminar el grupo.') } })
  const asignar = useMutation({ mutationFn: () => usuariosApi.actualizarGrupos(usuarioSeleccionado, gruposAsignados), onSuccess: () => { setError(''); setMensaje('Asignacion guardada correctamente.'); actualizar() }, onError: motivo => { setMensaje(''); setError(motivo instanceof Error ? motivo.message : 'No fue posible asignar los grupos.') } })
  const submit = (event: FormEvent) => { event.preventDefault(); setError(''); guardar.mutate() }
  const editar = (valor: string) => { setNombreEditando(valor); setNombre(valor); setError('') }
  const usuario = usuariosQuery.data?.find(item => item.codigoUsuario === usuarioSeleccionado)

  useEffect(() => { if (usuario) setGruposAsignados(usuario.grupos) }, [usuarioSeleccionado, usuariosQuery.data])
  function cambiarGrupo(grupo: string, seleccionado: boolean) { setGruposAsignados(actual => seleccionado ? [...new Set([...actual, grupo])] : actual.filter(item => item !== grupo)) }

  return <section className="page-grid"><div className="page-heading"><div><span className="eyebrow">Seguridad</span><h2>Grupos de usuarios</h2><p>Administra grupos y asigna sus permisos a los usuarios.</p></div></div>
    <article className="card catalog-card tab-content"><div className="form-title"><div><h3>{nombreEditando ? 'Editar grupo' : 'Nuevo grupo'}</h3><p>Los grupos se muestran en el formulario de registro.</p></div>{nombreEditando && <button className="link-button" onClick={() => { setNombreEditando(null); setNombre('') }}>Cancelar</button>}</div><form className="inline-form" onSubmit={submit}><input aria-label="Nombre de grupo" placeholder="Nombre del grupo" value={nombre} onChange={event => setNombre(event.target.value)} maxLength={100} required /><button className="primary" disabled={guardar.isPending}>{nombreEditando ? 'Guardar cambios' : 'Agregar grupo'}</button></form>{error && <p className="error">{error}</p>}<ul className="catalog-list">{gruposQuery.data?.map(grupo => <li key={grupo.nombre}><span>{grupo.nombre}{grupo.protegido && <small>Grupo protegido</small>}</span>{!grupo.protegido && <span><button className="link-button" onClick={() => editar(grupo.nombre)}>Editar</button><button className="danger-button" onClick={() => setEliminando(grupo.nombre)}>Eliminar</button></span>}</li>)}{gruposQuery.data?.length === 0 && <li className="empty">Sin grupos.</li>}</ul></article>
    <article className="card catalog-card tab-content"><div className="form-title"><div><h3>Asignar grupos</h3><p>Selecciona un usuario y define los grupos a los que pertenece.</p></div></div><label><span>Usuario</span><select value={usuarioSeleccionado} onChange={event => setUsuarioSeleccionado(event.target.value)}><option value="">Selecciona un usuario</option>{usuariosQuery.data?.map(item => <option key={item.codigoUsuario} value={item.codigoUsuario}>{item.nombre} ({item.codigoUsuario})</option>)}</select></label>{usuarioSeleccionado && <div className="group-checkboxes">{gruposQuery.data?.map(grupo => <label key={grupo.nombre} className="group-checkbox"><input type="checkbox" checked={gruposAsignados.includes(grupo.nombre)} onChange={event => cambiarGrupo(grupo.nombre, event.target.checked)} /> <span>{grupo.nombre}</span></label>)}</div>}{mensaje && <p className="success">{mensaje}</p>}<button className="primary" disabled={!usuarioSeleccionado || gruposAsignados.length === 0 || asignar.isPending} onClick={() => { setError(''); setMensaje(''); asignar.mutate() }}>{asignar.isPending ? 'Guardando...' : 'Guardar asignacion'}</button></article>
    <ConfirmDeleteDialog open={eliminando !== null} itemName={eliminando ?? ''} onCancel={() => setEliminando(null)} onConfirm={() => eliminando && eliminar.mutate(eliminando)} />
  </section>
}