import { useQuery } from '@tanstack/react-query'
import { cuentaApi } from '../api'

export function CuentaPage() {
  const cuenta = useQuery({ queryKey: ['cuenta'], queryFn: cuentaApi.obtener, retry: false, refetchOnMount: 'always' })
  const datos = cuenta.data

  return <section className="page-grid">
    <div className="page-heading"><div><span className="eyebrow">Perfil</span><h2>Cuenta</h2><p>Consulta los datos registrados para tu cuenta.</p></div></div>
    <article className="card cuenta-card">
      {cuenta.isLoading && <p className="empty">Cargando datos de la cuenta...</p>}
      {cuenta.isError && <p className="error">{cuenta.error instanceof Error ? cuenta.error.message : 'No fue posible cargar los datos de tu cuenta.'}</p>}
      {datos && <div className="cuenta-profile"><div className="cuenta-avatar">{datos.nombre.charAt(0).toUpperCase()}</div><div><h3>{datos.nombre}</h3><p>{datos.correo}</p></div></div>}
    </article>
  </section>
}