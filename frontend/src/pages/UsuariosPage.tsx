import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { gruposApi, usuariosApi } from '../api'
import { ConfirmDeleteDialog } from '../components/ConfirmDeleteDialog'
import type { UsuarioAdministrable } from '../types'

const empty = { codigoUsuario: '', nombre: '', correo: '', contrasena: '', status: true, grupos: [] as string[] }

export function UsuariosPage() {
  const client = useQueryClient()
  const [form, setForm] = useState(empty)
  const [editando, setEditando] = useState<UsuarioAdministrable | null>(null)
  const [eliminando, setEliminando] = useState<UsuarioAdministrable | null>(null)
  const [error, setError] = useState('')
  const [mensaje, setMensaje] = useState('')
  const usuarios = useQuery({ queryKey: ['usuarios'], queryFn: usuariosApi.listar })
  const grupos = useQuery({ queryKey: ['grupos'], queryFn: gruposApi.listar })
  const refrescar = () => { void client.invalidateQueries({ queryKey: ['usuarios'] }) }
  const guardar = useMutation({
    mutationFn: () => editando
      ? usuariosApi.actualizar(editando.codigoUsuario, { nombre: form.nombre, correo: form.correo || undefined, contrasena: form.contrasena || undefined, status: form.status, grupos: form.grupos })
      : usuariosApi.crear(form),
    onSuccess: () => { setForm(empty); setEditando(null); setError(''); setMensaje('Usuario guardado correctamente.'); refrescar() },
    onError: motivo => { setMensaje(''); setError(motivo instanceof Error ? motivo.message : 'No fue posible guardar el usuario.') },
  })
  const eliminar = useMutation({ mutationFn: (codigo: string) => usuariosApi.eliminar(codigo), onSuccess: () => { setEliminando(null); setMensaje('Usuario eliminado correctamente.'); refrescar() }, onError: motivo => { setEliminando(null); setError(motivo instanceof Error ? motivo.message : 'No fue posible eliminar el usuario.') } })

  useEffect(() => { if (editando) setForm({ codigoUsuario: editando.codigoUsuario, nombre: editando.nombre, correo: editando.correo ?? '', contrasena: '', status: editando.status, grupos: editando.grupos }) }, [editando])
  const submit = (event: FormEvent) => { event.preventDefault(); setError(''); setMensaje(''); guardar.mutate() }
  const alternarGrupo = (nombre: string, activo: boolean) => setForm(actual => ({ ...actual, grupos: activo ? [...new Set([...actual.grupos, nombre])] : actual.grupos.filter(grupo => grupo !== nombre) }))
  const cancelar = () => { setEditando(null); setForm(empty); setError(''); setMensaje('') }

  return <section className="page-grid"><div className="page-heading"><div><span className="eyebrow">Seguridad</span><h2>Usuarios</h2><p>Solo Master y Sistemas crean, editan, eliminan usuarios y asignan sus grupos.</p></div></div>
    <article className="card catalog-card"><div className="form-title"><div><h3>{editando ? 'Editar usuario' : 'Nuevo usuario'}</h3><p>{editando ? 'Deja la contraseña vacía para conservar la actual.' : 'Selecciona al menos un grupo para el usuario.'}</p></div>{editando && <button className="link-button" onClick={cancelar}>Cancelar</button>}</div><form className="form-grid" onSubmit={submit}><label><span>Nombre</span><input value={form.nombre} onChange={event => setForm({ ...form, nombre: event.target.value })} required maxLength={200} /></label><label><span>Correo</span><input type="email" value={form.correo} onChange={event => setForm({ ...form, correo: event.target.value })} required maxLength={254} placeholder="correo@ejemplo.com" /></label><label><span>Codigo de usuario</span><input value={form.codigoUsuario} onChange={event => setForm({ ...form, codigoUsuario: event.target.value })} required disabled={editando !== null} maxLength={50} /></label><label><span>Contraseña</span><input type="password" value={form.contrasena} onChange={event => setForm({ ...form, contrasena: event.target.value })} required={!editando} minLength={editando ? undefined : 8} placeholder={editando ? 'Sin cambios' : 'Minimo 8 caracteres'} /></label><label className="user-status"><span>Estado</span><input type="checkbox" checked={form.status} onChange={event => setForm({ ...form, status: event.target.checked })} /> Activo</label><div className="description-field"><span className="groups-label">Grupos</span><div className="group-checkboxes">{grupos.data?.map(grupo => <label key={grupo.nombre} className="group-checkbox"><input type="checkbox" checked={form.grupos.includes(grupo.nombre)} onChange={event => alternarGrupo(grupo.nombre, event.target.checked)} /><span>{grupo.nombre}</span></label>)}</div></div><div className="description-field">{error && <p className="error">{error}</p>}{mensaje && <p className="success">{mensaje}</p>}<button className="primary" disabled={guardar.isPending || form.grupos.length === 0}>{guardar.isPending ? 'Guardando...' : editando ? 'Guardar cambios' : 'Crear usuario'}</button></div></form></article>
    <article className="card table-card"><div className="table-toolbar"><h3>Usuarios registrados</h3></div><div className="table-wrap"><table><thead><tr><th>Usuario</th><th>Codigo</th><th>Correo</th><th>Grupos</th><th>Estado</th><th>Acciones</th></tr></thead><tbody>{usuarios.data?.map(usuario => <tr key={usuario.codigoUsuario}><td>{usuario.nombre}</td><td>{usuario.codigoUsuario}</td><td>{usuario.correo ?? "Sin correo"}</td><td>{usuario.grupos.join(', ') || 'Sin grupo'}</td><td>{usuario.status ? 'Activo' : 'Inactivo'}</td><td className="actions"><button className="link-button" onClick={() => setEditando(usuario)}>Editar</button>{!['MS', 'SIS'].includes(usuario.codigoUsuario) && <button className="danger-button" onClick={() => setEliminando(usuario)}>Eliminar</button>}</td></tr>)}{usuarios.data?.length === 0 && <tr><td colSpan={6} className="empty">Sin usuarios.</td></tr>}</tbody></table></div></article>
    <ConfirmDeleteDialog open={eliminando !== null} itemName={eliminando?.nombre ?? ''} onCancel={() => setEliminando(null)} onConfirm={() => eliminando && eliminar.mutate(eliminando.codigoUsuario)} />
  </section>
}
