import type {
  Catalogo,
  CatalogoTipo,
  CompraRecibida,
  CompraRecibidaRequest,
  InicioSesionResponse,
  IniciarSesionRequest,
  UsuarioSesion,
  Grupo,
  UsuarioAdministrable,
  Empresa,
  EmpresaRequest,
  CrearUsuarioAdministrativoRequest,
  ActualizarUsuarioAdministrativoRequest,
  ProductoPedido,
  CrearProductoPedidoRequest,
  ActualizarProductoPedidoRequest,
  RegistroPrecioPedido,
  PaginaProductosPedido,
  CuentaUsuario,
} from './types'

export const apiBaseUrl = import.meta.env.VITE_API_URL ?? '/api'
export const actualizacionesHubUrl = apiBaseUrl.replace(/\/api\/?$/, '/hub/actualizaciones')
const sesionKey = 'china-venezuela-sesion'

type ProblemDetails = { title?: string; detail?: string; errors?: Record<string, string[]> }

export function obtenerSesion(): InicioSesionResponse | null {
  try {
    const valor = sessionStorage.getItem(sesionKey)
    return valor ? JSON.parse(valor) as InicioSesionResponse : null
  } catch {
    return null
  }
}

export function guardarSesion(sesion: InicioSesionResponse) { sessionStorage.setItem(sesionKey, JSON.stringify(sesion)) }
export function limpiarSesion() { sessionStorage.removeItem(sesionKey) }
export function getAccessToken() { return obtenerSesion()?.token ?? null }

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const token = getAccessToken()
  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...init,
    headers: { 'Content-Type': 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}), ...init?.headers },
  })
  if (response.ok) return response.status === 204 ? (undefined as T) : (await response.json() as T)
  if (response.status === 401) { limpiarSesion(); window.dispatchEvent(new Event('sesion-invalida')) }
  const problem = await response.json().catch(() => ({})) as ProblemDetails
  const validation = problem.errors ? Object.values(problem.errors).flat().join(' ') : ''
  throw new Error(validation || problem.detail || problem.title || 'No fue posible completar la operacion.')
}

async function requestMultipart<T>(path: string, formData: FormData): Promise<T> {
  const token = getAccessToken()
  const response = await fetch(`${apiBaseUrl}${path}`, { method: 'PUT', body: formData, headers: token ? { Authorization: `Bearer ${token}` } : undefined })
  if (response.ok) return await response.json() as T
  const problem = await response.json().catch(() => ({})) as ProblemDetails
  const validation = problem.errors ? Object.values(problem.errors).flat().join(' ') : ''
  throw new Error(validation || problem.detail || problem.title || 'No fue posible cargar la imagen.')
}

async function obtenerImagen(path: string): Promise<string | null> {
  const token = getAccessToken()
  const response = await fetch(`${apiBaseUrl}${path}`, { headers: token ? { Authorization: `Bearer ${token}` } : undefined })
  if (response.status === 404) return null
  if (!response.ok) throw new Error('No fue posible cargar la imagen.')
  return URL.createObjectURL(await response.blob())
}
export const authApi = {
  iniciarSesion: (data: IniciarSesionRequest) => request<InicioSesionResponse>('/auth/iniciar-sesion', { method: 'POST', body: JSON.stringify(data) }),
}

export const cuentaApi = {
  obtener: () => request<CuentaUsuario>('/cuenta')
}

export const gruposApi = {
  listar: () => request<Grupo[]>('/grupos'),
  crear: (nombre: string) => request<Grupo>('/grupos', { method: 'POST', body: JSON.stringify({ nombre }) }),
  actualizar: (nombreActual: string, nombre: string) => request<Grupo>(`/grupos/${encodeURIComponent(nombreActual)}`, { method: 'PUT', body: JSON.stringify({ nombre }) }),
  eliminar: (nombre: string) => request<void>(`/grupos/${encodeURIComponent(nombre)}`, { method: 'DELETE' }),
}

