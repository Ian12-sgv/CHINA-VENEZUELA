import { useState } from 'react'
import { CatalogosPage } from './pages/CatalogosPage'
import { ComprasRecibidasPage } from './pages/ComprasRecibidasPage'

type View = 'compras-recibidas' | 'campos-compras'

export default function App() {
  const [view, setView] = useState<View>('compras-recibidas')
  return <div className="app-shell">
    <header className="topbar"><div className="header-left"><div className="brand"><span>China → Venezuela</span><h1>Recibos de compra</h1></div></div>
      <nav className="header-pills" aria-label="Navegación principal"><button className={view === 'compras-recibidas' ? 'active' : ''} onClick={() => setView('compras-recibidas')}>Recibos de compra</button><button className={view === 'campos-compras' ? 'active' : ''} onClick={() => setView('campos-compras')}>CamposCompras</button></nav>
    </header>
    <main>{view === 'compras-recibidas' ? <ComprasRecibidasPage /> : <CatalogosPage />}</main>
  </div>
}