export type Catalogo = { id: string; nombre: string }

export type CompraRecibida = {
  id: string
  contenedorCompartidoId: string | null
  nombreContenedor: string
  numeroContenedor: string
  empresaId: string
  descripcion: string | null
  fechaSalida: string
  fechaLlegada: string | null
  aduana: string | null
  puertoLlegada: string
  marcaBultoId: string | null
  fechaCreacionUtc: string
  fechaActualizacionUtc: string | null
}

export type CompraRecibidaRequest = Omit<CompraRecibida, 'id' | 'fechaCreacionUtc' | 'fechaActualizacionUtc'>
export type CatalogoTipo = 'empresas' | 'marcas-bulto' | 'contenedores-compartidos'