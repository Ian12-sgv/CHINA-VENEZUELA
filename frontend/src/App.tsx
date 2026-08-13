import { useState } from 'react'
import { limpiarSesion, obtenerSesion } from './api'
import { CatalogosPage } from './pages/CatalogosPage'
import { ComprasRecibidasPage } from './pages/ComprasRecibidasPage'
import { AuthPage } from './pages/AuthPage'
import { GruposPage } from './pages/GruposPage'
import { UsuariosPage } from './pages/UsuariosPage'
import { useActualizacionesEnTiempoReal } from './hooks/useActualizacionesEnTiempoReal'
import type { InicioSesionResponse } from './types'

type View = 'compras-recibidas' | 'campos-compras' | 'grupos' | 'usuarios'

export default function App() {
  const [sesion, setSesion] = useState<InicioSesionResponse | null>(() => obtenerSesion())
  if (!sesion) return <AuthPage onAuthenticated={setSesion} />
  return <Aplicacion sesion={sesion} onCerrarSesion={() => { limpiarSesion(); setSesion(null) }} />
}

function Aplicacion({ sesion, onCerrarSesion }: { sesion: InicioSesionResponse; onCerrarSesion: () => void }) {
  const [view, setView] = useState<View>('compras-recibidas')
  const puedeGestionarGrupos = ['MS', 'SIS'].includes(sesion.usuario.codigoUsuario)
  useActualizacionesEnTiempoReal()

  return <div className="app-shell">
    <header className="topbar"><div className="header-left"><div className="brand"><span>China - Venezuela</span><h1>Recibos de compra</h1></div></div>
      <div className="header-actions"><nav className="header-pills" aria-label="Navegacion principal"><button className={view === 'compras-recibidas' ? 'active' : ''} onClick={() => setView('compras-recibidas')}>Recibos de compra</button><button className={view === 'campos-compras' ? 'active' : ''} onClick={() => setView('campos-compras')}>CamposCompras</button>{puedeGestionarGrupos && <><button className={view === 'grupos' ? 'active' : ''} onClick={() => setView('grupos')}>Grupos</button><button className={view === 'usuarios' ? 'active' : ''} onClick={() => setView('usuarios')}>Usuarios</button></>}</nav><div className="session-controls"><span>{sesion.usuario.nombre}</span><button type="button" onClick={onCerrarSesion}>Salir</button></div></div>
    </header>
    <main>{view === 'compras-recibidas' ? <ComprasRecibidasPage /> : view === 'campos-compras' ? <CatalogosPage /> : view === 'grupos' && puedeGestionarGrupos ? <GruposPage /> : view === 'usuarios' && puedeGestionarGrupos ? <UsuariosPage /> : null}</main>
  </div>
}