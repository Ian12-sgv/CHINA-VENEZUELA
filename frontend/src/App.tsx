import { useEffect, useState } from 'react'
import { limpiarSesion, obtenerSesion } from './api'
import { CatalogosPage } from './pages/CatalogosPage'
import { ComprasRecibidasPage } from './pages/ComprasRecibidasPage'
import { AuthPage } from './pages/AuthPage'
import { GruposPage } from './pages/GruposPage'
import { UsuariosPage } from './pages/UsuariosPage'
import { PedidosPage } from './pages/PedidosPage'
import { useActualizacionesEnTiempoReal } from './hooks/useActualizacionesEnTiempoReal'
import type { InicioSesionResponse } from './types'

type View = 'compras-recibidas' | 'pedidos' | 'campos-compras' | 'grupos' | 'usuarios'

export default function App() {
  const [sesion, setSesion] = useState<InicioSesionResponse | null>(() => obtenerSesion())
  useEffect(() => {
    const manejarSesionInvalida = () => setSesion(null)
    window.addEventListener('sesion-invalida', manejarSesionInvalida)
    return () => window.removeEventListener('sesion-invalida', manejarSesionInvalida)
  }, [])
  if (!sesion) return <AuthPage onAuthenticated={setSesion} />
  return <Aplicacion sesion={sesion} onCerrarSesion={() => { limpiarSesion(); setSesion(null) }} />
}

function perteneceAGrupo(grupos: string[], nombre: string) {
  return grupos.some(grupo => grupo.trim().toLocaleLowerCase() === nombre.toLocaleLowerCase())
}

function Aplicacion({ sesion, onCerrarSesion }: { sesion: InicioSesionResponse; onCerrarSesion: () => void }) {
  const esAdministrador = ['MS', 'SIS'].includes(sesion.usuario.codigoUsuario)
  const puedeVerCompras = esAdministrador || perteneceAGrupo(sesion.usuario.grupos, 'oficina')
  const puedeVerPedidos = esAdministrador || perteneceAGrupo(sesion.usuario.grupos, 'Pedidos')
  const puedeGestionarGrupos = esAdministrador
  const [view, setView] = useState<View>(() => puedeVerPedidos && !puedeVerCompras ? 'pedidos' : 'compras-recibidas')
  useActualizacionesEnTiempoReal()

  if (!puedeVerCompras && !puedeVerPedidos) {
    return <div className="app-shell"><header className="topbar"><div className="header-left"><div className="brand"><span>China - Venezuela</span><h1>Recibos de compra</h1></div></div><div className="session-controls"><span>{sesion.usuario.nombre}</span><button type="button" onClick={onCerrarSesion}>Salir</button></div></header><main><section className="page-grid"><p className="error">Tu usuario no pertenece a un grupo con acceso a modulos. Solicita al administrador que te asigne a oficina o Pedidos.</p></section></main></div>
  }

  return <div className="app-shell">
    <header className="topbar"><div className="header-left"><div className="brand"><span>China - Venezuela</span><h1>Recibos de compra</h1></div></div>
      <div className="header-actions"><nav className="header-pills" aria-label="Navegacion principal">{puedeVerCompras && <><button className={view === 'compras-recibidas' ? 'active' : ''} onClick={() => setView('compras-recibidas')}>Recibos de compra</button><button className={view === 'campos-compras' ? 'active' : ''} onClick={() => setView('campos-compras')}>CamposCompras</button></>}{puedeVerPedidos && <button className={view === 'pedidos' ? 'active' : ''} onClick={() => setView('pedidos')}>Pedidos</button>}{puedeGestionarGrupos && <><button className={view === 'grupos' ? 'active' : ''} onClick={() => setView('grupos')}>Grupos</button><button className={view === 'usuarios' ? 'active' : ''} onClick={() => setView('usuarios')}>Usuarios</button></>}</nav><div className="session-controls"><span>{sesion.usuario.nombre}</span><button type="button" onClick={onCerrarSesion}>Salir</button></div></div>
    </header>
    <main>{view === 'compras-recibidas' && puedeVerCompras ? <ComprasRecibidasPage /> : view === 'pedidos' && puedeVerPedidos ? <PedidosPage /> : view === 'campos-compras' && puedeVerCompras ? <CatalogosPage /> : view === 'grupos' && puedeGestionarGrupos ? <GruposPage /> : view === 'usuarios' && puedeGestionarGrupos ? <UsuariosPage /> : null}</main>
  </div>
}