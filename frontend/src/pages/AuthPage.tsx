import { useState } from 'react'
import type { FormEvent } from 'react'
import { authApi, guardarSesion } from '../api'
import type { InicioSesionResponse } from '../types'

type Props = { onAuthenticated: (sesion: InicioSesionResponse) => void }

export function AuthPage({ onAuthenticated }: Props) {
  const [contrasena, setContrasena] = useState('')
  const [nombre, setNombre] = useState('')
  const [error, setError] = useState('')
  const [guardando, setGuardando] = useState(false)

  async function enviar(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError('')
    setGuardando(true)
    try {
      const sesion = await authApi.iniciarSesion({ nombre, contrasena })
      guardarSesion(sesion)
      onAuthenticated(sesion)
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : 'No fue posible iniciar sesion.')
    } finally {
      setGuardando(false)
    }
  }

  return <main className="auth-shell">
    <section className="auth-card">
      <p className="eyebrow">China - Venezuela</p>
      <h1>Recibos de compra</h1>
      <p className="auth-description">Ingresa con las credenciales entregadas por el administrador del sistema.</p>
      <form className="auth-form" onSubmit={enviar}>
        <label><span>Nombre de usuario</span><input value={nombre} onChange={event => setNombre(event.target.value)} maxLength={200} required placeholder="Ej. Martha" autoComplete="username" /></label>
        <label><span>Contrasena</span><input type="password" value={contrasena} onChange={event => setContrasena(event.target.value)} minLength={8} required placeholder="Tu contrasena" autoComplete="current-password" /></label>
        {error && <p className="error" role="alert">{error}</p>}
        <button className="primary auth-submit" disabled={guardando}>{guardando ? 'Procesando...' : 'Iniciar sesion'}</button>
      </form>
    </section>
  </main>
}