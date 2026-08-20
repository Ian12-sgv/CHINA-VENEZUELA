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
  receptorCodigoUsuario: string | null
  receptorNombre: string | null
  receptorCorreo: string | null
  fechaCreacionUtc: string
  fechaActualizacionUtc: string | null
  fechaComprobanteEnviadoUtc: string | null
}

export type CompraRecibidaRequest = Omit<CompraRecibida, 'id' | 'receptorNombre' | 'receptorCorreo' | 'fechaCreacionUtc' | 'fechaActualizacionUtc' | 'fechaComprobanteEnviadoUtc'>
export type CatalogoTipo = 'empresas' | 'marcas-bulto' | 'contenedores-compartidos' | 'aduanas' | 'puertos-llegada'

export type UsuarioSesion = {
  codigoUsuario: string
  nombre: string
  correo: string | null
  status: boolean
  grupos: string[]
}

export type InicioSesionResponse = { token: string; usuario: UsuarioSesion }
export type IniciarSesionRequest = { nombre: string; contrasena: string }
export type Grupo = { nombre: string; protegido: boolean }
export type UsuarioAdministrable = UsuarioSesion
export type CuentaUsuario = { codigoUsuario: string; nombre: string; correo: string }
export type CrearUsuarioAdministrativoRequest = { codigoUsuario: string; nombre: string; correo: string; contrasena: string; status: boolean; grupos: string[] }
export type ActualizarUsuarioAdministrativoRequest = { nombre: string; correo?: string; contrasena?: string; status: boolean; grupos: string[] }
export type ClasificacionEmpresa = 'Oriente' | 'Occidente' | 'Aliada'
export type Empresa = { id: string; nombre: string; rif: string | null; clasificacion: ClasificacionEmpresa | null }
export type EmpresaRequest = { nombre: string; rif: string; clasificacion: ClasificacionEmpresa }









export type ProductoPedido = { id: string; codigoBarra: string; referencia: string; nombre: string; marca: string | null; categoria: string; talla: string | null; color: string | null; fabricante: string | null; precioDetal: number; costo: number; fechaPedido: string; activo: boolean; enviado: boolean; fechaEnvioUtc: string | null; tieneImagen: boolean; creadoPorCodigoUsuario: string; fechaCreacionUtc: string }
export type CrearProductoPedidoRequest = Omit<ProductoPedido, 'id' | 'activo' | 'enviado' | 'fechaEnvioUtc' | 'tieneImagen' | 'creadoPorCodigoUsuario' | 'fechaCreacionUtc'>
export type RegistroPrecioPedido = { id: string; codigoBarra: string; producto: string; sucursal: string; precioSistema: number; precioVerificado: number }
export type PaginaProductosPedido = { items: ProductoPedido[]; total: number; pagina: number; tamanoPagina: number; totalPaginas: number }

export type ActualizarProductoPedidoRequest = CrearProductoPedidoRequest
