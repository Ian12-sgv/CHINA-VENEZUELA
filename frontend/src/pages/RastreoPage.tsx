import { useEffect, useMemo, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faBox, faClipboardCheck, faGlobe, faHouse, faIndustry, faListCheck, faMagnifyingGlass, faPaperPlane, faWrench } from '@fortawesome/free-solid-svg-icons'
import { pedidosApi } from '../api'
import type { ProductoPedido } from '../types'

type EtapaRastreo = {
  titulo: string
  descripcion: string
  icono: typeof faListCheck
}

const etapas: EtapaRastreo[] = [
  { titulo: 'Lista creada', descripcion: 'Pedido registrado', icono: faListCheck },
  { titulo: 'Pedido confirmado', descripcion: 'Datos del pedido listos', icono: faClipboardCheck },
  { titulo: 'Fábrica recibió', descripcion: 'Proveedor recibe el pedido', icono: faIndustry },
  { titulo: 'En producción', descripcion: 'Fábrica inicia el pedido', icono: faWrench },
  { titulo: 'Producción lista', descripcion: 'Fábrica termina el pedido', icono: faBox },
  { titulo: 'Control de calidad', descripcion: 'Pedido en validación', icono: faMagnifyingGlass },
  { titulo: 'Validado y enviado', descripcion: 'Pedido despachado', icono: faPaperPlane },
  { titulo: 'En tránsito', descripcion: 'Camino a Venezuela', icono: faGlobe },
  { titulo: 'Entregado', descripcion: 'Cliente confirma recepción', icono: faHouse },
]

function etapaCalculada(pedido: ProductoPedido) {
  if (pedido.enviado) return 6
  if (pedido.fechaInicioFabricacion) return 3
  return 1
}

function identificadorPedido(pedido: ProductoPedido) {
  return pedido.referenciaAsignada || pedido.codigoBarraAsignado || pedido.id.slice(0, 8)
}

export function RastreoPage() {
  const pedidos = useQuery({ queryKey: ['rastreo-pedidos'], queryFn: () => pedidosApi.productos(1, '', undefined, 5000) })
  const [pedidoId, setPedidoId] = useState('')
  const [etapaPrueba, setEtapaPrueba] = useState<number | null>(null)
  const [sinEtapaPrueba, setSinEtapaPrueba] = useState(false)
  const [etapaDesmarcada, setEtapaDesmarcada] = useState<number | null>(null)
  const items = pedidos.data?.items ?? []

  useEffect(() => {
    if (!pedidoId && items[0]) setPedidoId(items[0].id)
    if (pedidoId && !items.some(item => item.id === pedidoId)) setPedidoId(items[0]?.id ?? '')
  }, [items, pedidoId])

  useEffect(() => { setEtapaPrueba(null); setSinEtapaPrueba(false); setEtapaDesmarcada(null) }, [pedidoId])

  const pedido = useMemo(() => items.find(item => item.id === pedidoId) ?? items[0] ?? null, [items, pedidoId])

  if (pedidos.isLoading) return <p className="loading">Cargando rastreo de pedidos...</p>
  if (pedidos.isError) return <p className="error">No fue posible cargar el rastreo de pedidos.</p>
  if (!pedido) return <section className="page-grid"><div className="rastreo-hero"><span className="eyebrow">Rastreo en vivo</span><h2>Sabes dónde está tu pedido, siempre.</h2></div><article className="card rastreo-empty"><h3>Aún no hay pedidos para rastrear</h3><p>Registra un pedido para visualizar su avance aquí.</p></article></section>

  const actual = sinEtapaPrueba ? -1 : etapaPrueba ?? etapaCalculada(pedido)
  const etapa = actual >= 0 ? etapas[actual] : null
  const hastaCompletada = sinEtapaPrueba ? etapaDesmarcada ?? 0 : actual
  const alternarEtapaPrueba = (indice: number) => {
    if (indice === actual) {
      setEtapaPrueba(null)
      setSinEtapaPrueba(true)
      setEtapaDesmarcada(indice)
      return
    }
    setEtapaPrueba(indice)
    setSinEtapaPrueba(false)
    setEtapaDesmarcada(null)
  }

  return <section className="rastreo-page">
    <div className="rastreo-hero"><div><span className="eyebrow">Rastreo en vivo</span><h2>Sabes dónde está tu pedido, siempre.</h2></div><label className="rastreo-selector"><span>Pedido</span><select value={pedido.id} onChange={event => setPedidoId(event.target.value)}>{items.map(item => <option key={item.id} value={item.id}>#{identificadorPedido(item)}</option>)}</select></label></div>
    <article className="rastreo-timeline-card">
      <div className="rastreo-progress" aria-label={etapa ? `Etapa actual: ${etapa.titulo}` : 'Sin etapa marcada'}>
        {etapas.map((item, indice) => <button type="button" key={item.titulo} className={`rastreo-stage ${indice < hastaCompletada ? 'completed' : ''} ${indice === actual ? 'current' : ''}`} onClick={() => alternarEtapaPrueba(indice)} aria-pressed={indice === actual} aria-label={`Marcar ${item.titulo} como etapa actual`}>
          <span className="rastreo-stage-line" aria-hidden="true" />
          <span className="rastreo-stage-icon"><FontAwesomeIcon icon={item.icono} /></span>
          <span className="rastreo-stage-text"><strong>{item.titulo}</strong><small>{item.descripcion}</small>{indice === actual && <em>En curso</em>}</span>
        </button>)}
      </div>
      <footer className="rastreo-detail"><strong>Pedido #{identificadorPedido(pedido)}</strong><span>Grupo: {pedido.grupoPedidoNombre ?? 'Sin grupo'}</span><span>Estado actual: {etapa?.titulo ?? 'Sin marcar'}</span>{pedido.fechaEnvioUtc && <span>Enviado: {new Date(pedido.fechaEnvioUtc).toLocaleDateString('es-VE')}</span>}<span className="rastreo-test-note">Modo prueba: haz clic en una etapa para marcarla.</span></footer>
    </article>
  </section>
}