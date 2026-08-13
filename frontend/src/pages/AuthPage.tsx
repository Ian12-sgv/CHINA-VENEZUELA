import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { authApi, guardarSesion } from '../api'
import type { InicioSesionResponse } from '../types'

type Modo = 'ingresar' | 'registrar'
type Props = { onAuthenticated: (sesion: InicioSesionResponse) => void }

export function AuthPage({ onAuthenticated }: Props) {
  const [modo, setModo] = useState<Modo>('ingresar')
  const [codigoUsuario, setCodigoUsuario] = useState('')
  const [contrasena, setContrasena] = useState('')
  const [nombre, setNombre] = useState('')
  const [nombreGrupo, setNombreGrupo] = useState('')
  const [correo, setCorreo] = useState('')
  const [grupos, setGrupos] = useState<string[]>([])
  const [error, setError] = useState('')
  const [guardando, setGuardando] = useState(false)

  useEffect(() => {
    void authApi.grupos().then(resultado => {
      setGrupos(resultado)
      setNombreGrupo(actual => actual || resultado[0] || '')
    }).catch(() => setError('No fue posible cargar los grupos disponibles.'))
  }, [])

  async function enviar(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError('')
    setGuardando(true)
    try {
      if (modo === 'registrar') {
        await authApi.registrar({ codigoUsuario, nombre, correo, contrasena, nombreGrupo })
      }
      const sesion = await authApi.iniciarSesion({ nombre, contrasena })
      guardarSesion(sesion)
      onAuthenticated(sesion)
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'No fue posible completar la operacion.')
    } finally {
      setGuardando(false)
    }
  }

  function cambiarModo(nuevoModo: Modo) {
    setModo(nuevoModo)
    setError('')
  }

  return <main className="auth-shell">
    <section className="auth-card">
      <p className="eyebrow">China - Venezuela</p>
      <h1>Recibos de compra</h1>
      <p className="auth-description">Accede para registrar y consultar la mercancia recibida.</p>
      <div className="auth-tabs" role="tablist" aria-label="Acceso">
        <button type="button" className={modo === 'ingresar' ? 'active' : ''} onClick={() => cambiarModo('ingresar')}>Iniciar sesion</button>
        <button type="button" className={modo === 'registrar' ? 'active' : ''} onClick={() => cambiarModo('registrar')}>Registrarme</button>
      </div>
      <form className="auth-form" onSubmit={enviar}>
        <label><span>Nombre de usuario</span><input value={nombre} onChange={event => setNombre(event.target.value)} maxLength={200} required placeholder="Ej. Martha" autoComplete="username" /></label>
        {modo === 'registrar' && <>
          <label><span>Correo</span><input type="email" value={correo} onChange={event => setCorreo(event.target.value)} maxLength={254} required placeholder="correo@ejemplo.com" autoComplete="email" /></label><label><span>Grupo</span><select value={nombreGrupo} onChange={event => setNombreGrupo(event.target.value)} required disabled={grupos.length === 0}><option value="">Selecciona un grupo</option>{grupos.map(grupo => <option key={grupo} value={grupo}>{grupo}</option>)}</select></label>
          <label><span>Codigo de usuario</span><input value={codigoUsuario} onChange={event => setCodigoUsuario(event.target.value)} maxLength={50} required placeholder="Ej. MARTHA" /></label>
        </>}
        <label><span>Contrasena</span><input type="password" value={contrasena} onChange={event => setContrasena(event.target.value)} minLength={8} required placeholder="Minimo 8 caracteres" autoComplete={modo === 'ingresar' ? 'current-password' : 'new-password'} /></label>
        {error && <p className="error" role="alert">{error}</p>}
        <button className="primary auth-submit" disabled={guardando || (modo === 'registrar' && grupos.length === 0)}>{guardando ? 'Procesando...' : modo === 'ingresar' ? 'Iniciar sesion' : 'Crear cuenta'}</button>
      </form>
    </section>
  </main>
}