export const usuariosApi = {
  listar: () => request<UsuarioAdministrable[]>('/usuarios'),
  crear: (data: CrearUsuarioAdministrativoRequest) => request<UsuarioAdministrable>('/usuarios', { method: 'POST', body: JSON.stringify(data) }),
  actualizar: (codigoUsuario: string, data: ActualizarUsuarioAdministrativoRequest) => request<UsuarioAdministrable>(`/usuarios/${encodeURIComponent(codigoUsuario)}`, { method: 'PUT', body: JSON.stringify(data) }),
  actualizarGrupos: (codigoUsuario: string, grupos: string[]) => request<UsuarioAdministrable>(`/usuarios/${encodeURIComponent(codigoUsuario)}/grupos`, { method: 'PUT', body: JSON.stringify({ grupos }) }),
  eliminar: (codigoUsuario: string) => request<void>(`/usuarios/${encodeURIComponent(codigoUsuario)}`, { method: 'DELETE' }),
}
export const receptoresApi = {
  listar: () => request<UsuarioSesion[]>('/receptores'),
}
export const comprasApi = {
  listar: () => request<CompraRecibida[]>('/compras-recibidas'),
  crear: (data: CompraRecibidaRequest) => request<CompraRecibida>('/compras-recibidas', { method: 'POST', body: JSON.stringify(data) }),
  actualizar: (id: string, data: CompraRecibidaRequest) => request<CompraRecibida>(`/compras-recibidas/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  eliminar: (id: string) => request<void>(`/compras-recibidas/${id}`, { method: 'DELETE' }),
  enviarComprobante: (id: string) => request<{ receptor: string; copia: string; enviadoEnUtc: string }>(`/compras-recibidas/${id}/comprobante/enviar`, { method: 'POST' }),
}

export const empresasApi = {
  listar: () => request<Empresa[]>('/empresas'),
  crear: (data: EmpresaRequest) => request<Empresa>('/empresas', { method: 'POST', body: JSON.stringify(data) }),
  actualizar: (id: string, data: EmpresaRequest) => request<Empresa>(`/empresas/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  eliminar: (id: string) => request<void>(`/empresas/${id}`, { method: 'DELETE' }),
}
export const catalogosApi = {
  listar: (tipo: CatalogoTipo) => request<Catalogo[]>(`/${tipo}`),
  crear: (tipo: CatalogoTipo, nombre: string) => request<Catalogo>(`/${tipo}`, { method: 'POST', body: JSON.stringify({ nombre }) }),
  actualizar: (tipo: CatalogoTipo, id: string, nombre: string) => request<Catalogo>(`/${tipo}/${id}`, { method: 'PUT', body: JSON.stringify({ nombre }) }),
  eliminar: (tipo: CatalogoTipo, id: string) => request<void>(`/${tipo}/${id}`, { method: 'DELETE' }),
}



export const pedidosApi = {
  productos: (pagina = 1, busqueda = '', fechaPedido = '', enviado?: boolean) => request<PaginaProductosPedido>(`/pedidos/productos?pagina=${pagina}&tamanoPagina=10${busqueda ? `&busqueda=${encodeURIComponent(busqueda)}` : ''}${fechaPedido ? `&fechaPedido=${fechaPedido}` : ''}${enviado === undefined ? '' : `&enviado=${enviado}`}`),
  crearProducto: (data: CrearProductoPedidoRequest) => request<ProductoPedido>('/pedidos/productos', { method: 'POST', body: JSON.stringify(data) }),
  actualizarProducto: (id: string, data: ActualizarProductoPedidoRequest) => request<ProductoPedido>(`/pedidos/productos/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  eliminarProducto: (id: string) => request<void>(`/pedidos/productos/${id}`, { method: 'DELETE' }),
  enviarProducto: (id: string, receptorCodigoUsuario: string) => request<{ receptor: string; copia: string; enviadoEnUtc: string }>(`/pedidos/productos/${id}/enviar`, { method: 'POST', body: JSON.stringify({ receptorCodigoUsuario }) }),
  subirImagen: (id: string, archivo: File) => { const data = new FormData(); data.append('imagen', archivo); return requestMultipart(`/pedidos/productos/${id}/imagen`, data) },
  obtenerImagen: (id: string) => obtenerImagen(`/pedidos/productos/${id}/imagen`),
  eliminarImagen: (id: string) => request<void>(`/pedidos/productos/${id}/imagen`, { method: 'DELETE' }),
  registrosPrecios: () => request<RegistroPrecioPedido[]>('/pedidos/registros-precios'),
}
