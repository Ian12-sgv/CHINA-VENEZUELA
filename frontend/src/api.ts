import type { Catalogo, CatalogoTipo, CompraRecibida, CompraRecibidaRequest } from './types'

const baseUrl = import.meta.env.VITE_API_URL ?? '/api'

type ProblemDetails = { title?: string; detail?: string; errors?: Record<string, string[]> }

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${baseUrl}${path}`, { headers: { 'Content-Type': 'application/json', ...init?.headers }, ...init })
  if (response.ok) return response.status === 204 ? (undefined as T) : (await response.json() as T)
  const problem = await response.json().catch(() => ({})) as ProblemDetails
  const validation = problem.errors ? Object.values(problem.errors).flat().join(' ') : ''
  throw new Error(validation || problem.detail || problem.title || 'No fue posible completar la operación.')
}

export const comprasApi = {
  listar: () => request<CompraRecibida[]>('/compras-recibidas'),
  crear: (data: CompraRecibidaRequest) => request<CompraRecibida>('/compras-recibidas', { method: 'POST', body: JSON.stringify(data) }),
  actualizar: (id: string, data: CompraRecibidaRequest) => request<CompraRecibida>(`/compras-recibidas/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  eliminar: (id: string) => request<void>(`/compras-recibidas/${id}`, { method: 'DELETE' }),
}

export const catalogosApi = {
  listar: (tipo: CatalogoTipo) => request<Catalogo[]>(`/${tipo}`),
  crear: (tipo: CatalogoTipo, nombre: string) => request<Catalogo>(`/${tipo}`, { method: 'POST', body: JSON.stringify({ nombre }) }),
  actualizar: (tipo: CatalogoTipo, id: string, nombre: string) => request<Catalogo>(`/${tipo}/${id}`, { method: 'PUT', body: JSON.stringify({ nombre }) }),
  eliminar: (tipo: CatalogoTipo, id: string) => request<void>(`/${tipo}/${id}`, { method: 'DELETE' }),
}